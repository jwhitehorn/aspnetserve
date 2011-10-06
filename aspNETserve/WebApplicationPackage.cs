using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Utils.Zip;

namespace aspNETserve {
    /// <summary>
    /// Represents a WebApplicationPackage archive.
    /// </summary>
    public class WebApplicationPackage : IDisposable {
        private bool _isOpen;
        private readonly bool _ownsStream;
        private readonly Stream _stream;
        private const int _bufferSize = 4096;
        private string _extractedPackagePath;

        /// <summary>
        /// Constructs an instance of WebApplicationPackage.
        /// This constructor assumes ownership of the stream, and will
        /// close it when the WebApplicationPackage is disposed.
        /// </summary>
        /// <param name="stream">A Stream containing the WAP archive.</param>
        public WebApplicationPackage(Stream stream) : this(stream, true) {}

        /// <summary>
        /// Constructs an instance of WebApplicationPackage.
        /// </summary>
        /// <param name="stream">A Stream containing the WAP archive.</param>
        /// <param name="ownsStream">If true the stream will be closed when the 
        /// instance of WebApplicationPackage is disposed, otherwise it will be left open.
        /// </param>
        public WebApplicationPackage(Stream stream, bool ownsStream) {
            _stream = stream;
            _ownsStream = ownsStream;
        }

        /// <summary>
        /// releases the unmanaged resources allocated by the current
        /// WebApplicationPackage instance.
        /// </summary>
        public virtual void Dispose() {
            if(_ownsStream)
                _stream.Close();
            if(IsOpen)
                DeleteDirectory(_extractedPackagePath);
            IsOpen = false;
        }

        /// <summary>
        /// Opens the WebApplicationPackage for reading.
        /// </summary>
        public virtual void Open() {
            if(IsOpen)
                throw new Exception("Cannot open WAP, it is already opened.");
            IsOpen = true;
            string directory = (Guid.NewGuid()).ToString();
            _extractedPackagePath = Path.Combine(Path.GetTempPath(), directory);
            Directory.CreateDirectory(_extractedPackagePath);

            ExtractWapToPath(_extractedPackagePath);
        }

        /// <summary>
        /// Returns the path on disk to the contents
        /// of an opened WebApplicationPackage.
        /// </summary>
        public virtual string PhysicalPath {
            get {
                if(!IsOpen)
                    throw new Exception("The WAP is not opened yet.");
                return Path.Combine(_extractedPackagePath, "site");    //all site data is located within a sub directory called site
            }
        }

        /// <summary>
        /// Determines if the WebApplicationPackage has been opened
        /// </summary>
        public virtual bool IsOpen {
            get { return _isOpen; }
            protected set { _isOpen = value; }
        }

        protected virtual void ExtractWapToPath(string path) {
            using(ZipFile zipFile = new ZipFile(_stream)) {
                zipFile.ExtractAll(path);
            }
        }

        protected virtual void DeleteDirectory(string path) {
            string[] files = Directory.GetFiles(path);
            foreach(string file in files) {
                File.Delete(file);
            }

            string[] subDirs = Directory.GetDirectories(path);
            foreach(string subDir in subDirs) {
                DeleteDirectory(subDir);
            }
            Directory.Delete(path);
        }
    }
}
