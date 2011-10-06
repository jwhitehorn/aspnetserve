/************************************************************************
 * Copyright (c) 2006-2008, Jason Whitehorn (jason.whitehorn@gmail.com)
 * All rights reserved.
 * 
 * Source code and binaries distributed under the terms of the included
 * license, see license.txt for details.
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace aspNETserve.Core {

    public interface IRequest {

        string HttpMethod { get; }
        NameValueCollection QueryString { get; }
        Uri Url { get; }
        string RawUrl { get; }
        byte[] PostData { get; }
        string GetKnownRequestHeader(int index);
        string GetUnknownRequestHeader(string name);
        System.Net.IPEndPoint LocalEndPoint { get; }
        System.Net.IPEndPoint RemoteEndPoint { get; }
        Guid RequestId { get; }
        string HttpVersion { get; }
        /// <summary>
        /// Determines if the client connection is suppose to be kept alive or closed
        /// after the request is processed.
        /// </summary>
        bool IsKeepAlive { get; }
        /// <summary>
        /// Determines if the request was sent over a secure channel.
        /// </summary>
        bool IsSecure { get; }
    }
}
