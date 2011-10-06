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
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using aspNETserve;

namespace SimpleWebServer {
    public partial class frm_Main : Form {
        private WebApplicationPackage _wap; 
        public frm_Main(string[] args) {
            InitializeComponent();
            if (args != null && args.Length > 0) {
                txtPhysicalDir.Text = args[0];
                btnStart_Click(null, null);
            }
        }

        private void frm_Main_Load(object sender, EventArgs e) {
            DisplayStatus();
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            CloseWapIfNeeded();
        }

        private void btnStart_Click(object sender, EventArgs e) {
            System.Threading.Thread.Sleep(10);
            pictureBox1.Refresh();
            pnlPleaseWait.Visible = true;
            pnlButtons.Visible = false;
            if (Program.Server == null) {

                if(txtPhysicalDir.Text.ToLower().Trim().EndsWith(".wap")) {
                    _wap = new WebApplicationPackage(File.Open(txtPhysicalDir.Text, FileMode.Open), true);
                    _wap.Open();
                    Program.Server = new AsyncServer(System.Net.IPAddress.Any, txtVirtualDir.Text, _wap.PhysicalPath, int.Parse(txtPort.Text));                    
                }else {
                    Program.Server = new AsyncServer(System.Net.IPAddress.Any, txtVirtualDir.Text, txtPhysicalDir.Text, int.Parse(txtPort.Text));   
                }
                Program.Server.ServerRunning += OnServerRunning;
                Program.Server.FailureStarting += OnServerStartFailure;
                try {
                    Program.Server.StartAsync();
                } catch {
                    Program.Server = null;
                    MessageBox.Show("Error starting server, please try again.");
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e) {
            if (Program.Server != null) {
                Program.Server.Stop();
                Program.Server = null;
            }
            CloseWapIfNeeded();
            DisplayStatus();
        }

        private void OnServerRunning(object sender, EventArgs e) {
            DisplayStatus();
            if (pnlPleaseWait.InvokeRequired) {
                pnlPleaseWait.Invoke(new MethodInvoker(delegate() { pnlPleaseWait.Visible = false; pnlButtons.Visible = true; }));
            } else {
                pnlPleaseWait.Visible = false;
                pnlButtons.Visible = true;
            }
        }

        private void OnServerStartFailure(object sender, EventArgs e) {
            Program.Server = null;
            CloseWapIfNeeded();
            MessageBox.Show("Unable to start server on the desired port. Please try another port.");
            DisplayStatus();
            if (pnlPleaseWait.InvokeRequired) {
                pnlPleaseWait.Invoke(new MethodInvoker(delegate() { pnlPleaseWait.Visible = false; pnlButtons.Visible = true; }));
            } else {
                pnlPleaseWait.Visible = false;
                pnlButtons.Visible = true;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e) {
            DialogResult result = folderBrowser.ShowDialog();
            txtPhysicalDir.Text = folderBrowser.SelectedPath;
        }
        private void DisplayStatus() {
            if (InvokeRequired) {
                Invoke(new MethodInvoker(DisplayStatus));
                return;
            }
            this.Text = "aspNETserve " + /*aspNETserve.Server.Version() +*/ " (";
            if (Program.Server == null){
                this.Text += "Not Running)";
                lblViewSite.Visible = false;
                lnkSite.Visible = false;
                txtVirtualDir.Enabled = true;
                txtPhysicalDir.Enabled = true;
                txtPort.Enabled = true;
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                btnBrowse.Enabled = true;
            }else {
                this.Text += "Running)";
                lblViewSite.Visible = true;
                txtVirtualDir.Enabled = false;
                txtPhysicalDir.Enabled = false;
                txtPort.Enabled = false;
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                btnBrowse.Enabled = false;
                lnkSite.Text = "http://localhost:" + txtPort.Text + txtVirtualDir.Text;
                lnkSite.Visible = true;
            }
        }

        private void lnkSite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                System.Diagnostics.Process.Start(lnkSite.Text);
            } catch { }//i don't care if you can't open the url!
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            if(Program.Server != null)
                Program.Server.Dispose();
            Application.Exit();
        }

        private void aboutAspNETserveToolStripMenuItem_Click(object sender, EventArgs e) {
            frm_About about = new frm_About();
            about.ShowDialog();
        }

        private void frm_Main_SizeChanged(object sender, EventArgs e) {
            if (WindowState == FormWindowState.Minimized) {
                ShowInTaskbar = false;
                t.Text = Text;
                t.Visible = true;
                t.Icon = Icon;
                t.MouseDoubleClick += new MouseEventHandler(t_MouseClick);
                t.ContextMenu = new ContextMenu(new MenuItem[] { new MenuItem("Show",  t_MouseClick)});
                string message;
                if (Program.Server != null) {
                    message = lnkSite.Text;
                } else {
                    message = "aspNETserve is still open";
                }
                t.ShowBalloonTip(5000, "aspNETserve", message, ToolTipIcon.None);
            }
        }

        private void t_MouseClick(object sender, EventArgs e) {
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            t.Visible = false;
        }

        private void CloseWapIfNeeded() {
            if (_wap != null) {
                _wap.Dispose();
                _wap = null;
            }
        }

        NotifyIcon t = new NotifyIcon();

        private void debugOutputToolStripMenuItem1_Click(object sender, EventArgs e) {
            frm_DebugOutput debugOutput = new frm_DebugOutput();
            debugOutput.Show(this);
        }
    }
}