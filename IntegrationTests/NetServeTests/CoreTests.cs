/************************************************************************
 * Copyright (c) 2006-2008, Jason Whitehorn (jason.whitehorn@gmail.com)
 * All rights reserved.
 * 
 * Source code and binaries distributed under the terms of the included
 * license, see license.txt for details.
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.IO;
using NUnit.Framework;
using aspNETserve;

namespace IntegrationTests.NetServeTests {
    [TestFixture]
    public class CoreTests : BaseTest {
        [Test]
        [Category("/")]
        public void CycleTest() {
            Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/", PhysicalPath, _port);
            s.Start();
            s.Stop();
        }
        [Test]
        [Category("virtual directory")]
        public void VirtualDirectoryCycleTest() {
            Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/vDir", PhysicalPath, _port);
            s.Start();
            s.Stop();
        }
        [Test]
        [Category("/")]
        public void SimpleGETTest() {
            Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/", PhysicalPath, _port);
            s.Start();
            WebClient helper = new WebClient();
            string data;
            try {
                data = helper.DownloadString("http://localhost:" + _port + "/default.aspx");
            } catch {
                s.Stop();
                throw;
            }
            Assert.AreEqual(false, string.IsNullOrEmpty(data));
            s.Stop();
        }
        [Test]
        [Category("virtual directory")]
        public void VirtualDirectorySimpleGETTest() {
            Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/vDir", PhysicalPath, _port);
            s.Start();
            WebClient helper = new WebClient();
            string data;
            try {
                data = helper.DownloadString("http://localhost:" + _port + "/vDir/default.aspx");
            } catch {
                s.Stop();
                throw;
            }
            Assert.AreEqual(false, string.IsNullOrEmpty(data));
            s.Stop();
        }
        [Test]
        [Category("/")]
        public void ResponseRedirectTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/", PhysicalPath, _port)) {
                s.Start();
                string expectedPage = GetPage("http://localhost:" + _port + "/RedirectHome.aspx");
                Assert.AreNotEqual(null, expectedPage);
                HttpWebResponse response = GetPageResponse("http://localhost:" + _port + "/RedirectHome.aspx") as HttpWebResponse;
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Stream stream = response.GetResponseStream();
                byte[] buffer = new byte[response.ContentLength];
                stream.Read(buffer, 0, (int)response.ContentLength);
                string actualPage = Encoding.UTF8.GetString(buffer);
                Assert.AreEqual(expectedPage, actualPage);
                s.Stop();
            }
        }
        [Test]
        [Category("virtual directory")]
        public void VirtualDirectoryResponseRedirectTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/vDir", PhysicalPath, _port)) {
                s.Start();
                string expectedPage = GetPage("http://localhost:" + _port + "/vDir/RedirectHome.aspx");
                Assert.AreNotEqual(null, expectedPage);
                HttpWebResponse response = GetPageResponse("http://localhost:" + _port + "/vDir/RedirectHome.aspx") as HttpWebResponse;
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Stream stream = response.GetResponseStream();
                byte[] buffer = new byte[response.ContentLength];
                stream.Read(buffer, 0, (int)response.ContentLength);
                string actualPage = Encoding.UTF8.GetString(buffer);
                Assert.AreEqual(expectedPage, actualPage);
                s.Stop();
            }
        }
        [Test]
        [Category("/")]
        public void SimplePostTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/", PhysicalPath, _port)) {
                s.Start();
                string url = string.Format("http://localhost:{0}/PostTest.aspx", _port);
                string guid = Guid.NewGuid().ToString();

                NameValueCollection values = new NameValueCollection();                
                values.Add("txtName", guid);

                string page = PostValues(url, values);

                Console.WriteLine(guid);
                Console.WriteLine(page);
                Assert.AreEqual(true, page.Contains(guid));

                s.Stop();
            }
        }
        [Test]
        [Category("virtual directory")]
        public void VirtualDirectorySimplePostTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/vDir", PhysicalPath, _port)) {
                s.Start();
                string url = string.Format("http://localhost:{0}/vDir/PostTest.aspx", _port);
                string guid = Guid.NewGuid().ToString();

                NameValueCollection values = new NameValueCollection();
                values.Add("txtName", guid);

                string page = PostValues(url, values);

                Console.WriteLine(guid);
                Console.WriteLine(page);
                Assert.AreEqual(true, page.Contains(guid));

                s.Stop();
            }
        }
        [Test]
        [Category("/")]
        public void SubDirectoryRequestTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/", PhysicalPath, _port)) {
                s.Start();
                string page = GetPage(string.Format("http://localhost:{0}/SubFolder/", _port));
                Assert.AreEqual(true, page.Contains("Hello World"));
                s.Stop();
            }
        }
        [Test]
        [Category("virtual directory")]
        public void VirtualDirectorySubDirectoryRequestTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/vDir", PhysicalPath, _port)) {
                s.Start();
                string page = GetPage(string.Format("http://localhost:{0}/vDir/SubFolder/", _port));
                Assert.AreEqual(true, page.Contains("Hello World"));
                s.Stop();
            }
        }
        [Test]
        [Category("/")]
        public void RootDirectoryRequestTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/", PhysicalPath, _port)) {
                s.Start();
                string data = GetPage(string.Format("http://localhost:{0}/", _port));
                Assert.AreEqual(false, string.IsNullOrEmpty(data));
                s.Stop();
            }
        }
        [Test]
        [Category("virtual directory")]
        public void VirtualDirectoryRootDirectoryRequestTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/vDir", PhysicalPath, _port)) {
                s.Start();
                string data = GetPage(string.Format("http://localhost:{0}/vDir/", _port));
                Assert.AreEqual(false, string.IsNullOrEmpty(data));
                s.Stop();
            }
        }
    }
}
