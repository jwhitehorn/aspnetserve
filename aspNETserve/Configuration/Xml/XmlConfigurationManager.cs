/************************************************************************
 * Copyright (c) 2006-2008, Jason Whitehorn (jason.whitehorn@gmail.com)
 * All rights reserved.
 * 
 * Source code and binaries distributed under the terms of the included
 * license, see license.txt for details.
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace aspNETserve.Configuration.Xml {
    public static class XmlConfigurationManager {
        private static readonly XmlSchema _xmlSchema;

        static XmlConfigurationManager() {
            Assembly currentAsm = Assembly.GetExecutingAssembly();
            using (Stream s = currentAsm.GetManifestResourceStream("aspNETserve.Configuration.Xml.aspNETserve.config.xsd")) {
                _xmlSchema = XmlSchema.Read(s, null);
                s.Close();
            }
        }

        public static IConfiguration FromXml(Stream stream) {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(stream);

            xDoc.Schemas.Add(_xmlSchema);
            xDoc.Validate(null);    //asert that the document confirms to the XSD

            Configuration config = new Configuration(new List<IApplication>());

            XmlElement serverElement = xDoc.DocumentElement;    //load the root element, named "server".
            foreach(XmlNode applicationElement in serverElement.ChildNodes) {   //loop over each "application" element...

                string physicalPath = applicationElement.Attributes["physicalPath"].Value; 
                //construct an IApplcation to represent this XmlNode
                Application app = new Application(physicalPath, new List<IDomain>(), new List<IEndPoint>());

                foreach(XmlNode childNode in applicationElement.ChildNodes) {
                    //the "application" element can either contain "domain" or "endpoint" elements, so...

                    switch (childNode.Name) {    //...first figure out which it it
                        case "domain":
                            Domain domain = new Domain(childNode.Attributes["name"].Value, childNode.Attributes["virtualPath"].Value);
                            app.Domains.Add(domain);
                            break;
                        case "endpoint":
                            IPAddress ip = IPAddress.Parse(childNode.Attributes["ip"].Value);
                            int port = int.Parse(childNode.Attributes["port"].Value);
                            //since the "secure" attribute is optional we need to check for null first.
                            bool isSecure = childNode.Attributes["secure"] == null ? false : bool.Parse(childNode.Attributes["secure"].Value);
                            EndPoint endPoint = new EndPoint(ip, port, isSecure);
                            app.EndPoints.Add(endPoint);
                            break;
                    }
                }

                config.Applications.Add(app);   //finally add the IApplication to the running tally on the configuration object.
            }

            return config;
        }

    }
}
