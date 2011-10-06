/************************************************************************
 * Copyright (c) 2006-2008, Jason Whitehorn (jason.whitehorn@gmail.com)
 * All rights reserved.
 * 
 * Source code and binaries distributed under the terms of the included
 * license, see license.txt for details.
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace aspNETserve.Configuration {
    public class EndPoint : IEndPoint {
        private IPAddress _ip;
        private int _port;
        private bool _isSecure;

        public EndPoint(IPAddress ip, int port, bool isSecure) {
            _ip = ip;
            _port = port;
            _isSecure = isSecure;
        }

        public IPAddress Ip {
            get { return _ip; }
        }

        public int Port {
            get { return _port; }
        }

        public bool IsSecure {
            get { return _isSecure; }
        }
    }
}
