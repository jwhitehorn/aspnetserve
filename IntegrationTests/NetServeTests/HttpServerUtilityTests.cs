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

namespace IntegrationTests.NetServeTests {
    [TestFixture]
    public class HttpServerUtilityTests : BaseTest {
        [Test]
        public void MachineNameTest() {
            string value = GetServerVariable("Context.Server.MachineName");
            Assert.AreEqual("SID", value);
        }
        [Test]
        public void ScriptTimeoutTest() {
            string value = GetServerVariable("Context.Server.ScriptTimeout");
            Assert.AreEqual("30000000", value);
        }
    }
}
