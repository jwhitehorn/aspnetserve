/************************************************************************
 * Copyright (c) 2006-2008, Jason Whitehorn (jason.whitehorn@gmail.com)
 * All rights reserved.
 * 
 * Source code and binaries distributed under the terms of the included
 * license, see license.txt for details.
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using aspNETserve.Core;
using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTests {
    [TestFixture]
    public class AspNetWorkerTests {

        [Test]
        [Description("Determines that the GetAppPath method of the AspNetWorker IAspNetWorker implementation returns the virtual path provided on the constructor.")]
        public void GetAppPath_When_Virtual_Path_Is_Root_Test() {
            MockRepository mocks = new MockRepository();
            string virtualPath = @"/";

            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, virtualPath, @"z:\temp");

            Assert.AreEqual(virtualPath, aspNetWorker.GetAppPath());
        }

        [Test]
        [Description("Determines that the GetAppPath method of the AspNetWorker IAspNetWorker implementation returns the virtual path provided on the constructor.")]
        public void GetAppPath_When_Virtual_Path_Is_Complex_Test() {
            MockRepository mocks = new MockRepository();
            string virtualPath = @"/foo";

            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, virtualPath, @"z:\temp");

            Assert.AreEqual(virtualPath, aspNetWorker.GetAppPath());
        }

        [Test]
        [Description("Determines that the GetAppPathTranslated method returns the physical path as passed on AspNetWorker's constructor.")]
        public void GetAppPathTranslated_Test() {
            MockRepository mocks = new MockRepository();
            string physicalPath = @"c:\temp";

            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", physicalPath);

            Assert.AreEqual(physicalPath, aspNetWorker.GetAppPathTranslated());
        }

        [Test]
        [Description("Determines that the GetAppPoolID method returns teh ID of the AppDomain that the worker process resides in.")]
        public void GetAppPoolID_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", @"c:\temp");

            Assert.AreEqual(AppDomain.CurrentDomain.Id.ToString(), aspNetWorker.GetAppPoolID());
            
        }

        [Test]
        [Description("Determines that the GetBytesRead method returns the number of bytes read from the IRequest")]
        public void GetBytesRead_When_Bytes_Read_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();
            byte[] postData = new byte[]{1, 2, 3, 4, 5, 6, 7, 8, 9, 0};

            using(mocks.Unordered()) {
                Expect.Call(request.PostData).Return(postData).Repeat.Any();
                Expect.Call(request.RawUrl).Return("/test/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); } ).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual(postData.Length, aspNetWorker.GetBytesRead());

        }

        [Test]
        [Description("Determines that the GetBytesRead method returns the number of bytes read from the IRequest")]
        public void GetBytesRead_When_No_Bytes_Read_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();
            byte[] postData = new byte[] { };

            using (mocks.Unordered()) {
                Expect.Call(request.PostData).Return(postData).Repeat.Any();
                Expect.Call(request.RawUrl).Return("/test/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual(postData.Length, aspNetWorker.GetBytesRead());

        }

        [Test]
        [Description("Determines that GetConnectionID allows returns zero.")]
        public void GetConnectionID_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", @"c:\temp");
            Assert.AreEqual(0, aspNetWorker.GetConnectionID());
            
        }

        [Test]
        [Description("Determines that the GetFilePath correctly returns the file path for a folder request.")]
        public void GetFilePath_For_Root_Folder_Path_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/test/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual("/test/", aspNetWorker.GetFilePath());
        }

        [Test]
        [Description("Determines that the GetFilePath correctly returns the file path for a page request.")]
        public void GetFilePath_For_Root_Page_Path_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/test/Test.aspx").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual("/test/Test.aspx", aspNetWorker.GetFilePath());
        }

        [Test]
        [Description("Determines that the GetFilePath correctly returns the file path for a folder request.")]
        public void GetFilePath_For_Non_Root_Folder_Path_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/foo/test/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/foo", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual("/foo/test/", aspNetWorker.GetFilePath());
        }

        [Test]
        [Description("Determines that the GetFilePath correctly returns the file path for a page request.")]
        public void GetFilePath_For_Non_Root_Page_Path_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/foo/test/Test.aspx").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/foo", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual("/foo/test/Test.aspx", aspNetWorker.GetFilePath());
        }

        [Test]
        [Description("Determines that the GetFilePathTranslated method returns the filename for a folder request.")]
        public void GetFilePathTranslated_For_Folder_Path_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/foo/test/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/foo", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual(@"c:\temp\test", aspNetWorker.GetFilePathTranslated());
        }

        [Test]
        [Description("Determines that the GetFilePathTranslated method returns the filename for a page request.")]
        public void GetFilePathTranslated_For_Page_Path_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/foo/test/Page.aspx").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/foo", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual(@"c:\temp\test\Page.aspx", aspNetWorker.GetFilePathTranslated());
        }

        [Test]
        [Description("Determines that the GetFilePathTranslated method returns the filename for a folder request.")]
        public void GetFilePathTranslated_For_Root_Folder_Path_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/foo/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/foo", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual(@"c:\temp", aspNetWorker.GetFilePathTranslated());
        }

        [Test]
        [Description("Determines that the GetFilePathTranslated method returns the filename for a page request.")]
        public void GetFilePathTranslated_For_Root_Page_Path_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/foo/Page.aspx").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/foo", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual(@"c:\temp\Page.aspx", aspNetWorker.GetFilePathTranslated());
        }

        [Test]
        [Description("Determines that the GetHttpVerbName is correct when the HttpMethod is GET")]
        public void GetHttpVerbName_When_Is_Get_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/foo/").Repeat.Any();
                Expect.Call(request.HttpMethod).Return("GET").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/foo", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual("GET", aspNetWorker.GetHttpVerbName());
        }

        [Test]
        [Description("Determines that the GetHttpVerbName is correct when the HttpMethod is POST")]
        public void GetHttpVerbName_When_Is_Post_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/foo/").Repeat.Any();
                Expect.Call(request.HttpMethod).Return("POST").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/foo", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual("POST", aspNetWorker.GetHttpVerbName());
        }

        [Test]
        [Description("Determines that the GetHttpVerbName is correct when the HttpMethod is PUT")]
        public void GetHttpVerbName_When_Is_Put_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/foo/").Repeat.Any();
                Expect.Call(request.HttpMethod).Return("PUT").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/foo", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual("PUT", aspNetWorker.GetHttpVerbName());
        }

        [Test]
        [Description("Determines that the GetHttpVerbName is correct when the HttpMethod is DELETE")]
        public void GetHttpVerbName_When_Is_Delete_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/foo/").Repeat.Any();
                Expect.Call(request.HttpMethod).Return("DELETE").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/foo", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual("DELETE", aspNetWorker.GetHttpVerbName());
        }

        [Test]
        [Description("Determines that the GetHttpVersion simply delegates to the request object")]
        public void GetHttpVersion_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/foo/").Repeat.Any();
                Expect.Call(request.HttpVersion).Return("HTTP/1.1").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/foo", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual("HTTP/1.1", aspNetWorker.GetHttpVersion());
        }

        [Test]
        [Description("Determines that the GetKnownRequestHeader method simply delegates to the request object.")]
        public void GetKnownRequestHeader_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();
            string expectedValue = (new Guid()).ToString();
            int index = (new Random()).Next();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/foo/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(request.GetKnownRequestHeader(index)).Return(expectedValue).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/foo", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual(expectedValue, aspNetWorker.GetKnownRequestHeader(index));
        }

        [Test]
        [Description("Determines that the GetLocalAddress method simply delegates to the request object.")]
        public void GetLocalAddress_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();
            IPEndPoint ipEndPoint = new IPEndPoint(new IPAddress(new byte[]{47, 94, 23, 12}), 1238);

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/foo/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(request.LocalEndPoint).Return(ipEndPoint).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/foo", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual(ipEndPoint.Address.ToString(), aspNetWorker.GetLocalAddress());
        }

        [Test]
        [Description("Determines that the GetLocalPort method simply delegates to the request object.")]
        public void GetLocalPort_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();
            IPEndPoint ipEndPoint = new IPEndPoint(new IPAddress(new byte[] { 47, 94, 23, 12 }), 1238);

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/foo/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(request.LocalEndPoint).Return(ipEndPoint).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/foo", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual(ipEndPoint.Port, aspNetWorker.GetLocalPort());
        }

        [Test]
        [Description("Ensures that the GetPathInfo method returns the empty string when URI contains no tail")]
        public void GetPathInfo_With_No_Tail_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/foo/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/foo", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual("", aspNetWorker.GetPathInfo());
        }

        [Test]
        [Description("Ensures that the GetPathInfo method returns the empty string when URI contains no tail")]
        public void GetPathInfo_With_Tail_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/foo/Page.aspx/ATail").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/foo", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual("/ATail", aspNetWorker.GetPathInfo());
        }

        [Test]
        [Description("Determines that the GetPreLoadedEntityBody returns the posted data")]
        public void GetPreloadedEntityBody_When_No_Data_Loaded_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();
            byte[] postData = new byte[] { };

            using (mocks.Unordered()) {
                Expect.Call(request.PostData).Return(postData).Repeat.Any();
                Expect.Call(request.RawUrl).Return("/test/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual(postData, aspNetWorker.GetPreloadedEntityBody());
        }

        [Test]
        [Description("Determines that the GetPreLoadedEntityBody returns the posted data")]
        public void GetPreloadedEntityBody_When_Data_Is_Loaded_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();
            byte[] postData = new byte[] { 12, 56, 32, 98, 46 };

            using (mocks.Unordered()) {
                Expect.Call(request.PostData).Return(postData).Repeat.Any();
                Expect.Call(request.RawUrl).Return("/test/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual(postData, aspNetWorker.GetPreloadedEntityBody());
        }

        [Test]
        [Description("Determines that the GetProtocol method returns HTTPS when both Request and Response are secure")]
        public void GetProtocol_When_Request_And_Response_Are_Both_Secure_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(response.IsSecure).Return(true).Repeat.Any();
                Expect.Call(request.IsSecure).Return(true).Repeat.Any();
                Expect.Call(request.RawUrl).Return("/test/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual("HTTPS", aspNetWorker.GetProtocol());
        }

        [Test]
        [Description("Determines that the GetProtocol method returns HTTPS when both Request and Response are not secure")]
        public void GetProtocol_When_Request_And_Response_Are_Both_Not_Secure_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(response.IsSecure).Return(false).Repeat.Any();
                Expect.Call(request.IsSecure).Return(false).Repeat.Any();
                Expect.Call(request.RawUrl).Return("/test/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual("HTTP", aspNetWorker.GetProtocol());
        }

        [Test]
        [Description("Determines that the GetProtocol method returns HTTPS when both Request is secure and Response is not secure")]
        public void GetProtocol_When_Request_Is_Secure_And_Response_Is_Not_Secure_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(response.IsSecure).Return(false).Repeat.Any();
                Expect.Call(request.IsSecure).Return(true).Repeat.Any();
                Expect.Call(request.RawUrl).Return("/test/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual("HTTP", aspNetWorker.GetProtocol());
        }

        [Test]
        [Description("Determines that the GetProtocol method returns HTTPS when both Request is not secure and Response is secure")]
        public void GetProtocol_When_Request_Is_Not_Secure_And_Response_Is_Secure_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(response.IsSecure).Return(false).Repeat.Any();
                Expect.Call(request.IsSecure).Return(true).Repeat.Any();
                Expect.Call(request.RawUrl).Return("/test/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.AreEqual("HTTP", aspNetWorker.GetProtocol());
        }

        [Test]
        [Description("Determines that the HeadersSent method returns true when the response has sent headers.")]
        public void HeadersSent_Is_True_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(response.HeadersSent).Return(true).Repeat.Any();
                Expect.Call(request.RawUrl).Return("/test/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.IsTrue(aspNetWorker.HeadersSent());
        }

        [Test]
        [Description("Determines that the HeadersSent method returns false when the response has not sent headers.")]
        public void HeadersSent_Is_False_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(response.HeadersSent).Return(false).Repeat.Any();
                Expect.Call(request.RawUrl).Return("/test/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            Assert.IsFalse(aspNetWorker.HeadersSent());
        }

        [Test]
        [Description("Determiens that the FlushResponse method calls the Flush method of the Response object even when the parameter is false")]
        public void FlushResponse_Is_Not_Final_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/test/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
                Expect.Call(delegate { response.Flush(); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            aspNetWorker.FlushResponse(false);
        }

        [Test]
        [Description("Determiens that the FlushResponse method calls the Flush method of the Response object even when the parameter is true")]
        public void FlushResponse_Is_Final_Test() {
            MockRepository mocks = new MockRepository();
            IAspNetRuntime aspNetRuntime = mocks.CreateMock<IAspNetRuntime>();
            ITransaction transaction = mocks.CreateMock<ITransaction>();
            IResponse response = mocks.CreateMock<IResponse>();
            IRequest request = mocks.CreateMock<IRequest>();

            using (mocks.Unordered()) {
                Expect.Call(request.RawUrl).Return("/test/").Repeat.Any();
                Expect.Call(transaction.Request).Return(request).Repeat.Any();
                Expect.Call(transaction.Response).Return(response).Repeat.Any();
                Expect.Call(delegate { aspNetRuntime.ProcessRequest(null); }).IgnoreArguments();
                Expect.Call(delegate { response.Flush(); }).IgnoreArguments();
            }
            mocks.ReplayAll();

            AspNetWorker aspNetWorker = new AspNetWorker(aspNetRuntime, "/", @"c:\temp");

            aspNetWorker.ProcessTransaction(transaction);
            aspNetWorker.FlushResponse(true);
        }
    }
}
