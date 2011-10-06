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
using System.Net;
using System.IO;
using NUnit.Framework;
using aspNETserve;


namespace IntegrationTests.NetServeTests {
    [TestFixture]
    public class HttpHandlerTests : BaseTest {
        [Test]
        [Category("/")]
        public void SimpleHandlerTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/", PhysicalPath, _port)) {
                s.Start();
                WebResponse response = GetPageResponse("http://localhost:" + _port + "/Handler.ashx");
                Assert.AreEqual("text/plain; charset=utf-8", response.ContentType);
                Stream stream = response.GetResponseStream();
                byte[] buffer = new byte[response.ContentLength];
                stream.Read(buffer, 0, (int)response.ContentLength);
                Assert.AreEqual("Hello World", Encoding.UTF8.GetString(buffer));
                s.Stop();
            }
        }
        [Test]
        [Category("virtual directory")]
        public void VirtualDirectorySimpleHandlerTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/vDir", PhysicalPath, _port)) {
                s.Start();
                WebResponse response = GetPageResponse("http://localhost:" + _port + "/vDir/Handler.ashx");
                Assert.AreEqual("text/plain; charset=utf-8", response.ContentType);
                Stream stream = response.GetResponseStream();
                byte[] buffer = new byte[response.ContentLength];
                stream.Read(buffer, 0, (int)response.ContentLength);
                Assert.AreEqual("Hello World", Encoding.UTF8.GetString(buffer));
                s.Stop();
            }
        }
        [Test]
        [Category("/")]
        public void ImageHandlerTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/", PhysicalPath, _port)) {
                s.Start();
                WebResponse response = GetPageResponse("http://localhost:" + _port + "/images/Handler.ashx?img=image1.jpg&chunked=false");
                Assert.AreEqual("image/jpeg", response.ContentType);
                Stream stream = response.GetResponseStream();
                
                using(FileStream f = File.Open(PhysicalPath + "images\\image1.jpg", FileMode.Open)){
                    byte[] actualData = new byte[f.Length];
                    f.Read(actualData, 0, (int)f.Length);
                    f.Close();

                    List<byte> downloadedData = new List<byte>(actualData.Length);
                    while (stream.CanRead) {
                        int v = stream.ReadByte();
                        if (v == -1)
                            break;
                        downloadedData.Add((byte)v);
                    }

                    Assert.AreEqual(actualData.Length, downloadedData.Count);
                    for (int i = 0; i != actualData.Length; i++) {
                        Assert.AreEqual(actualData[i], downloadedData[i]);
                    }
                }

                s.Stop();
            }
        }
        [Test]
        [Category("virtual directory")]
        public void VirtualDirectoryImageHandlerTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/vDir", PhysicalPath, _port)) {
                s.Start();
                WebResponse response = GetPageResponse("http://localhost:" + _port + "/vDir/images/Handler.ashx?img=image1.jpg&chunked=false");
                Assert.AreEqual("image/jpeg", response.ContentType);
                Stream stream = response.GetResponseStream();

                using (FileStream f = File.Open(PhysicalPath + "images\\image1.jpg", FileMode.Open)) {
                    byte[] actualData = new byte[f.Length];
                    f.Read(actualData, 0, (int)f.Length);
                    f.Close();

                    List<byte> downloadedData = new List<byte>(actualData.Length);
                    while (stream.CanRead) {
                        int v = stream.ReadByte();
                        if (v == -1)
                            break;
                        downloadedData.Add((byte)v);
                    }

                    Assert.AreEqual(actualData.Length, downloadedData.Count);
                    for (int i = 0; i != actualData.Length; i++) {
                        Assert.AreEqual(actualData[i], downloadedData[i]);
                    }
                }

                s.Stop();
            }
        }
        [Test]
        [Category("/")]
        public void ImageHandlerChunkedTransferTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/", PhysicalPath, _port)) {
                s.Start();
                WebResponse response = GetPageResponse("http://localhost:" + _port + "/images/Handler.ashx?img=image1.jpg&chunked=true");
                Assert.AreEqual("image/jpeg", response.ContentType);
                Stream stream = response.GetResponseStream();

                using (FileStream f = File.Open(PhysicalPath + "images\\image1.jpg", FileMode.Open)) {
                    byte[] actualData = new byte[f.Length];
                    f.Read(actualData, 0, (int)f.Length);
                    f.Close();

                    List<byte> downloadedData = new List<byte>(actualData.Length);
                    while (stream.CanRead) {
                        int v = stream.ReadByte();
                        if (v == -1)
                            break;
                        downloadedData.Add((byte)v);
                    }

                    Assert.AreEqual(actualData.Length, downloadedData.Count);
                    for (int i = 0; i != actualData.Length; i++) {
                        Assert.AreEqual(actualData[i], downloadedData[i]);
                    }
                }

                s.Stop();
            }
        }
        [Test]
        [Category("virtual directory")]
        public void VirtualDirectoryImageHandlerChunkedTransferTest() {
            using (Server s = new Server(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), "/vDir", PhysicalPath, _port)) {
                s.Start();
                WebResponse response = GetPageResponse("http://localhost:" + _port + "/vDir/images/Handler.ashx?img=image1.jpg&chunked=true");
                Assert.AreEqual("image/jpeg", response.ContentType);
                Stream stream = response.GetResponseStream();

                using (FileStream f = File.Open(PhysicalPath + "images\\image1.jpg", FileMode.Open)) {
                    byte[] actualData = new byte[f.Length];
                    f.Read(actualData, 0, (int)f.Length);
                    f.Close();

                    List<byte> downloadedData = new List<byte>(actualData.Length);
                    while (stream.CanRead) {
                        int v = stream.ReadByte();
                        if (v == -1)
                            break;
                        downloadedData.Add((byte)v);
                    }

                    Assert.AreEqual(actualData.Length, downloadedData.Count);
                    for (int i = 0; i != actualData.Length; i++) {
                        Assert.AreEqual(actualData[i], downloadedData[i]);
                    }
                }

                s.Stop();
            }
        }
    }
}
