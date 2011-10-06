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
using System.Net;
using System.IO;
using NUnit.Framework;
using aspNETserve;
using IntegrationTests.NetServeTests.Auxillary.WebServices;

namespace IntegrationTests.NetServeTests {
    [TestFixture]
    public class WebServicesTest : BaseTest {
        [Test]
        public void SimpleWSCallTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/", PhysicalPath, _port)) {
                s.Start();
                using (WebService ws = new WebService()) {
                    ws.Url = string.Format("http://localhost:{0}/WebService.asmx", _port);
                    string response = ws.HelloWorld();
                    Assert.AreEqual("Hello World", response);
                }
                s.Stop();
            }
        }

        [Test]
        public void PrimitiveTypeWSCallTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/", PhysicalPath, _port)) {
                s.Start();
                using (WebService ws = new WebService()) {
                    ws.Url = string.Format("http://localhost:{0}/WebService.asmx", _port);
                    int result = ws.DoAdd(4, 7);
                    Assert.AreEqual(11, result);
                }
                s.Stop();
            }
        }
    }
}
