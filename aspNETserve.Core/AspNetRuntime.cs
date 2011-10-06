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
using System.Web;

namespace aspNETserve.Core {
    public class AspNetRuntime : IAspNetRuntime {

        public void ProcessRequest(IAspNetWorker aspNetWorkerRequest) {
            //TODO: Find a way to prevent the IAspNetWorkerRequest implementation
            //from also having to be a HttpWorkerRequest. Perhaps some type of a 
            //proxy object.
            HttpRuntime.ProcessRequest((HttpWorkerRequest) aspNetWorkerRequest);
        }
    }
}
