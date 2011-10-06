<%@ WebHandler Language="C#" Class="Handler" %>
/************************************************************************
 * Copyright (c) 2006-2008, Jason Whitehorn (jason.whitehorn@gmail.com)
 * All rights reserved.
 * 
 * Source code and binaries distributed under the terms of the included
 * license, see license.txt for details.
 ************************************************************************/
using System;
using System.Web;
using System.IO;

public class Handler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "image/jpeg";
        context.Response.Cache.SetCacheability(HttpCacheability.Public);
        if(context.Request["chunked"] == "true")
            context.Response.BufferOutput = false;
        
        using (FileStream f = File.Open(context.Server.MapPath("~/images/" + context.Request["img"]), FileMode.Open)) {
            byte[] buffer = new byte[f.Length];
            f.Read(buffer, 0, (int)f.Length);

            context.Response.OutputStream.Write(buffer, 0, (int)f.Length);
            f.Close();
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}