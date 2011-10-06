/************************************************************************
 * Copyright (c) 2006-2008, Jason Whitehorn (jason.whitehorn@gmail.com)
 * All rights reserved.
 * 
 * Source code and binaries distributed under the terms of the included
 * license, see license.txt for details.
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.IO;

namespace aspNETserve.Core {
    public sealed class AspNetWorker : HttpWorkerRequest, IAspNetWorker {

        public AspNetWorker(IAspNetRuntime aspNetWorker, string virtualDir, string physicalDir)  {
            _physicalDir = physicalDir;
            _virtualDir = virtualDir;
            _aspNetWorker = aspNetWorker;
            _serverVariables = new Dictionary<string, string>();
            _appPoolId = AppDomain.CurrentDomain.Id.ToString();
        }

        public AspNetWorker(IAspNetRuntime aspNetWorker, string virtualDir, string physicalDir, IDictionary<string, string> serverVariables) : this(aspNetWorker, virtualDir, physicalDir) {
            _serverVariables = serverVariables;
        }

        public void ProcessTransaction(ITransaction transaction) {
            Trace.TraceInformation("Entering AspNetWorker.ProcessTransaction");
            try {
                ParseRequest(transaction.Request);
                string page = _filePath;
                if (IsRestricted(page)) {
                    throw new NotImplementedException("not implemented yet.");
                        //TODO: Find a way to implement the below line
                    //return Response.Error403Forbidden;
                }
                _request = transaction.Request;
                _response = transaction.Response;
                _aspNetWorker.ProcessRequest(this);
                if (_callback != null)
                    _callback(this, _callbackPayload);
            }finally {
                Trace.TraceInformation("Leaving AspNetWorker.ProcessTransaction");
            }
        }

        public IDictionary<string, string> ServerVariables {
            get { return _serverVariables; }
            set { _serverVariables = value; }
        }

        #region HttpWorkerRequest methods
        
        public override string GetAppPath() {
            return _virtualDir;
        }

        public override string GetAppPathTranslated() {
            return _physicalDir;
        }

        public override string GetAppPoolID() {
            return _appPoolId;
        }

        public override long GetBytesRead() {
            return _request.PostData.Length;
        }

        public override long GetConnectionID() {
            return 0;   //this is suppose to always return 0.
        }

        public override string GetFilePath() {
            return _virtualFilePath;
        }

        public override string GetFilePathTranslated() {
            string result = _request.RawUrl;
            if (result.Contains("?"))
                result = result.Remove(result.IndexOf('?'));
            if (result.Contains("#"))
                result = result.Remove(result.IndexOf('#'));
            result = MapPath(result);
            return result;
        }

        public override string GetHttpVerbName() {
            return _request.HttpMethod;
        }

        public override string GetHttpVersion() {
            return _request.HttpVersion;
        }

        public override string GetKnownRequestHeader(int index) {
            return _request.GetKnownRequestHeader(index);
        }

        public override string GetLocalAddress() {
            return _request.LocalEndPoint.Address.ToString();
        }

        public override int GetLocalPort() {
            return _request.LocalEndPoint.Port;
        }

        public override string GetPathInfo() {
            return _pathInfo;
        }

        public override byte[] GetPreloadedEntityBody() {
            return _request.PostData;
        }

        public override string GetProtocol() {
            return IsSecure() ? "HTTPS" : "HTTP";
        }

        public override bool HeadersSent() {
            return _response.HeadersSent;
        }

        public override void FlushResponse(bool finalFlush) {
            _response.Flush();
        }

        public override string GetQueryString() {
            return GetQueryString(_request.RawUrl);
        }

        public override byte[] GetQueryStringRawBytes() {
            return Encoding.ASCII.GetBytes(GetQueryString());
        }

        public override string GetRawUrl() {
            return _request.RawUrl;
        }

        public override string GetRemoteAddress() {
            return _request.RemoteEndPoint.Address.ToString();
        }

        public override string GetRemoteName() {
            string result = _request.RemoteEndPoint.Address.ToString();
            //if (result == "127.0.0.1")
            //    result = "localhost";
            return result;
        }

        public override int GetRemotePort() {
            return _request.RemoteEndPoint.Port;
        }

        public override int GetRequestReason() {
            return HttpWorkerRequest.ReasonDefault;
        }

        public override string GetServerName() {
            string result = _request.LocalEndPoint.Address.ToString();
            if (result == "127.0.0.1")
                result = "localhost";
            return result;
        }

        public override string GetServerVariable(string name) {
            if (_serverVariables.ContainsKey(name))
                return _serverVariables[name];
            return "";
        }

        public override Guid RequestTraceIdentifier {
            get {
                return _request.RequestId;
            }
        }

        public override string GetUnknownRequestHeader(string name) {
            return _request.GetUnknownRequestHeader(name);
        }

        public override string GetUriPath() {
            return _virtualFilePath;
        }

        public override long GetUrlContextID() {
            return 0;   //this is suppose to always return 0.
        }

        public override IntPtr GetUserToken() {
            /*
             * 
             */
            //System.Security.Principal.WindowsIdentity f = System.Security.Principal.WindowsIdentity.GetCurrent();
            //return f.Token;
            return IntPtr.Zero;
        }

        public override bool IsClientConnected() {
            return true;    //<<HACK
        }

        public override bool IsSecure() {
            return _request.IsSecure && _response.IsSecure;
        }

        public override string MachineConfigPath {
            get { return string.Format("{0}CONFIG\\machine.config", HttpRuntime.AspInstallDirectory); }
        }

        public override string MachineInstallDirectory {
            get { return HttpRuntime.AspInstallDirectory; }
        }

        public override string MapPath(string path) {
            string result = _physicalDir + path.Remove(0, _virtualDir.Length);
            result = result.Replace("/", "\\");
            if (result.EndsWith("\\") && !result.EndsWith(":\\\\"))
                result = result.Substring(0, result.Length - 1);
            result = result.Replace("\\\\", "\\");
            return result;
        }

        public override string RootWebConfigPath {
            get { return string.Format("{0}CONFIG\\web.config", HttpRuntime.AspInstallDirectory); }
        }

        public override void SendCalculatedContentLength(int contentLength) {
            SendCalculatedContentLength((long)contentLength);
        }

        public override void SendCalculatedContentLength(long contentLength) {
            _response.SendKnownResponseHeader((int)HttpWorkerRequest.HeaderContentLength, contentLength.ToString());
        }

        public override void SetEndOfSendNotification(HttpWorkerRequest.EndOfSendNotification callback, object extraData) {
            _callback = callback;
            _callbackPayload = extraData;
        }

        public override void SendKnownResponseHeader(int index, string value) {
            _response.SendKnownResponseHeader(index, value);
        }

        public override void SendResponseFromMemory(byte[] data, int length) {
            int curLength = 0;
            if (_response.RawData != null)
                curLength += _response.RawData.Length;
            byte[] buffer = new byte[length + curLength];
            if (_response.RawData != null)
                Buffer.BlockCopy(_response.RawData, 0, buffer, 0, _response.RawData.Length);
            Buffer.BlockCopy(data, 0, buffer, curLength, length);
            _response.RawData = buffer;
        }

        public override void SendResponseFromMemory(IntPtr data, int length) {
            byte[] buffer = new byte[length];
            for (int i = 0; i < length; i++) {
                buffer[i] = System.Runtime.InteropServices.Marshal.ReadByte(data, i);
            }
            SendResponseFromMemory(buffer, length);
        }

        public override void SendResponseFromFile(string filename, long offset, long length) {
            /*
             * Opens a file given its filename, and writes the entire contents out
             * to the client.
             */ 
            using (FileStream file = File.OpenRead(filename)) {

                byte[] buffer = new byte[file.Length];
                file.Read(buffer, 0, (int)file.Length);
                SendResponseFromMemory(buffer, buffer.Length);

                file.Close();
            }
        }

        public override void SendStatus(int statusCode, string statusDescription) {
            _response.StatusCode = statusCode;
            _response.StatusDescription = statusDescription;
        }

        public override void SendUnknownResponseHeader(string name, string value) {
            _response.SendUnknownResponseHeader(name, value);
        }

        #region I DONT KNOW
        public override void CloseConnection() {
            //???
        }
        public override void EndOfRequest() {
            //???
        }
        public override byte[] GetClientCertificate() {
            return base.GetClientCertificate();
        }
        public override byte[] GetClientCertificateBinaryIssuer() {
            return base.GetClientCertificateBinaryIssuer();
        }
        public override int GetClientCertificateEncoding() {
            return base.GetClientCertificateEncoding();
        }
        public override byte[] GetClientCertificatePublicKey() {
            return base.GetClientCertificatePublicKey();
        }
        public override DateTime GetClientCertificateValidFrom() {
            return base.GetClientCertificateValidFrom();
        }
        public override DateTime GetClientCertificateValidUntil() {
            return base.GetClientCertificateValidUntil();
        }
        public override int GetPreloadedEntityBody(byte[] buffer, int offset) {
            return base.GetPreloadedEntityBody(buffer, offset);
        }
        public override int GetPreloadedEntityBodyLength() {
            return base.GetPreloadedEntityBodyLength();
        }
        public override int GetTotalEntityBodyLength() {
            return base.GetTotalEntityBodyLength();
        }
        public override string[][] GetUnknownRequestHeaders() {
            return base.GetUnknownRequestHeaders();
        }
        public override IntPtr GetVirtualPathToken() {
            return base.GetVirtualPathToken();
        }
        public override bool IsEntireEntityBodyIsPreloaded() {
            return base.IsEntireEntityBodyIsPreloaded();
        }
        public override int ReadEntityBody(byte[] buffer, int offset, int size) {
            return base.ReadEntityBody(buffer, offset, size);
        }
        public override int ReadEntityBody(byte[] buffer, int size) {
            return base.ReadEntityBody(buffer, size);
        }
        public override void SendResponseFromFile(IntPtr handle, long offset, long length) {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
        
        #endregion
    //-------------------------

        private bool IsRestricted(string fileName) {
            bool restricted = false;
            foreach (string dir in _secureDirs) {
                if (fileName.StartsWith(dir)) {
                    restricted = true;
                    break;
                }
            }
            return restricted;
        }

        private string GetQueryString(string rawUrl) {
            string result = "";
            if (rawUrl.Contains("?")) {
                result = rawUrl.Split(new char[] { '?' })[1];
            }
            return result;
        }

        private void ParseRequest(IRequest req){
            string rawUrl = req.RawUrl;
            int idxQuery = rawUrl.IndexOf('?');
            if (idxQuery != -1)
                _virtualFilePath = rawUrl.Substring(0, rawUrl.IndexOf('?'));
            else
                _virtualFilePath = rawUrl;

            int idxLastSlash = _virtualFilePath.LastIndexOf('/');
            int idxFirstDot = _virtualFilePath.IndexOf('.');    //this will not get the dots in the www.domain.com, because that is not part of the rawUrl
            if ((idxFirstDot != -1 && idxLastSlash != -1) && (idxFirstDot < idxLastSlash)) {
                //if the url has a dot (.) and a slash (/), and the dot occurs before the last slash
                //i.e., /site/something.aspx/foo
                int idxEndOfPreTail = _virtualFilePath.IndexOf('/', idxFirstDot);   //the first slash following the first dot.
                _pathInfo = _virtualFilePath.Substring(idxEndOfPreTail);    //save the tail
                _virtualFilePath = _virtualFilePath.Substring(0, idxEndOfPreTail);
            } else {
                _pathInfo = string.Empty;
            }

            if (_virtualFilePath[_virtualFilePath.Length-1] == '/') {
                foreach (string defaultPage in _defaultPages) {
                    string path = MapPath(_virtualFilePath + defaultPage);
                    if (File.Exists(path)) {
                        _virtualFilePath += defaultPage;
                        break;
                    }
                }
            }
            if (_virtualDir.Length > 1) {
                _filePath = _virtualFilePath.Substring(_virtualDir.Length);
                if (_filePath.StartsWith("/"))
                    _filePath = _filePath.Substring(1);
            } else {
                _filePath = _virtualFilePath;
            }
        }

        private string _virtualDir;
        private string _physicalDir;
        private static string[] _secureDirs = new string[] { 
                "/bin/",
                "/app_browsers/", 
                "/app_code/", 
                "/app_data/", 
                "/app_localresources/", 
                "/app_globalresources/", 
                "/app_webreferences/" 
        };
        private string[] _defaultPages = { "default.aspx", "index.html", "index.htm" };
        private IResponse _response;
        private IRequest _request;
        private HttpWorkerRequest.EndOfSendNotification _callback;
        private object _callbackPayload;
        private IDictionary<string, string> _serverVariables;
        private string _virtualFilePath;
        private string _filePath;
        private string _pathInfo;
        private IAspNetRuntime _aspNetWorker;
        private readonly string _appPoolId;
    }
}
