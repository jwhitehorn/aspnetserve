/************************************************************************
 * Copyright (c) 2006-2008, Jason Whitehorn (jason.whitehorn@gmail.com)
 * All rights reserved.
 * 
 * Source code and binaries distributed under the terms of the included
 * license, see license.txt for details.
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using aspNETserve;
using aspNETserve.Configuration;
using aspNETserve.Configuration.Xml;

namespace aspNETserve.Ice {
    public partial class Service1 : ServiceBase {
        private IList<IAsyncServer> _servers;
        private const string _configFileName = "aspNETserve.config.xml";
        private readonly FileSystemWatcher _fileWatcher;
        
        public Service1() {
            InitializeComponent();

            //setup a file watcher that will cycle the applications 
            //in case of a configuration change.
            _fileWatcher = new FileSystemWatcher(Environment.CurrentDirectory, ConfigFileName);
            _fileWatcher.Changed += delegate { CycleServer(); };
        }

        public void Start() {
            OnStart(null);
        }

        protected override void OnStart(string[] args) {
            Configure();
            foreach (IAsyncServer server in _servers)
                server.StartAsync(); 
        }

        protected override void OnStop() {
            foreach (IAsyncServer server in _servers)
                server.StopAsync();


            //since we want this thread to block until all the servers have stopped or
            //errored out then we need to wait. The reason we did async in the first place
            //was to speed up the shutdown process by allowing all the servers to shutdown
            //at the same time.

            bool waiting;
            do {
                waiting = false;    //We reset this at the top of each loop, otherwise we would never exit.

                foreach (IAsyncServer server in _servers) { //so look at each IAsyncServer
                    if(server.Status == ServerStatus.ShuttingDown) {
                        //and if the status is "ShuttingDown" then that means we need to keep waiting.
                        waiting = true;
                        break;
                    }
                }
                Thread.Sleep(300);  //Sleep a little to give other threads time to work
            } while (waiting);

            //At this point we know that none of the servers in shutting down.
            //What we don't know is what status they are in. But for now we
            //simply won't concern ourselfs, and assume they have stopped.
        }

        private void CycleServer() {
            OnStop();
            OnStart(null);
        }

        private void Configure() {
            if(_servers != null) {  //if the collection of servers exists then...
                foreach(IAsyncServer server in _servers) {
                    server.Dispose();   //...just make sure to call dipose on each server object.
                }
            }

            _servers = new List<IAsyncServer>(); //construct a new collection of servers

            using (FileStream configFile = File.OpenRead(ConfigFileName)) {
                //using the configuration file, read the application config elements...

                IConfiguration config = XmlConfigurationManager.FromXml(configFile);
                foreach(IApplication application in config.Applications) {
                    IAsyncServer server = new AsyncServer(application); //... to construct each server.
                    _servers.Add(server);   //...and add it to the collection.
                }
            }
        }

        private string ConfigFileName {
            get {
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string result = Path.Combine(path, _configFileName);
                return result;
            }
        }
    }
}
