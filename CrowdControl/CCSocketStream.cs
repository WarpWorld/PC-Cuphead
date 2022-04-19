using System;
using System.IO;
using System.Net.Sockets;

namespace WarpWorld.CrowdControl {
    /// <summary>Handles sending and receiving byte streams from the server.</summary>
    class SocketStream : Stream {
        readonly Socket socket;

        bool disposed;

        public SocketStream(Socket socket) {
            this.socket = socket;
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing && !disposed) {
                if (socket.Connected) {
                    socket.Shutdown(SocketShutdown.Both);
                }
                socket.Close();
                disposed = true;
            }
        }

        public override bool CanSeek => false;
        public override bool CanRead => !disposed;
        public override bool CanWrite => !disposed;

        public override long Length => throw new NotSupportedException();

        public override long Position {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();
        public override void SetLength(long value) => throw new NotImplementedException();

        public override void Flush() {}

        public override int Read(byte[] buffer, int offset, int count) {
            if (disposed) throw new ObjectDisposedException(GetType().FullName);

            if (socket.Available == 0) return 0;
            return socket.Receive(buffer, offset, count, SocketFlags.None);
        }

        public override void Write(byte[] buffer, int offset, int count) {
            if (disposed) throw new ObjectDisposedException(GetType().FullName);

            socket.Send(buffer, offset, count, SocketFlags.None);
        }
    }
}
