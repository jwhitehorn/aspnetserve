/************************************************************************
 * Copyright (c) 2006-2008, Jason Whitehorn (jason.whitehorn@gmail.com)
 * All rights reserved.
 * 
 * Source code and binaries distributed under the terms of the included
 * license, see license.txt for details.
 ************************************************************************/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class PostTest : System.Web.UI.Page {
    protected void Page_Load(object sender, EventArgs e) {

    }
    protected void btnSubmit_Click(object sender, EventArgs e) {
        lblWelcome.Text = "Welcome " + txtName.Text;
        Panel1.Visible = true;
    }
    protected void btnClear_Click(object sender, EventArgs e) {
        Panel1.Visible = false;
    }
}
