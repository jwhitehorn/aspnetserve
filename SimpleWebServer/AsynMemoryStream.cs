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
using System.Text;

namespace SimpleWebServer {
    public class AsynMemoryStream : MemoryStream {
        private event EventHandler _contentsUpdated;

        public override void Write(byte[] buffer, int offset, int count) {
            base.Write(buffer, offset, count);

            if (_contentsUpdated != null)
                _contentsUpdated(this, new DataWriteEventArgs(buffer, offset, count));
        }

        public override void WriteByte(byte value) {
            base.WriteByte(value);

            if (_contentsUpdated != null)
                _contentsUpdated(this, new DataWriteEventArgs(new byte[] { value }));
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
            return base.BeginWrite(buffer, offset, count,
                delegate(IAsyncResult ar) {
                    callback(ar);
                    if (_contentsUpdated != null) {
                        _contentsUpdated(this, new DataWriteEventArgs(buffer, offset, count));
                    }
                }, state);
        }

        public event EventHandler ContentsUpdated {
            add { _contentsUpdated += value; }
            remove { _contentsUpdated -= value; }
        }
    }
}
