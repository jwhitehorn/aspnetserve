/************************************************************************
 * Copyright (c) 2006-2008, Jason Whitehorn (jason.whitehorn@gmail.com)
 * All rights reserved.
 * 
 * Source code and binaries distributed under the terms of the included
 * license, see license.txt for details.
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using aspNETserve;

namespace ConsoleServer {
    class Program {
        /* NOTE:
         * This is only intended to be an example of a web server.
         * It is not intended to be a serious implementation, and as
         * such is lacking user input validation.
         */ 
        static void Main(string[] args) {
            Console.WriteLine("aspNETserve console server");
            Console.Write("IP: ");
            string rawIp = Console.ReadLine();
            Console.Write("Port: ");
            string port = Console.ReadLine();
            Console.Write("Physical Path: ");
            string physicalPath = Console.ReadLine();
            Console.Write("Virtual Path: ");
            string virtualPath = Console.ReadLine();

            Console.WriteLine();
            Console.Write("Starting...");
            System.Net.IPAddress endPoint = System.Net.IPAddress.Parse(rawIp);
            using (Server s = new Server(endPoint, virtualPath, physicalPath, int.Parse(port))) {
                s.Start();
                Console.WriteLine("DONE");
                Console.Write("Press any key to stop");
                Console.Read();
            }
        }
    }
}
