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
using aspNETserve.Core;
using NUnit.Framework;
using System.Web.Hosting;
using System.Web;

namespace IntegrationTests.NetServeTests {
    [TestFixture]
    public class HttpWorkerTests {
        [Test]
        public void MachineConfigPathTest() {
            AspNetWorker worker = GetHttpWorker("/webapp", "c:\\webapp\\");
            Assert.AreEqual(string.Format("{0}CONFIG\\machine.config", HttpRuntime.AspInstallDirectory), worker.MachineConfigPath);
        }
        [Test]
        public void MachineInstallDirectoryTest() {
            AspNetWorker worker = GetHttpWorker("/webapp", "c:\\webapp\\");
            Assert.AreEqual(HttpRuntime.AspInstallDirectory, worker.MachineInstallDirectory);
        }
        [Test]
        [Ignore]
        public void RequestTraceIdentifierTest() {
            /*
             * This method is no longer valid... this should possibly be removed soon.
             */ 
            SimpleWorkerRequest reference = new SimpleWorkerRequest("/webapp", "c:\\webapp\\", "default.aspx", "", null);

            AspNetWorker worker = GetHttpWorker("/webapp", "c:\\webapp\\");
            Assert.AreEqual(reference.RequestTraceIdentifier, worker.RequestTraceIdentifier);
        }
        [Test]
        public void RootWebConfigPathTest() {
            AspNetWorker worker = GetHttpWorker("/webapp", "c:\\webapp\\");
            Assert.AreEqual(string.Format("{0}CONFIG\\web.config", HttpRuntime.AspInstallDirectory), worker.RootWebConfigPath);
        }
        [Test]
        public void GetAppPathTest() {
            SimpleWorkerRequest reference = new SimpleWorkerRequest("/webapp", "c:\\webapp\\", "default.aspx", "", null);

            AspNetWorker worker = GetHttpWorker("/webapp", "c:\\webapp\\");
            Assert.AreEqual(reference.GetAppPath(), worker.GetAppPath());
        }
        [Test]
        public void GetAppPathTranslatedTest() {
            SimpleWorkerRequest reference = new SimpleWorkerRequest("/webapp", "c:\\webapp\\", "default.aspx", "", null);

            AspNetWorker worker = GetHttpWorker("/webapp", "c:\\webapp\\");
            Assert.AreEqual(reference.GetAppPathTranslated(), worker.GetAppPathTranslated());
        }
        [Ignore]
        [Test]
        public void GetAppPoolIDTest() {
            /*
             * This test was ignored because I fail to find it useful at the moment.
             */ 
            SimpleWorkerRequest reference = new SimpleWorkerRequest("/webapp", "c:\\webapp\\", "default.aspx", "", null);

            AspNetWorker worker = GetHttpWorker("/webapp", "c:\\webapp\\");
            Assert.AreEqual(reference.GetAppPoolID(), worker.GetAppPoolID());
        }
        //GetBytesRead 
        //GetClientCertificate 
        //GetClientCertificateBinaryIssuer 
        //GetClientCertificateEncoding 
        //GetClientCertificatePublicKey 
        //GetClientCertificateValidFrom 
        //GetClientCertificateValidUntil 
        //GetConnectionID 
        [Test]
        public void GetFilePathTest() {
            SimpleWorkerRequest reference = new SimpleWorkerRequest("/webapp", "c:\\webapp\\", "default.aspx", "", null);

            AspNetWorker worker = GetHttpWorker("/webapp", "c:\\webapp\\");
            Assert.AreEqual(reference.GetFilePath(), worker.GetFilePath());
        }
        [Test]
        public void GetFilePathTranslatedTest() {
            SimpleWorkerRequest reference = new SimpleWorkerRequest("/webapp", "c:\\webapp\\", "default.aspx", "", null);

            AspNetWorker worker = GetHttpWorker("/webapp", "c:\\webapp\\");
            Assert.AreEqual(reference.GetFilePathTranslated(), worker.GetFilePathTranslated());
        }
        [Test]
        public void GetHttpVerbNameTest() {
            SimpleWorkerRequest reference = new SimpleWorkerRequest("/webapp", "c:\\webapp\\", "default.aspx", "", null);

            AspNetWorker worker = GetHttpWorker("/webapp", "c:\\webapp\\");
            Assert.AreEqual(reference.GetHttpVerbName(), worker.GetHttpVerbName());
        }
        //GetHttpVersion
        //GetKnownRequestHeader 
        //GetKnownRequestHeaderIndex 
        //GetKnownRequestHeaderName 
        //GetKnownResponseHeaderIndex 
        //GetKnownResponseHeaderName 
        [Test]
        public void GetLocalAddressTest() {
            SimpleWorkerRequest reference = new SimpleWorkerRequest("/webapp", "c:\\webapp\\", "default.aspx", "", null);

            AspNetWorker worker = GetHttpWorker("/webapp", "c:\\webapp\\");
            Assert.AreEqual(reference.GetLocalAddress(), worker.GetLocalAddress());
        }
        //GetLocalPort 
        [Test]
        public void GetPathInfoTest() {
            SimpleWorkerRequest reference = new SimpleWorkerRequest("/webapp", "c:\\webapp\\", "default.aspx", "", null);

            AspNetWorker worker = GetHttpWorker("/webapp", "c:\\webapp\\");
            Assert.AreEqual(reference.GetPathInfo(), worker.GetPathInfo());
        }
        //GetPreloadedEntityBody 
        //GetPreloadedEntityBodyLength 
        //GetProtocol 
        //GetQueryString 
        //GetQueryStringRawBytes 
        [Test]
        public void GetRawUrlTest() {
            SimpleWorkerRequest reference = new SimpleWorkerRequest("/webapp", "c:\\webapp\\", "default.aspx", "", null);

            AspNetWorker worker = GetHttpWorker("/webapp", "c:\\webapp\\");
            Assert.AreEqual(reference.GetRawUrl(), worker.GetRawUrl());
        }
        //GetRemoteAddress 
        [Test]
        public void GetRemoteNameTest() {
            SimpleWorkerRequest reference = new SimpleWorkerRequest("/webapp", "c:\\webapp\\", "default.aspx", "", null);

            AspNetWorker worker = GetHttpWorker("/webapp", "c:\\webapp\\");
            Assert.AreEqual(reference.GetRemoteName(), worker.GetRemoteName());
        }
        //GetRemotePort 
        //GetRequestReason 
        [Test]
        public void GetServerNameTest() {
            SimpleWorkerRequest reference = new SimpleWorkerRequest("/webapp", "c:\\webapp\\", "default.aspx", "", null);

            AspNetWorker worker = GetHttpWorker("/webapp", "c:\\webapp\\");
            Assert.AreEqual(reference.GetServerName(), worker.GetServerName());
        }
        //GetServerVariable 
        //GetStatusDescription 
        //GetTotalEntityBodyLength 
        //GetUnknownRequestHeader 
        //GetUnknownRequestHeaders 
        [Test]
        public void GetUriPathTest() {
            SimpleWorkerRequest reference = new SimpleWorkerRequest("/webapp", "c:\\webapp\\", "default.aspx", "", null);

            AspNetWorker worker = GetHttpWorker("/webapp", "c:\\webapp\\");
            Assert.AreEqual(reference.GetUriPath(), worker.GetUriPath());
        }
        //GetUrlContextID 
        //GetUserToken 
        //GetVirtualPathToken 
    //----------------------------------------------------
        private AspNetWorker GetHttpWorker(string virtualDir, string physicalDir) {
            throw new Exception();
            //Request request = new Request();
            //request.HttpMethod = "GET";
            //request.LocalEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 80);
            //request.PostData = new byte[0];
            //request.QueryString = null;
            //if (virtualDir.Length > 1)
            //    request.RawUrl = virtualDir + "/default.aspx";
            //else
            //    request.RawUrl = "/default.aspx";
            //request.RemoteEndPoint = request.LocalEndPoint;
            //request.Url = new Uri("http://localhost" + request.RawUrl);

            //HttpWorker worker = new HttpWorker(virtualDir, physicalDir);
            //worker.Process(request);
            //return worker;
        }
    }
}
