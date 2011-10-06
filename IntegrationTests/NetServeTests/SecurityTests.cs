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
using aspNETserve;
using NUnit.Framework;
using System.Web.Hosting;
using System.Web;
using System.Net;
using System.Security.Principal;

namespace IntegrationTests.NetServeTests {
    [TestFixture]
    public class SecurityTests : BaseTest {
        [Test]
        [Category("/")]
        public void ContextIdentityTest() {
            string identity = GetServerVariable("Context.User.Identity.Name");
            WindowsIdentity processIdentity = WindowsIdentity.GetCurrent();
            Assert.AreEqual(processIdentity.Name, identity);
        }
    }
}
