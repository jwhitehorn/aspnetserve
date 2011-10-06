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

namespace aspNETserve.Configuration {
    public class Domain : IDomain {
        private string _name;
        private string _virtualPath;

        public Domain(string name, string virtualPath) {
            _name = name;
            _virtualPath = virtualPath;
        }

        public string Name {
            get { return _name; }
        }

        public string VirtualPath {
            get { return _virtualPath; }
        }

    }
}
