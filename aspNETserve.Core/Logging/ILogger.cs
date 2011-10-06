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
    public interface ILogger {
        void LogException(LogLevel level, string message, Exception ex);
        void LogException(LogLevel level, Exception ex);
        void LogMessage(LogLevel level, string message);
        void LogMemberEntry();
        void LogMemberExit();
        event LogExceptionEventHandler LogExceptionEvent;
        event LogMessageEventHandler LogMessageEvent;
    }
}
