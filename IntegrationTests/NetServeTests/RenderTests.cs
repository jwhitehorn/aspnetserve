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
using aspNETserve;
using NUnit.Framework;

namespace IntegrationTests.NetServeTests {
    [TestFixture]
    public class RenderTests : BaseTest {
        [Test]
        public void Default_aspxRenderTest() {
            Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/", PhysicalPath, _port);
            s.Start();
            try {
                string data = GetPage("http://localhost:" + _port + "/default.aspx");
                Console.WriteLine("received HTML:");
                Console.WriteLine(data);
                Assert.AreEqual(true, data.Contains(_defaultASPX));
            } finally {
                s.Stop();
            }
        }
        [Test]
        public void SimplePostTest_aspxRenderTest() {
            Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/", PhysicalPath, _port);
            s.Start();
            try {
                string data = GetPage("http://localhost:" +  _port + "/SimplePostTest.aspx");
                Console.WriteLine(data);
                Assert.AreEqual(true, data.Contains(_doPostbackJS));
            } finally {
                s.Stop();
            }
        }
        //-----------------------------------------------------------------------------------
        #region Expected HTML
        private const string _defaultASPX = @"<a id=""HyperLink1"" href=""PostTest.aspx"">Postback Test</a></div>
        <a id=""HyperLink2"" href=""RequestDump.aspx"">Request Dump</a>";
        private const string _doPostbackJS = @"<script type=""text/javascript"">
<!--
var theForm = document.forms['form1'];
if (!theForm) {
    theForm = document.form1;
}
function __doPostBack(eventTarget, eventArgument) {
    if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
        theForm.__EVENTTARGET.value = eventTarget;
        theForm.__EVENTARGUMENT.value = eventArgument;
        theForm.submit();
    }
}
// -->
</script>";
        #endregion
    }
}
