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

namespace aspNETserve.Core {

    public class TransactionContainer : ITransaction {
        private IRequest _request;
        private IResponse _response;

        public TransactionContainer(IRequest request, IResponse response) {
            _request = request;
            _response = response;
        }

        public virtual IRequest Request {
            get { return _request; }
        }

        public virtual IResponse Response {
            get { return _response; }
        }
    }
}
