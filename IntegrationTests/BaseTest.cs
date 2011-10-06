/************************************************************************
 * Copyright (c) 2006-2008, Jason Whitehorn (jason.whitehorn@gmail.com)
 * All rights reserved.
 * 
 * Source code and binaries distributed under the terms of the included
 * license, see license.txt for details.
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using NUnit.Framework;
using aspNETserve;
using System.Net;

namespace IntegrationTests {
    public class BaseTest {
        [SetUp]
        public void Setup() {
            _testId = Guid.NewGuid().ToString();
        }
        [TearDown]
        public void Teardown() {
        }
    //---------------------------------
        protected string TestId {
            get { return _testId; }
        }
        protected string PhysicalPath {
            get { /*return _tempPath + "\\" + _testId;*/
                return _pathToSite;
            }
        }
        protected string PostValues(string url, NameValueCollection values) {
            WebClient helper = new WebClient();
            string page = helper.DownloadString(url);
            MatchCollection names = Regex.Matches(page, "name=\"([^\"]+)\"[^/>]*value=\"([^\"]+)\"");
            for(int i = 0; i != names.Count; i++){
                string name = names[i].Groups[1].Value;
                if (values[name] == null) {
                    string value = names[i].Groups[2].Value;
                    values.Add(name, value);
                }
            }
            byte[] data = helper.UploadValues(url, values);
            page = Encoding.UTF8.GetString(data);
            return page;
        }
        protected string GetServerVariable(string variable) {
            return GetServerVariable(variable, "/");
        }
        protected string GetServerVariable(string variable, string virtualDir) {
            Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), virtualDir, PhysicalPath, _port);
            s.Start();
            string data = "";
            string url;
            if (virtualDir == "/")
                url = "http://localhost:" + _port + "/GetServerVariable.aspx?var=" + variable;
            else
                url = "http://localhost:" + _port + virtualDir + "/GetServerVariable.aspx?var=" + variable;
            try {
                data = GetPage(url);
            } finally {
                s.Stop();
            }
            return data;
        }
        protected string GetPage(string url) {
            string data = "";
            WebResponse response = GetPageResponse(url);
            Stream stream = response.GetResponseStream();
            byte[] buffer = new byte[response.ContentLength];
            stream.Read(buffer, 0, (int)response.ContentLength);
            data = Encoding.UTF8.GetString(buffer);
            return data;
        }
        protected WebResponse GetPageResponse(string url) {
            System.Net.WebRequest helper = HttpWebRequest.Create(url);
            ((HttpWebRequest)helper).UserAgent = _userAgent;

            WebResponse response = helper.GetResponse();
            return response;
        }

        protected const string _userAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.8.1.4) Gecko/20070515 Firefox/2.0.0.4";
        protected const int _port = 8081;
        private string _testId;
        private const string _tempPath = "c:\\jason\\temp";
        private const string _pathToSite = @"C:\Users\Jason\projects\aspNETserve\SampleWebsite\";
    }
}
