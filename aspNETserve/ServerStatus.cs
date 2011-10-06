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

namespace aspNETserve {
    /// <summary>
    /// Enumeration of possible IServer statuses.
    /// </summary>
    public enum ServerStatus {
        /// <summary>
        /// A "Stopped" server is one that is not running, or attempting to run or shutdown.
        /// </summary>
        Stopped,
        /// <summary>
        /// A "Starting" server is one that is attempted to Start, but is not yet operational.
        /// </summary>
        Starting,
        /// <summary>
        /// A "Running" server is one that is started and ready for use.
        /// </summary>
        Running,
        /// <summary>
        /// A "ShuttingDown" server is one that is attempting to stop.
        /// </summary>
        ShuttingDown
    }
}
