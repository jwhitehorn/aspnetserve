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
using System.Reflection;

public partial class GetServerVariable : System.Web.UI.Page {
    protected void Page_Load(object sender, EventArgs e) {
        string var = Request["var"];

        string[] properties = var.Split(new char[] { '.' });
        object result = typeof(GetServerVariable).InvokeMember(properties[0], BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, this, new object[] { });
        for (int i = 1; i < properties.Length; i++) {
            result = result.GetType().InvokeMember(properties[i], BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, result, new object[] { });
        }
        Response.ContentType = "text/plain";
        Response.ClearContent();
        if (result == null)
            result = string.Empty;
        Response.Write(result);
        Response.End();
    }
}
