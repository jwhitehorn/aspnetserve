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
    public class Application : IApplication {
        private string _physicalPath;
        private IList<IDomain> _domains;
        private IList<IEndPoint> _endPoints;

        public Application(string physicalPath, IList<IDomain> domains, IList<IEndPoint> endPoints) {
            _physicalPath = physicalPath;
            _domains = domains;
            _endPoints = endPoints;
        }

        public string PhysicalPath {
            get { return _physicalPath; }
        }

        public IList<IDomain> Domains {
            get { return _domains; }
        }

        public IList<IEndPoint> EndPoints {
            get { return _endPoints; }
        }

        
    }
}
