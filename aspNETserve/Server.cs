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
using System.Web;
using System.Web.Hosting;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using aspNETserve.Core;
using aspNETserve.Configuration;
using aspNETserve.Core.Logging;

namespace aspNETserve {
    /// <summary>
    /// Handles incoming HTTP communications, and passes them into the AppDomain 
    /// for processing by the Host.
    /// </summary>
    public class Server : IServer {
        private string _appId;
        private int _port;
        private IPAddress _endPoint;
        private string _virtualDir;
        private string _physicalDir;
        private Socket _sock;
        private DomainHook _domainHook;
        private ApplicationManager _appManager;
        private IDictionary<string, string> _serverVariables;
        private ServerStatus _status = ServerStatus.Stopped;
        private int _openConnections;
        private int _maxConnections = 200;
        private int _initialRequestTimeout = 30000;
        private int _keepAliveRequestTimeout = 30000;

        protected Server() { }

        public Server(IPAddress endPoint, string virtualDir, string physicalDir, int port) {
            _endPoint = endPoint;
            _virtualDir = virtualDir;
            _physicalDir = physicalDir;
            if (!_physicalDir.EndsWith("\\"))
                _physicalDir += "\\";
            _port = port;
            _appId = Guid.NewGuid().ToString(); //generate a new application id
            _appManager = ApplicationManager.GetApplicationManager();
            PrepareServerVariables();
            ConfigureResponses();
        }

        public Server(IApplication application) : this( application.EndPoints[0].Ip, 
                                                        application.Domains[0].VirtualPath, 
                                                        application.PhysicalPath,
                                                        application.EndPoints[0].Port) {
            /*
             * This constructort takes simple-minded approach to reading configuration elements 
             * from an IApplication. While not robust, it is functional. A more complete
             * implementation "will come soon" :-).
             */
        }

        public virtual void Start() {
            /*
             * Create a new AppDomain containing the Host, and begin handling incoming requests.
             */ 
            if (_sock != null) 
                return;
            _openConnections = 0;
            SetStatus(ServerStatus.Starting);
            try {
                _domainHook = _appManager.CreateObject(_appId, typeof(DomainHook), _virtualDir, _physicalDir, false) as DomainHook;
                _domainHook.Configure(_virtualDir, _physicalDir, _serverVariables);
                _domainHook.RegisterILogger(Logger.Instance);
                _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _sock.Bind(new IPEndPoint(_endPoint, _port));
                _sock.Listen(100);
                _sock.BeginAccept(new AsyncCallback(ProcessRequest), _sock);
            } catch {
                SetStatus(ServerStatus.Stopped);
                throw;
            }
            SetStatus(ServerStatus.Running);
    
        }

        public virtual void Stop() {
            if (_sock != null) {
                SetStatus(ServerStatus.ShuttingDown);
                _sock.Close();  //stop accepting connections
                _domainHook.DeRegisterILogger(Logger.Instance);
                _domainHook.Dispose();    //this will call InitiateShutdown from within the app domain
            }
            _sock = null;
            SetStatus(ServerStatus.Stopped);
        }

        public virtual ServerStatus Status {
            get { return _status; }
        }

        public static string Version() {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public virtual void Dispose() {
            if (_status == ServerStatus.Running)
                Stop();
        }

        /// <summary>
        /// Gets or Sets the timeout period in ms for initial requests.
        /// </summary>
        public virtual int InitialRequestTimeout {
            get { return _initialRequestTimeout; }
            set {
                if(value <= 0)
                    throw new ArgumentOutOfRangeException("InitialRequestTimeout must be greater than zero.");
                _initialRequestTimeout = value;
            }
        }

        /// <summary>
        /// Gets or Sets the timeout period in ms for keep alive requests.
        /// This is the maximum time the server will wait to between receiving chuncks
        /// of data in a single request following a clients initial request.
        /// </summary>
        public virtual int KeepAliveRequestTimeout {
            get { return _keepAliveRequestTimeout; }
            set {
                if(value <= 0)
                    throw new ArgumentOutOfRangeException("KeepAliveRequestTimeout must be greater than zero.");
                _keepAliveRequestTimeout = value;
            }
        }

        /// <summary>
        /// The maximum number of simultaneous connections allowed.
        /// Incoming requests will be declined once the maximum amount
        /// has been reached.
        /// </summary>
        public virtual int MaxConnections {
            get { return _maxConnections; }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("MaxConnections must be greater than zero.");
                _maxConnections = value;
            }
        }

        protected virtual void PrepareServerVariables() {
            _serverVariables = new Dictionary<string, string>();
            _serverVariables.Add("SERVER_SOFTWARE", "aspNETserve/" + Version());
        }

        protected virtual void ConfigureResponses() {
//            Response error403Forbidden = new Response();
//            error403Forbidden.StatusDescription = "Forbidden";
//            error403Forbidden.StatusCode = (int)HttpResponseCode.NotFound;
//            error403Forbidden.RawData = Encoding.UTF8.GetBytes(@"
//                        <html>
//                        <head><title>403 - Forbidden</title></head>
//                        <body>
//                        <h1>403 - Forbidden</h1><br/>
//                        The server is actively refusing access to the specified resource.<br/>
//                        <hr/>
//                        aspNETserve/" + Version() + @"
//                        </body>
//                        </html>
//                    ");
//            Response.Error403Forbidden = error403Forbidden;
        }

        /// <summary>
        /// This method processes the socket request. This is the first entry point for requested entering the aspNETserve
        /// server.
        /// </summary>
        /// <param name="async">The IAsyncResult used to aquire to Socket.</param>
        protected virtual void ProcessRequest(IAsyncResult async) {
            Logger.Instance.LogMemberEntry();
            Socket com = null;
            Interlocked.Increment(ref _openConnections);
            try {
                com = _sock.EndAccept(async);
                _sock.BeginAccept(new AsyncCallback(ProcessRequest), _sock);

                if (_openConnections < _maxConnections) {   //if the number of open connections is "safe", then continue...

                    HttpRequestResponse transaction = null;
                    do {
                        int timeout = transaction == null ? _initialRequestTimeout : _keepAliveRequestTimeout; //the initial request and subsequent requests may have different timeouts
                        transaction = new HttpRequestResponse(new NetworkStream(com, false), (IPEndPoint)com.LocalEndPoint, (IPEndPoint)com.RemoteEndPoint, timeout);
                        _domainHook.ProcessTransaction(transaction);
                        transaction.Response.Flush();
                    } while (transaction.Request.IsKeepAlive);  //loop while the client wants to "keep alive".

                }   //otherwise we want to fall through and close this socket, and decrement the connection count.
                com.Close();

            } catch(Exception ex) {
                Logger.Instance.LogException(LogLevel.Error, "Error processing request", ex);
                if (com != null) {
                    try {
                        com.Close();
                    } catch { }
                }
            } finally {
                Interlocked.Decrement(ref _openConnections);
                Logger.Instance.LogMemberExit();
            }
        }

        protected virtual void SetStatus(ServerStatus status) {
            _status = status;
        }

        protected string AppId {
            get { return _appId; }
            set { _appId = value; }
        }

        protected int Port {
            get { return _port; }
            set { _port = value; }
        }

        protected IPAddress EndPoint {
            get { return _endPoint; }
            set { _endPoint = value; }
        }

        protected string VirtualDir {
            get { return _virtualDir; }
            set { _virtualDir = value; }
        }

        protected string PhysicalDir {
            get { return _physicalDir; }
            set { _physicalDir = value; }
        }

        protected Socket Sock {
            get { return _sock; }
            set { _sock = value; }
        }

        protected DomainHook DomainHook {
            get { return _domainHook; }
            set { _domainHook = value; }
        }

        protected ApplicationManager AppManager {
            get { return _appManager; }
            set { _appManager = value; }
        }

        protected IDictionary<string, string> ServerVariables {
            get { return _serverVariables; }
            set { _serverVariables = value; }
        }

    }
}
