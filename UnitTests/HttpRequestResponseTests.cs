/************************************************************************
 * Copyright (c) 2006-2008, Jason Whitehorn (jason.whitehorn@gmail.com)
 * All rights reserved.
 * 
 * Source code and binaries distributed under the terms of the included
 * license, see license.txt for details.
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using aspNETserve;
using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTests {
    [TestFixture]
    public class HttpRequestResponseTests {

        [Test]
        public void Simple_Get_Request_Test() {
            MemoryStream stream = GetRequestStream("GET / HTTP/1.1", "Host: Localhost", "");
            HttpRequestResponse httpRequestResponse = new HttpRequestResponse(stream, null, null);

            Assert.AreEqual(httpRequestResponse.Request.HttpMethod, "GET");
            Assert.AreEqual(httpRequestResponse.Request.RawUrl, "/");
            Assert.IsFalse(httpRequestResponse.Request.IsKeepAlive);
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Error parsing request headers.")]
        public void Simple_Get_Request_Lacking_Host_Header_Test() {
            //This test should throw an exception as the Host header is required.
            //Ideally we would be able to better test for this, but for now all we
            //can do is test for the above generic exception.
            MemoryStream stream = GetRequestStream("GET / HTTP/1.1", "");
            HttpRequestResponse httpRequestResponse = new HttpRequestResponse(stream, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "The supplied stream does not support read timeouts. Please supply an alternative stream or use a different constructor.")]
        public void HttpRequestResponse_Construction_With_Non_Timeout_Stream_When_Specifying_Timeout_Test() {
            MockRepository mocks = new MockRepository();

            Stream s = mocks.CreateMock<Stream>();
            using(mocks.Unordered()) {
                Expect.Call(s.CanTimeout).Return(false);
            }
            mocks.ReplayAll();

            HttpRequestResponse httpRequestResponse = new HttpRequestResponse(s, null, null, 400);
        }

        [Test]
        public void HttpRequestResponse_Construction_With_Non_Timeout_Stream_Test() {
            Stream s = GetRequestStream("GET / HTTP/1.1", "Host: Localhost", "");
            Assert.IsFalse(s.CanTimeout);

            HttpRequestResponse httpRequestResponse = new HttpRequestResponse(s, null, null);
        }

        [Test]
        [Description("A test to determine if a URL contained a URL encoded path is properly decode by the runtime.")]
        public void Url_Encoded_Urls_Test() {
            MemoryStream request = GetRequestStream("GET /virDir/A%20Path%20That%20Needs%20Encoding.aspx HTTP/1.1",
                                                    "Host: Localhost", "");
            HttpRequestResponse httpRequestResponse = new HttpRequestResponse(request, null, null);
            Assert.AreEqual("/virDir/A Path That Needs Encoding.aspx", httpRequestResponse.Request.RawUrl);
        }

        [Test]
        public void IsKeepAlive_True_Test() {
            MemoryStream stream = GetRequestStream("GET / HTTP/1.1", "Host: Localhost", "Connection: Keep-Alive", "");
            HttpRequestResponse httpRequestResponse = new HttpRequestResponse(stream, null, null);

            Assert.IsTrue(httpRequestResponse.Request.IsKeepAlive);
        }

        [Test]
        public void IsKeepAlive_False_Test() {
            MemoryStream stream = GetRequestStream("GET / HTTP/1.1", "Host: Localhost", "");
            HttpRequestResponse httpRequestResponse = new HttpRequestResponse(stream, null, null);

            Assert.IsFalse(httpRequestResponse.Request.IsKeepAlive);
        }

        [Test]
        public void PostData_On_Get_Test() {
            MemoryStream stream = GetRequestStream("GET / HTTP/1.1", "Host: Localhost", "");
            HttpRequestResponse httpRequestResponse = new HttpRequestResponse(stream, null, null);

            Assert.AreEqual(0, httpRequestResponse.Request.PostData.Length);
        }

        [Test]
        public void Empty_PostData_Test() {
            MemoryStream stream = GetRequestStream("POST / HTTP/1.1", "Host: Localhost", "Content-Length: 0", "");
            HttpRequestResponse httpRequestResponse = new HttpRequestResponse(stream, null, null);

            Assert.AreEqual(0, httpRequestResponse.Request.PostData.Length);
        }

        [Test]
        public void NonEmpty_PostData_Test() {
            byte[] postedData = new byte[]{4, 9, 123, 87, 182, 44, 250};

            MemoryStream stream = GetRequestStream("POST / HTTP/1.1", "Host: Localhost", "Content-Length: 7", "", Encoding.ASCII.GetString(postedData));
            HttpRequestResponse httpRequestResponse = new HttpRequestResponse(stream, null, null);

            Assert.AreEqual(postedData, httpRequestResponse.Request.PostData);
        }

        private MemoryStream GetRequestStream(params string[] lines) {
            MemoryStream result = new MemoryStream();
            foreach(string line in lines) {
                byte[] rawData = Encoding.ASCII.GetBytes(string.Format("{0}\r\n", line));
                result.Write(rawData, 0, rawData.Length);
            }
            result.Position = 0;
            return result;
        }
    }
}
