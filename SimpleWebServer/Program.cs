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
using System.Windows.Forms;
using aspNETserve;

namespace SimpleWebServer {
    internal static class Program {
        [STAThread]
        public static int Main(String[] args) {
            //log4net.Config.XmlConfigurator.Configure(System.IO.File.OpenRead("log4net_config.xml"));
            Application.Run(new frm_Main(args));
            if (Server != null)
                Server.Dispose();
            return 0;
        }
        internal static IAsyncServer Server {
            get { return _server; }
            set { _server = value; }
        }
        private static IAsyncServer _server;
    }
}
