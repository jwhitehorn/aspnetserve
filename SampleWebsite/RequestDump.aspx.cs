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

public partial class RequestDump : System.Web.UI.Page {
    protected void Page_Load(object sender, EventArgs e) {
        string result = "Accept Types:<br/>";
        foreach (string acceptType in Context.Request.AcceptTypes) {
            result += acceptType + "<br/>";
        }
        try {
            result += "AnonymousID: " + Context.Request.AnonymousID + "<br/>";
        } catch {
            result += "ERROR accessing Context.Request.AnonymousID<br/>";
        }
        try {
            result += "ApplicationPath: " + Context.Request.ApplicationPath + "<br/>";
        } catch {
            result += "ERROR accessing Context.Request.ApllicationPath<br/>";
        }
        try {
            result += "AppRelativeCurrentExecutionFilePath: " + Context.Request.AppRelativeCurrentExecutionFilePath + "<br/>";
        } catch {
            result += "ERROR accessing Context.Request.AppRelativeCurrentExecutionFilePath<br/>";
        }
        result += "Browser: " + Context.Request.Browser.Type + "<br/>";
        result += "ContentLength: " + Context.Request.ContentLength + "<br/>";
        result += "CurrentExecutionFilePath: " + Context.Request.CurrentExecutionFilePath + "<br/>";
        result += "FilePath: " + Context.Request.FilePath + "<br/>";
        result += "HttpMethod: " + Context.Request.HttpMethod + "<br/>";
        result += "IsAuthenticated: " + Context.Request.IsAuthenticated + "<br/>";
        result += "IsLocal: " + Context.Request.IsLocal + "<br/>";
        result += "IsSecureConnection: " + Context.Request.IsSecureConnection + "<br/>";
        result += "Path: " + Context.Request.Path + "<br/>";
        result += "PathInfo: " + Context.Request.PathInfo + "<br/>";
        result += "PhysicalApplicationPath: " + Context.Request.PhysicalApplicationPath + "<br/>";
        result += "PhysicalPath: " + Context.Request.PhysicalPath + "<br/>";
        result += "QueryString: " + Context.Request.QueryString.ToString() + "<br/>";
        result += "RawUrl: " + Context.Request.RawUrl + "<br/>";
        result += "RequestType: " + Context.Request.RequestType + "<br/>";
        result += "TotalBytes: " + Context.Request.TotalBytes + "<br/>";
        result += "Url: " + Context.Request.Url.ToString() + "<br/>";
        result += "UrlReferrer: " + Context.Request.UrlReferrer.ToString() + "<br/>";
        result += "UserAgent: " + Context.Request.UserAgent + "<br/>";
        result += "UserHostAddress: " + Context.Request.UserHostAddress + "<br>";
        result += "UserHostName: " + Context.Request.UserHostName + "<br/>";
        Response.Write(result);
    }
}
