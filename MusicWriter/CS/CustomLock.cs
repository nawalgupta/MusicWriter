using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class CustomLock
    {
        volatile int i = 0;

        public void Lock() {
            do {
                while (i != 0)
                    Thread.Sleep(1);

                i++;

                Thread.Sleep(1);

                if (i != 1)
                    i--;
                else return;
            }
            while (true);
        }

        public void Unlock() {
            i--;
        }

        public Acquisition Acquire() =>
            new Acquisition(this);

        public class Acquisition : IDisposable
        {
            readonly CustomLock @lock;

            internal Acquisition(CustomLock @lock) {
                this.@lock = @lock;
                @lock.Lock();
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing) {
                if (!disposedValue) {
                    if (disposing) {
                        @lock.Unlock();
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            ~Acquisition() {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(false);
            }

            // This code added to correctly implement the disposable pattern.
            public void Dispose() {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion
        }
    }
}
