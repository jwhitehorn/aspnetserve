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
using System.Text;
using System.Windows.Forms;

namespace SimpleWebServer {
    public partial class frm_About : Form {
        public frm_About() {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e) {
            CenterToParent();
            string ver = System.Reflection.Assembly.GetAssembly(typeof(aspNETserve.Server)).GetName().Version.ToString();
            lblVersion.Text = string.Format("Version {0}", ver);
        }

        private void btnClose_Click(object sender, EventArgs e) {
            Close();
        }

        private void lblJasonWhitehorn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start("http://jason.whitehorn.ws/");
        }

        private void lblaspNETserveHomepage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start("http://code.google.com/p/aspnetserve/");
        }
    }
}