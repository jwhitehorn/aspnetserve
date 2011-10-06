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
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SimpleWebServer {
    public partial class frm_DebugOutput : Form {
        private AsynMemoryStream _memoryStream;
        private TraceListener _traceListener;
        private Thread _flusherThread;

        public frm_DebugOutput() {
            InitializeComponent();
        }

        private void DebugOutput_Load(object sender, EventArgs e) {
            _memoryStream = new AsynMemoryStream();
            _memoryStream.ContentsUpdated += TraceUpdated;
            _traceListener = new TextWriterTraceListener(_memoryStream);
            Trace.Listeners.Add(_traceListener);
            Trace.WriteLine("Debug Output Console Opened");

            _flusherThread = new Thread(delegate() {
                                            while (Thread.CurrentThread.IsAlive) {
                                                Trace.Flush();
                                                Thread.Sleep(300);
                                            }
                                        });
            _flusherThread.Start();
        }

        protected void TraceUpdated(object sender, EventArgs e) {
            DataWriteEventArgs dataEvent = e as DataWriteEventArgs;
            if(dataEvent == null)
                return;

            if(txtOutput.InvokeRequired) {
                txtOutput.Invoke(new EventHandler(TraceUpdated), sender, e);
                return;
            }

            txtOutput.Text += Encoding.UTF7.GetString(dataEvent.Buffer, dataEvent.Offset, dataEvent.Count);
        }

        private void frm_DebugOutput_FormClosing(object sender, FormClosingEventArgs e) {
            Trace.WriteLine("Debug Output Console Closed");
            Trace.Listeners.Remove(_traceListener);
            _flusherThread.Abort();
            _flusherThread.Join();
            _memoryStream.ContentsUpdated -= TraceUpdated;
            _traceListener = null;
            _memoryStream.Dispose();
            _memoryStream.Close();
        }
    }
}
