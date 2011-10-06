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
    public class Configuration : IConfiguration {
        private IList<IApplication> _applications;

        public Configuration(IList<IApplication> applications) {
            _applications = applications;
        }

        public IList<IApplication> Applications {
            get { return _applications; }
        }
    }
}
