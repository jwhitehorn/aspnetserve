/************************************************************************
 * Copyright (c) 2006-2008, Jason Whitehorn (jason.whitehorn@gmail.com)
 * All rights reserved.
 * 
 * Source code and binaries distributed under the terms of the included
 * license, see license.txt for details.
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using aspNETserve;

namespace IntegrationTests.NetServeTests {
    /// <summary>
    /// battery of test to verify that the HttpRequest object
    /// of the running servers Context is behaving correctly.
    /// </summary>
    [TestFixture]
    public class HttpRequestObjectTests : BaseTest{
        [Test]
        [Ignore]
        public void AcceptTypesTest() {
            throw new NotImplementedException();
        }
        [Test]
        [Ignore]
        public void AnonymousIDTest() {
            throw new NotImplementedException();
        }
        [Test]
        public void ApplicationPathTest() {
            string value = GetServerVariable("Context.Request.ApplicationPath");
            Assert.AreEqual("/", value);
        }
        [Test]
        public void ApplicationPathVirtualPathTest(){
            string value = GetServerVariable("Context.Request.ApplicationPath", "/vDir");
            Assert.AreEqual("/vDir", value);
        }
        [Test]
        public void AppRelativeCurrentExecutionFilePathTest() {
            string value = GetServerVariable("Context.Request.AppRelativeCurrentExecutionFilePath");
            Assert.AreEqual("~/GetServerVariable.aspx", value);
        }
        [Test]
        [Ignore]
        public void BrowserTest() {
            throw new NotImplementedException();
        }
        [Test]
        [Ignore]
        public void ClientCertificateTest() {
            throw new NotImplementedException();
        }
        [Test]
        public void ContentEncodingTest() {
            string value = GetServerVariable("Context.Request.ContentEncoding");
            Assert.AreEqual("System.Text.UTF8Encoding", value);
        }
        [Test]
        public void ContentLengthTest() {
            string value = GetServerVariable("Context.Request.ContentLength");
            Assert.AreEqual("0", value);
        }
        [Test]
        [Ignore]
        public void ContentTypeTest() {
            throw new NotImplementedException();
        }
        [Test]
        [Ignore]
        public void CookiesTest() {
            throw new NotImplementedException();
        }
        [Test]
        public void CurrentExecutionFilePathTest() {
            string value = GetServerVariable("Context.Request.CurrentExecutionFilePath");
            Assert.AreEqual("/GetServerVariable.aspx", value);
        }
        [Test]
        public void FilePathTest() {
            string value = GetServerVariable("Context.Request.FilePath");
            Assert.AreEqual("/GetServerVariable.aspx", value);
        }
        [Test]
        [Ignore]
        public void FilesTest() {
            throw new NotImplementedException();
        }
        [Test]
        [Ignore]
        public void FilterTest() {
            throw new NotImplementedException();
        }
        [Test]
        [Ignore]
        public void FormTest() {
            throw new NotImplementedException();
        }
        [Test]
        [Ignore]
        public void HeadersTest() {
            throw new NotImplementedException();
        }
        [Test]
        public void HttpMethodTest() {
            string value = GetServerVariable("Context.Request.HttpMethod");
            Assert.AreEqual("GET", value);
        }
        [Test]
        [Ignore]
        public void InputStreamTest() {
            throw new NotImplementedException();
        }
        [Test]
        public void IsAuthenticatedTest() {
            string value = GetServerVariable("Context.Request.IsAuthenticated");
            Assert.AreEqual("False", value);
        }
        [Test]
        public void IsLocalTest() {
            string value = GetServerVariable("Context.Request.IsLocal");
            Assert.AreEqual("True", value);
        }
        [Test]
        public void IsSecureConnectionTest() {
            string value = GetServerVariable("Context.Request.IsSecureConnection");
            Assert.AreEqual("False", value);
        }
        [Test]
        [Ignore]
        public void ItemTest() {
            throw new NotImplementedException();
        }
        [Test]
        [Ignore]
        public void LogonUserIdentityTest() {
            throw new NotImplementedException();
        }
        [Test]
        [Ignore]
        public void ParamsTest() {
            throw new NotImplementedException();
        }
        [Test]
        public void PathTest() {
            string value = GetServerVariable("Context.Request.Path");
            Assert.AreEqual("/GetServerVariable.aspx", value);
        }
        [Test]
        public void PathInfoTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/", PhysicalPath, _port)) {
                s.Start();
                string url = string.Format("http://localhost:{0}/Tail.aspx", _port);
                Console.WriteLine("Getting: " + url);
                string page = GetPage(url);
                Console.WriteLine(page);
                Assert.AreEqual(true, page.Contains("<span id=\"lblTail\"></span>"));

                url = string.Format("http://localhost:{0}/Tail.aspx/foo", _port);
                Console.WriteLine("Getting: " + url);
                page = GetPage(url);
                Console.WriteLine(page);
                Assert.AreEqual(true, page.Contains("<span id=\"lblTail\">/foo</span>"));
                s.Stop();
            }
        }
        [Test]
        public void PhysicalApplicationPathTest() {
            string value = GetServerVariable("Context.Request.PhysicalApplicationPath");
            Assert.AreEqual(PhysicalPath, value);
        }
        [Test]
        public void PhysicalPathTest() {
            string value = GetServerVariable("Context.Request.PhysicalPath");
            Assert.AreEqual(PhysicalPath + "GetServerVariable.aspx", value);
        }
        [Test]
        public void QueryStringTest() {
            string value = GetServerVariable("Context.Request.QueryString");
            Assert.AreEqual("var=Context.Request.QueryString", value);
        }
        [Test]
        public void RawUrlTest() {
            string value = GetServerVariable("Context.Request.RawUrl");
            Assert.AreEqual(@"/GetServerVariable.aspx?var=Context.Request.RawUrl", value);
        }
        [Test]
        public void RequestTypeTest() {
            string value = GetServerVariable("Context.Request.RequestType");
            Assert.AreEqual("GET", value);
        }
        [Test]
        [Ignore]
        public void ServerVariablesTest() {
            throw new NotImplementedException();
        }
        [Test]
        public void TotalBytesTest() {
            string value = GetServerVariable("Context.Request.TotalBytes");
            Assert.AreEqual("0", value);
        }
        [Test]
        public void UrlTest() {
            string value = GetServerVariable("Context.Request.Url");
            Assert.AreEqual("http://localhost:" + _port + "/GetServerVariable.aspx?var=Context.Request.Url", value);
        }
        [Test]
        [Ignore]
        public void UrlReferrerTest() {
            throw new NotImplementedException();
        }
        [Test]
        public void UserAgentTest() {
            string value = GetServerVariable("Context.Request.UserAgent");
            Assert.AreEqual(_userAgent, value);
        }
        [Test]
        public void UserHostAddressTest() {
            string value = GetServerVariable("Context.Request.UserHostAddress");
            Assert.AreEqual("127.0.0.1", value);
        }
        [Test]
        public void UserHostNameTest() {
            string value = GetServerVariable("Context.Request.UserHostName");
            Assert.AreEqual("127.0.0.1", value);
        }
        [Test]
        [Ignore]
        public void UserLanguagesTest() {
            throw new NotImplementedException();
        }
    }
}
