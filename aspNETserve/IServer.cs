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
    /// Interface to represent an aspNETserve server.
    /// </summary>
    public interface IServer : IDisposable {
        void Start();
        void Stop();
        ServerStatus Status { get; }
    }
}
