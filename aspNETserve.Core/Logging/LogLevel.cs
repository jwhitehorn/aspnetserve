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

namespace aspNETserve.Core.Logging {
    public enum LogLevel {
        Debug = 0,
        Info = 50,
        Warning = 100,
        Error = 150,
        Fatal = 200
    }
}
