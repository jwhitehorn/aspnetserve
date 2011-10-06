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
    /// <summary>
    /// An IAspNetWorker does all the dirty work
    /// of handling a request, and generating a response.
    /// </summary>
    public interface IAspNetWorker {
        void ProcessTransaction(ITransaction transaction);
        IDictionary<string, string> ServerVariables { set; get; }

        /// <summary>
        /// The virtual directory server configuration element.
        /// </summary>
        string GetAppPath();
        /// <summary>
        /// The physical path to the web root.
        /// </summary>
        string GetAppPathTranslated();
        /// <summary>
        /// The current executing AppDomain's Id
        /// </summary>
        string GetAppPoolID();
        /// <summary>
        /// The numbers of bytes posted
        /// </summary>
        long GetBytesRead();
        /// <summary>
        /// Allways returns 0
        /// </summary>
        long GetConnectionID();
        /// <summary>
        /// The virtual file path
        /// </summary>
        string GetFilePath();
        /// <summary>
        /// Returns the path on disk for the current URI.
        /// </summary>
        /// <remarks>
        /// If the URI lacks a file name, and instead just specifies a directory, 
        /// then the result of GetFilePathTranslated will just be the directory.
        /// </remarks>
        string GetFilePathTranslated();
        /// <summary>
        /// Gets the HTTP verb for the current request.s
        /// </summary>
        string GetHttpVerbName();
        /// <summary>
        /// Gets the version of the HTTP protocol used for communications with the client.
        /// </summary>
        string GetHttpVersion();
        /// <summary>
        /// Returns a known Http header by its index as refered to by HttpWorkerRequest
        /// </summary>
        /// <returns></returns>
        string GetKnownRequestHeader(int index);
        /// <summary>
        /// Gets the network address that the current request was answered on. This is a address
        /// corresponding to the local server.
        /// </summary>
        string GetLocalAddress();
        /// <summary>
        /// Gets the port number that the current request was answered on. This is a port
        /// number belonging to the local server.
        /// </summary>
        int GetLocalPort();
        /// <summary>
        /// Gets the "tail" portion of the request URI.
        /// </summary>
        string GetPathInfo();
        /// <summary>
        /// Gets the portion of bytes posted that has been loaded so far.
        /// </summary>
        byte[] GetPreloadedEntityBody();
        /// <summary>
        /// If the request and response are secure the protocol is HTTPS, otherwise HTTP.
        /// </summary>
        string GetProtocol();
        /// <summary>
        /// Determines if the HTTP headers have been sent to the client yet or not.
        /// </summary>
        bool HeadersSent();
        /// <summary>
        /// Sends any unsent content to the client.
        /// </summary>
        void FlushResponse(bool finalFlush);
    }
}
