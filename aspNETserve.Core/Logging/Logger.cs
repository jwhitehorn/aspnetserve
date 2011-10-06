/************************************************************************
 * Copyright (c) 2006-2008, Jason Whitehorn (jason.whitehorn@gmail.com)
 * All rights reserved.
 * 
 * Source code and binaries distributed under the terms of the included
 * license, see license.txt for details.
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace aspNETserve.Core.Logging {

    public class Logger : MarshalByRefObject, ILogger {

        private Logger(){}

        private event LogExceptionEventHandler _logExceptionEvent;
        private event LogMessageEventHandler _logMessageEvent;

        public void LogException(LogLevel level, string message, Exception ex) {
            if (_logExceptionEvent != null)
                _logExceptionEvent(level, message, ex);
        }

        public void LogException(LogLevel level, Exception ex) {
            LogException(level, string.Empty, ex);
        }

        public void LogMessage(LogLevel level, string message) {
            if (_logMessageEvent != null)
                _logMessageEvent(level, message);
        }

        public event LogExceptionEventHandler LogExceptionEvent {
            add { _logExceptionEvent += value; }
            remove { _logExceptionEvent -= value; }
        }

        public event LogMessageEventHandler LogMessageEvent {
            add { _logMessageEvent += value; }
            remove { _logMessageEvent -= value; }
        }

        public void LogMemberEntry() {
            StackTrace stack = new StackTrace();
            StackFrame caller = stack.GetFrame(1);
            if(caller == null)
                return;
            string memberName = string.Format("{0}.{1}", caller.GetMethod().DeclaringType.Name, caller.GetMethod().Name);

            LogMessage(LogLevel.Debug, string.Format("Entering {0}", memberName));
        }
        
        public void LogMemberExit() {
            StackTrace stack = new StackTrace();
            StackFrame caller = stack.GetFrame(1);
            if (caller == null)
                return;
            string memberName = string.Format("{0}.{1}", caller.GetMethod().DeclaringType.Name, caller.GetMethod().Name);

            LogMessage(LogLevel.Debug, string.Format("Leaving {0}", memberName));            
        }

        public static Logger Instance {
            //fully lazy instantiation technique inspired by
            //http://www.yoda.arachsys.com/csharp/singleton.html
            get { return Helper._loggerInstance; }
        }

        private class Helper {
            static Helper(){}
            internal static readonly Logger _loggerInstance = new Logger();
        }
    }
}
