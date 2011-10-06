/************************************************************************
 * Copyright (c) 2006-2008, Jason Whitehorn (jason.whitehorn@gmail.com)
 * All rights reserved.
 * 
 * Source code and binaries distributed under the terms of the included
 * license, see license.txt for details.
 ************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Web;
using aspNETserve.Core;

namespace aspNETserve {

    /// <summary>
    /// Represents a HTTP communications channel over a Stream.
    /// </summary>
    public class HttpRequestResponse : MarshalByRefObject, IRequest, IResponse, ITransaction {

        private readonly Stream _stream;
        private readonly Guid _requestId = Guid.NewGuid();
        private readonly IPEndPoint _localEndPoint;
        private readonly IPEndPoint _remoteEndPoint;

        private const int _bufferSize = 1024;

        private string _httpMethod;
        //private NameValueCollection _queryString;
        private Uri _url;
        private string _rawUrl;
        private byte[] _postData;
        private string[] _knownRequestHeaders;
        private IDictionary<string, string> _unknownRequestHeaders;

        private int _statusCode = (int)HttpResponseCode.Ok;
        private string _statusDescription = "OK";
        private byte[] _rawData;
        private string _mimeType = "text/html";
        private string[] _knownResponseHeaders;
        private Hashtable _unknownResponseHeaders;
        private bool _headersSent = false;

        /// <summary>
        /// Creates an instance of HttpSocketRequestResponse from the supplied Stream.
        /// </summary>
        /// <param name="timeout">The timeout in milliseconds to wait for data.</param>
        /// <param name="stream">The Stream to send and receive HTTP data over.</param>
        public HttpRequestResponse(Stream stream, IPEndPoint localEndPoint, IPEndPoint remoteEndPoint, int timeout) {
            /*
             * The whole IPEndPoint for localEndPoint and remoteEndPoint is very much a leaky abstraction.
             * For the moment there isn't much I can do as classes "down stream" need them.
             * Those down stream classes will need to be fixed first.
             */ 
            if (stream == null)
                throw new ArgumentNullException("stream", "Stream cannot be null.");

            if(!stream.CanTimeout && timeout > 0)
                throw new ArgumentException("The supplied stream does not support read timeouts. Please supply an alternative stream or use a different constructor.");

            _localEndPoint = localEndPoint;
            _remoteEndPoint = remoteEndPoint;

            _knownRequestHeaders = new string[HttpWorkerRequest.RequestHeaderMaximum];
            for (int i = 0; i != HttpWorkerRequest.RequestHeaderMaximum; i++)
                _knownRequestHeaders[i] = "";
            _unknownRequestHeaders = new Dictionary<string, string>(20);

            _unknownResponseHeaders = new Hashtable();
            _knownResponseHeaders = new string[HttpWorkerRequest.ResponseHeaderMaximum];
            for (int i = 0; i != HttpWorkerRequest.ResponseHeaderMaximum; i++)
                _knownResponseHeaders[i] = "";

            _stream = stream;

            ParseHttpRequest(_stream, timeout);
        }

        /// <summary>
        /// Creates an instance of HttpSocketRequestResponse from the supplied Stream. This constructor will wait forever for HTTP data.
        /// </summary>
        public HttpRequestResponse(Stream stream, IPEndPoint localEndPoint, IPEndPoint remoteEndPoint) : this(stream, localEndPoint, remoteEndPoint, 0) { }

        public IRequest Request {
            get { return this; }
        }

        public IResponse Response {
            get { return this; }
        }

        #region IRequest Members

        string IRequest.HttpMethod {
            get { return _httpMethod; }
        }

        NameValueCollection IRequest.QueryString {
            get {
                throw new NotImplementedException();
            }
        }

        Uri IRequest.Url {
            get { return _url; }
        }

        string IRequest.RawUrl {
            get { return _rawUrl; }
        }

        byte[] IRequest.PostData {
            get { return _postData; }
        }

        string IRequest.GetKnownRequestHeader(int index) {
            return _knownRequestHeaders[index];
        }

        string IRequest.GetUnknownRequestHeader(string name) {
            return _unknownRequestHeaders[name];
        }

        IPEndPoint IRequest.LocalEndPoint {
            get { return _localEndPoint; }
        }

        IPEndPoint IRequest.RemoteEndPoint {
            get { return _remoteEndPoint; }
        }

        Guid IRequest.RequestId {
            get { return _requestId; }
        }

        bool IRequest.IsKeepAlive {
            get { return Request.GetKnownRequestHeader(HttpWorkerRequest.HeaderConnection).ToLower() == "keep-alive"; }
        }

        string IRequest.HttpVersion {
            get { 
                return "HTTP/1.1"; //TODO This is hardcoded for now 
            }
        }

        bool IRequest.IsSecure {
            get {
                return IsSecure();
            }
        }

        #endregion

        #region IResponse Members

        int IResponse.StatusCode {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        string IResponse.StatusDescription {
            get { return _statusDescription; }
            set { _statusDescription = value; }
        }

        byte[] IResponse.RawData {
            get { return _rawData; }
            set { _rawData = value; }
        }

        string IResponse.MimeType {
            get { return _mimeType; }
            set { _mimeType = value; }
        }

        void IResponse.SendKnownResponseHeader(int index, string value) {
            _knownResponseHeaders[index] = value;
        }

        void IResponse.SendUnknownResponseHeader(string name, string value) {
            _unknownResponseHeaders[name] = value;
        }

        bool IResponse.IsSecure {
            get {
                return IsSecure();
            }
        }

        void IResponse.Flush() {
            byte[] rawResponse = ToHttpResponse();
            _stream.Write(rawResponse, 0, rawResponse.Length);
        }

        bool IResponse.HeadersSent {
            get { return _headersSent; }
        }

        #endregion

        protected void ParseHttpRequest(Stream s, int timeout) {
            if(s.CanTimeout && timeout > 0)
                s.ReadTimeout = timeout;
            byte[] rawDataBuffer = new byte[0]; //start with a empty set of data... this array will be resized below.

            bool headerReceived = false;
            while (!headerReceived) {   //loop while the crlf crlf sequence signaling the end of the HTTP header has not been reached...
                byte[] buffer = new byte[_bufferSize];
                
                int bytesRead = s.Read(buffer, 0, _bufferSize);
                if(bytesRead == 0)
                    break;  //since there is nothing left then stop

                int oldRawDataBufferSize = rawDataBuffer.Length;
                //resize the raw data buffer to hold the new contents, and copy it over
                Array.Resize(ref rawDataBuffer, rawDataBuffer.Length + bytesRead);
                Buffer.BlockCopy(buffer, 0, rawDataBuffer, oldRawDataBufferSize, bytesRead);

                int startOffset = oldRawDataBufferSize >= 3 ? oldRawDataBufferSize - 3 : 0;
                int stopOffset = startOffset + bytesRead - 3;
                //loop over the newly received data looking for "\r\n\r\n" to tell use that we've received the entire HTTP header.
                for(int i = startOffset; i < stopOffset && !headerReceived; i++) {
                    headerReceived =
                        rawDataBuffer[i] == 13 &&
                        rawDataBuffer[i + 1] == 10 &&
                        rawDataBuffer[i + 2] == 13 &&
                        rawDataBuffer[i + 3] == 10;
                }
            }   //end of header loop.

            if (!headerReceived) {  //if we get here and haven't received a full header the something bad happened.
                throw new Exception("Unable to receive headers.");
            }

            string httpRequestData = Encoding.ASCII.GetString(rawDataBuffer);
            if (httpRequestData.StartsWith("POST")) {
                //at this point we have all of the HTTP headers, but we have possibly left some post data on the socket.
                int offset = httpRequestData.IndexOf("Content-Length:");
                offset += 15;
                string strLength = "";
                int i = 0;
                while (httpRequestData.Substring(offset + i, 2) != "\r\n") {
                    strLength += httpRequestData[offset + i].ToString();
                    i++;
                }
                strLength = strLength.Trim();
                int length = int.Parse(strLength);  //now we know how much post data we should have received...

                int offsetToContent = httpRequestData.IndexOf("\r\n\r\n") + 4;
                int bytesHave = httpRequestData.Length - offsetToContent;
                int bytesNeed = length - bytesHave; //now we know how much (if any) data is left on the socket

                if (bytesHave < length) {   //if we've left any we need to go and get it
                    byte[] buffer = new byte[bytesNeed];
                    try {
                        s.Read(buffer, 0, bytesNeed);
                        //TODO we should probably look at the return value from Receive to ensure we received
                        //the amount of data we we're expecting.
                    } catch (SocketException) {
                        throw; //oops!
                    }
                    //now that we've featched the remaining data, just append it to the existing http message.
                    httpRequestData += Encoding.ASCII.GetString(buffer, 0, bytesNeed);
                    Array.Resize(ref rawDataBuffer, rawDataBuffer.Length + bytesNeed);
                    Buffer.BlockCopy(buffer, 0, rawDataBuffer, rawDataBuffer.Length - bytesNeed, bytesNeed);

                }
                _postData = new byte[length];
                Buffer.BlockCopy(rawDataBuffer, offsetToContent, _postData, 0, length);
            }

            if (!ParseHttpHeaders(httpRequestData))
                throw new Exception("Error parsing request headers.");
        }

        private bool ParseHttpHeaders(string httpRequest) {
            string[] lines = httpRequest.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            _httpMethod = lines[0].Split(new char[] { ' ' })[0];
            string resource;
            try {
                resource = lines[0].Split(new char[] { ' ' })[1];
                resource = HttpUtility.UrlDecode(resource); //should we url decode this? I am beginning to think so.
            } catch {
                return false;
            }
            string host = "";
            for (int i = 1; i < lines.Length; i++) {
                if (lines[i].Length == 0)
                    break;  //end of headers.
                int split = lines[i].IndexOf(':');
                if (split >= 0) {
                    string header = lines[i].Substring(0, split);
                    string value = lines[i].Substring(split + 1).Trim();
                    switch (header) {
                        case "Accept":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderAccept] = value;
                            break;
                        case "Accept-Charset":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderAcceptCharset] = value;
                            break;
                        case "Accept-Encoding":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderAcceptEncoding] = value;
                            break;
                        case "Accept-Language":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderAcceptLanguage] = value;
                            break;
                        case "Allow":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderAllow] = value;
                            break;
                        case "Authorization":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderAuthorization] = value;
                            break;
                        case "Cache-Control":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderCacheControl] = value;
                            break;
                        case "Connection":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderConnection] = value;
                            break;
                        case "Content-Encoding":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderContentEncoding] = value;
                            break;
                        case "Content-Language":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderContentLanguage] = value;
                            break;
                        case "Content-Length":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderContentLength] = value;
                            break;
                        case "Content-Location":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderContentLocation] = value;
                            break;
                        case "Content-MD5":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderContentMd5] = value;
                            break;
                        case "Content-Range":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderContentRange] = value;
                            break;
                        case "Content-Type":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderContentType] = value;
                            break;
                        case "Date":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderDate] = value;
                            break;
                        case "ETag":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderEtag] = value;
                            break;
                        case "Expect":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderExpect] = value;
                            break;
                        case "Expires":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderExpires] = value;
                            break;
                        case "From":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderFrom] = value;
                            break;
                        case "Host":
                            host = value;
                            _knownRequestHeaders[HttpWorkerRequest.HeaderHost] = value;
                            break;
                        case "If-Match":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderIfMatch] = value;
                            break;
                        case "If-Modified-Since":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderIfModifiedSince] = value;
                            break;
                        case "If-None-Match":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderIfNoneMatch] = value;
                            break;
                        case "If-Range":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderIfRange] = value;
                            break;
                        case "If-Unmodified-Since":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderIfUnmodifiedSince] = value;
                            break;
                        case "Last-Modified":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderLastModified] = value;
                            break;
                        case "Location":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderLocation] = value;
                            break;
                        case "Max-Forwards":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderMaxForwards] = value;
                            break;
                        case "Pragma":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderPragma] = value;
                            break;
                        case "Proxy-Authenticate":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderProxyAuthenticate] = value;
                            break;
                        case "Proxy-Authorization":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderProxyAuthorization] = value;
                            break;
                        case "Referer":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderReferer] = value;
                            break;
                        case "Retry-After":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderRetryAfter] = value;
                            break;
                        case "Server":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderServer] = value;
                            break;
                        case "TE":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderTe] = value;
                            break;
                        case "Trailer":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderTrailer] = value;
                            break;
                        case "Transfer-Encoding":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderTransferEncoding] = value;
                            break;
                        case "Upgrade":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderUpgrade] = value;
                            break;
                        case "User-Agent":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderUserAgent] = value;
                            break;
                        case "Vary":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderVary] = value;
                            break;
                        case "Via":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderVia] = value;
                            break;
                        case "Warning":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderWarning] = value;
                            break;
                        case "WWW-Authenticate":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderWwwAuthenticate] = value;
                            break;
                        case "Keep-Alive":
                            _knownRequestHeaders[HttpWorkerRequest.HeaderKeepAlive] = value;
                            break;
                        default:
                            _unknownRequestHeaders.Add(header, value);
                            break;
                    }
                }
            }
            try {
                _url = new Uri("http://" + host + resource);
            } catch {
                return false;
            }
            _rawUrl = resource;
            if (_httpMethod == "POST") {

            } else {
                _postData = new byte[0];
            }
            return true;
        }

        protected void WriteLine(string data, List<byte> response) {
            response.AddRange(Encoding.ASCII.GetBytes(data + "\r\n"));
        }

        protected string GetHttpHeaderName(int index) {
            switch (index) {
                case HttpWorkerRequest.HeaderAcceptRanges:
                    return "Accept-Ranges";
                case HttpWorkerRequest.HeaderAge:
                    return "Age";
                case HttpWorkerRequest.HeaderAllow:
                    return "Allow";
                case HttpWorkerRequest.HeaderCacheControl:
                    return "Cache-Control";
                case HttpWorkerRequest.HeaderConnection:
                    return "Connection";
                case HttpWorkerRequest.HeaderContentEncoding:
                    return "Content-Encoding";
                case HttpWorkerRequest.HeaderContentLanguage:
                    return "Content-Language";
                case HttpWorkerRequest.HeaderContentLength:
                    return "Content-Length";
                case HttpWorkerRequest.HeaderContentLocation:
                    return "Content-Location";
                case HttpWorkerRequest.HeaderContentMd5:
                    return "Content-MD5";
                case HttpWorkerRequest.HeaderContentRange:
                    return "Content-Range";
                case HttpWorkerRequest.HeaderContentType:
                    return "Content-Type";
                case HttpWorkerRequest.HeaderDate:
                    return "Date";
                case HttpWorkerRequest.HeaderEtag:
                    return "ETag";
                case HttpWorkerRequest.HeaderExpires:
                    return "Expires";
                case HttpWorkerRequest.HeaderLastModified:
                    return "Last-Modified";
                case HttpWorkerRequest.HeaderLocation:
                    return "Location";
                case HttpWorkerRequest.HeaderProxyAuthenticate:
                    return "Proxy-Authenticate";
                case HttpWorkerRequest.HeaderRetryAfter:
                    return "Retry-After";
                case HttpWorkerRequest.HeaderServer:
                    return "Server";
                case HttpWorkerRequest.HeaderSetCookie:
                    return "Set-Cookie";
                case HttpWorkerRequest.HeaderTransferEncoding:
                    return "Transfer-Encoding";
                case HttpWorkerRequest.HeaderVary:
                    return "Vary";
                case HttpWorkerRequest.HeaderWwwAuthenticate:
                    return "WWW-Authenticate";
                case HttpWorkerRequest.ResponseHeaderMaximum:
                    return "Maximum";
            }
            return null;
        }

        protected bool IsSecure() {
            /*
             * This method assumes that the connection is secure if it
             * was received on port 443 (HTTPS port).
             * This feels "wrong", but at the same time I can't think
             * of a better way yet.
             */ 
            return _localEndPoint.Port == 443;
        }

        protected byte[] ToHttpResponse() {
            List<byte> response = new List<byte>();
            if (!_headersSent) {
                WriteLine("HTTP/1.1 " + _statusCode + " " + _statusDescription, response);
                for (int i = 0; i != _knownResponseHeaders.Length; i++) {
                    string data = _knownResponseHeaders[i];
                    if (!string.IsNullOrEmpty(data)) {
                        string header = GetHttpHeaderName(i);
                        if (header != null) {
                            WriteLine(string.Format("{0}: {1}", header, data), response);
                        }
                    }
                }
                foreach (string unknownHeader in _unknownResponseHeaders.Keys) {
                    WriteLine(unknownHeader + ": " + _unknownResponseHeaders[unknownHeader], response);
                }
                WriteLine("", response);
                _headersSent = true;
            }
            if (_rawData != null) {
                response.AddRange(_rawData);
                _rawData = null;
            }
            return response.ToArray();
        }
    }
}
