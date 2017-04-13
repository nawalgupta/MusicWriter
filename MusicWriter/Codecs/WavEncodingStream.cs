using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class WavEncodingStream : Stream
    {
        readonly FunctionWave wave;
        readonly Stream wavestream;
        long position = 0;
        const int HEADER_SIZE = 32; //??

        public override bool CanRead {
            get { return true; }
        }

        public override bool CanSeek {
            get { return false; }
        }

        public override bool CanWrite {
            get { return false; }
        }

        public override long Length {
            get { return wave.TotalSamples + HEADER_SIZE; }
        }

        public override long Position {
            get { return position; }
            set { Seek(value, SeekOrigin.Begin); }
        }

        public WavEncodingStream(FunctionWave wave) {
            this.wave = wave;
            wavestream = wave.OpenRead();
        }

        public override void Flush() {
            throw new InvalidOperationException();
        }

        public override int Read(byte[] buffer, int offset, int count) {
            var startcount = count;

            if (position < HEADER_SIZE) {
                // write header

                throw new NotImplementedException();

                if (position >= HEADER_SIZE)
                    wavestream.Seek(position - HEADER_SIZE, SeekOrigin.Begin);
            }

            if (position >= HEADER_SIZE) {
                count -= wavestream.Read(buffer, offset, count);
                position += count;
            }

            return startcount - count;
        }

        public override long Seek(long offset, SeekOrigin origin) {
            long value = 0;

            switch (origin) {
                case SeekOrigin.Begin:
                    value = offset;
                    break;

                case SeekOrigin.Current:
                    value = offset + position;
                    break;

                case SeekOrigin.End:
                    value = HEADER_SIZE + wavestream.Length - offset;
                    break;
            }

            if (position >= HEADER_SIZE || 
                value >= HEADER_SIZE)
                wavestream.Seek(value - HEADER_SIZE, SeekOrigin.Begin);

            return position = value;
        }

        public override void SetLength(long value) {
            throw new InvalidOperationException();
        }

        public override void Write(byte[] buffer, int offset, int count) {
            throw new InvalidOperationException();
        }
    }
}
