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
        readonly FunctionWave[] waves;
        readonly Stream[] wavestreams;
        readonly byte[] header;
        const int HEADER_SIZE = 36;

        readonly float totallength;
        readonly int bitspersample;
        readonly int bytespersample;
        readonly int numchannels;
        readonly int numsamples;
        readonly int byterate;
        readonly int numdatabytes;
        readonly int samplerate;
        readonly int bytesperunit;

        // a unit has multiple channel's samples in it.
        
        long position = 0;

        public override bool CanRead {
            get { return position != Length; }
        }

        public override bool CanSeek {
            get { return true; }
        }

        public override bool CanWrite {
            get { return false; }
        }

        public override long Length {
            get { return numdatabytes + HEADER_SIZE; }
        }

        public override long Position {
            get { return position; }
            set { Seek(value, SeekOrigin.Begin); }
        }

        public WavEncodingStream(params FunctionWave[] waves) {
            this.waves = waves;
            wavestreams = waves.Select(wave => wave.OpenRead()).ToArray();

            header = new byte[HEADER_SIZE];

            totallength = waves[0].Length.Value;
            if (!waves.All(wave => (int)wave.Length.Value == totallength))
                throw new InvalidOperationException();

            samplerate = (int)waves[0].SampleRate.Value;
            if (!waves.All(wave => (int)wave.SampleRate.Value == samplerate))
                throw new InvalidOperationException();

            bitspersample = waves[0].BitsPerSample.Value;
            if (!waves.All(wave => wave.BitsPerSample.Value == bitspersample))
                throw new InvalidOperationException();

            bytespersample = bitspersample / 8;

            bytesperunit = numchannels * bytespersample;
            numchannels = waves.Length;
            numsamples = (int)(totallength * samplerate);
            byterate = samplerate * bytesperunit;
            numdatabytes = (int)(totallength * byterate);

            using (var memorystream = new MemoryStream(header)) {
                using (var bw = new BinaryWriter(memorystream)) {
                    bw.Write((byte)0x52); // R
                    bw.Write((byte)0x49); // I
                    bw.Write((byte)0x46); // F
                    bw.Write((byte)0x46); // F

                    bw.Write(36 + numdatabytes);

                    bw.Write((byte)0x57); // W
                    bw.Write((byte)0x41); // A
                    bw.Write((byte)0x56); // V
                    bw.Write((byte)0x45); // E

                    bw.Write((byte)0x66); // f
                    bw.Write((byte)0x6d); // m
                    bw.Write((byte)0x74); // t
                    bw.Write((byte)0x20); //` '

                    bw.Write(16); // chunk size

                    bw.Write((ushort)1); // PCM

                    bw.Write((ushort)numchannels);

                    bw.Write(samplerate);

                    bw.Write(byterate); // byte rate

                    bw.Write((ushort)(numchannels * bitspersample / 8));

                    bw.Write((ushort)bitspersample);

                    bw.Write((byte)0x64); // d
                    bw.Write((byte)0x61); // a
                    bw.Write((byte)0x74); // t
                    bw.Write((byte)0x61); // a

                    bw.Write(numdatabytes);
                }
            }
        }

        public override void Flush() {
            throw new InvalidOperationException();
        }

        public override int Read(byte[] buffer, int offset, int count) {
            var startcount = count;

            if (position < HEADER_SIZE) {
                // write header

                var readcount = (int)Math.Min(count, HEADER_SIZE - position);

                Array.Copy(header, position, buffer, offset, readcount);

                position += readcount;
                count -= readcount;

                if (position >= HEADER_SIZE)
                    for(int i = numchannels - 1; i >= 0;i--)
                        wavestreams[i].Seek(position - HEADER_SIZE, SeekOrigin.Begin);
            }

            if (position >= HEADER_SIZE) {
                Task[] tasks = new Task[numchannels];
                int[] readcounts = new int[numchannels];

                var unitbytes = (int)((position - HEADER_SIZE) % bytesperunit);
                var position_unit_cieling = position;
                if (unitbytes > 0)
                    position_unit_cieling += bytesperunit;
                var count_unit_cieling = (int)(count - (position_unit_cieling - position));

                for (int i = numchannels - 1; i >= 0; i--)
                    tasks[i] =
                        Task.Run(
                                new Func<int, Action>(j => () => {
                                    var wavestream = wavestreams[j];
                                    int read;
                                    int attemptedtoread;
                                    int channelbytesoffset;
                                    var channel = Math.DivRem(unitbytes, bytespersample, out channelbytesoffset);
                                    var localoffset = offset;

                                    int localcount = 0;

                                    if (channel == j) {
                                        attemptedtoread = bytespersample - channelbytesoffset;
                                        if (attemptedtoread > count)
                                            attemptedtoread = count;

                                        read = wavestream.Read(buffer, localoffset, attemptedtoread);

                                        if (read != attemptedtoread) {
                                            readcounts[j] = read;
                                            return;
                                        }

                                        localcount += read;
                                        localoffset += read;
                                        localoffset += bytesperunit;
                                    }

                                    var units_whole =
                                        count_unit_cieling / bytesperunit;

                                    var unitbytes_endpart =
                                        count_unit_cieling % bytesperunit;

                                    var lefttoread = units_whole * bytespersample;

                                    if (unitbytes_endpart > j * bytespersample)
                                        lefttoread += Math.Min(unitbytes_endpart - j * bytespersample, bytespersample);

                                    while (lefttoread > 0) {
                                        attemptedtoread = Math.Min(bytespersample, lefttoread);

                                        read = wavestream.Read(buffer, localoffset, attemptedtoread);

                                        localcount += read;
                                        lefttoread -= read;
                                        localoffset += bytesperunit;

                                        if (read != attemptedtoread)
                                            break;
                                    }

                                    readcounts[j] = localcount;
                                })(i)
                            );

                Task.WaitAll(tasks);

                position += readcounts.Sum();

                if (readcounts.Sum() != count)
                    if (readcounts.Zip(readcounts.Skip(1), (a, b) => a > b).Any(_ => !_))
                        throw new InvalidOperationException("One of the streams was empty before the later one.");
            }

            return startcount - count;
        }

        public int RecommendedBufferSize(int units) =>
            units * bytesperunit;

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
                    value = HEADER_SIZE + numdatabytes - offset;
                    break;
            }

            if (position >= HEADER_SIZE ||
                value >= HEADER_SIZE)
                for (int i = 0; i < numchannels; i++)
                    wavestreams[i]
                        .Seek(
                                (value - HEADER_SIZE) / bytesperunit * bytespersample +
                                    Math.Max(
                                            bytespersample,
                                            Math.Min(
                                                    0,
                                                    (value - HEADER_SIZE) % bytesperunit
                                                        - bytespersample * i
                                                )
                                        ),
                                SeekOrigin.Begin
                            );

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
