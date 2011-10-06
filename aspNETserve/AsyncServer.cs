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
using System.Net;
using System.Threading;
using aspNETserve.Configuration;

namespace aspNETserve {
    /// <summary>
    /// An implementation of IServer that provides asynchronous operations.
    /// </summary>
    public class AsyncServer : Server, IAsyncServer {
        private EventHandler _serverRunning;
        private EventHandler _failureStarting;
        private EventHandler _serverStopped;
        private EventHandler _failureStopping;

        public AsyncServer(IPAddress endPoint, string virtualDir, string physicalDir, int port)
            : base(endPoint, virtualDir, physicalDir, port) { }

        public AsyncServer(IApplication application) : base(application) {}

        public virtual void StartAsync() {
            bool queued = ThreadPool.QueueUserWorkItem(
                    delegate(object o) {
                        try {
                            Start();
                        } catch { } //this needs to be supressed in this context
                    });
            if (!queued)
                throw new ApplicationException("Error starting");
        }

        public virtual void StopAsync() {
            bool queued = ThreadPool.QueueUserWorkItem(
                delegate {
                    try {
                        Stop();
                    } catch { } //this needs to be supressed in this context
                });
            if (!queued)
                throw new ApplicationException("Error starting");
        }

        public virtual event EventHandler ServerRunning {
            add { _serverRunning += value; }
            remove { _serverRunning -= value; }
        }

        public virtual event EventHandler FailureStarting {
            add { _failureStarting += value; }
            remove { _failureStarting -= value; }
        }

        public virtual event EventHandler ServerStopped {
            add { _serverStopped += value; }
            remove { _serverStopped -= value; }
        }

        public virtual event EventHandler FailureStopping {
            add { _failureStopping += value; }
            remove { _failureStopping -= value; }
        }

        protected override void SetStatus(ServerStatus status) {
            ServerStatus previousStatus = Status;
            base.SetStatus(status);

            if(status == previousStatus)
                return; //if there was not real state change (for whatever reason) then none of the below...
                        //...events matter. So there is no need to go any further.

            if (Status == ServerStatus.Running && _serverRunning != null)
                _serverRunning(this, EventArgs.Empty);  //regardless of the previous state, if we are running fire the "ServerRunning" event

            if (previousStatus == ServerStatus.Starting && Status == ServerStatus.Stopped && _failureStarting != null)
                _failureStarting(this, EventArgs.Empty);    //if we were starting but have now stopped this indicates a failure starting.

            if (Status == ServerStatus.Stopped && _serverStopped != null)
                _serverStopped(this, EventArgs.Empty);  //regardless of the previous state, if we've stopped fire the "ServerStopped" event

            if (previousStatus == ServerStatus.ShuttingDown && Status == ServerStatus.Running && _failureStopping != null)
                _failureStopping(this, EventArgs.Empty);    //if we were trying to stop, but have now given up and continued running
                                                            //fire the "FailureStopping" event.

        }
    }
}
