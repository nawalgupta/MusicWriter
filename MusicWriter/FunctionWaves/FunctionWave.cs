using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FunctionWave : BoundObject<FunctionWave>
    {
        public const string ItemName = "musicwriter.function-waves.object";

        public ObservableProperty<float> SampleRate { get; } =
            new ObservableProperty<float>(44100);
        public ObservableProperty<int> BitsPerSample { get; } =
            new ObservableProperty<int>(32);
        public ObservableProperty<float> Start { get; } =
            new ObservableProperty<float>();
        public ObservableProperty<float> Length { get; } =
            new ObservableProperty<float>();

        public ObservableProperty<FunctionSource> FunctionSource { get; } =
            new ObservableProperty<FunctionSource>();

        readonly Dictionary<int, StorageObjectID> fragments =
            new Dictionary<int, StorageObjectID>();
        readonly Dictionary<int, long> fragments_sizes =
            new Dictionary<int, long>();

        public long TotalBytes {
            get { return BitsPerSample.Value / 8 * totalsamples; }
        }

        uint totalsamples = 0;

        readonly IStorageObject obj;
        readonly IOListener listener_contentsset;
        readonly IOListener listener_childadded;
        readonly IOListener listener_childremoved;
        readonly IOListener listener_childcontentsset;

        public uint TotalSamples {
            get { return (uint)(Length.Value * SampleRate.Value); }
        }

        public uint EndSample {
            get { return (uint)((double)(Start.Value + Length.Value) * SampleRate.Value); }
        }

        public FunctionWave(
                StorageObjectID storageobjectID, 
                EditorFile file
            ) :
            base(
                    storageobjectID, 
                    file, 
                    FactoryInstance
                ) {
            obj = file.Storage[storageobjectID];

            var functionsources =
                File
                    [FunctionContainer.ItemName]
                    .As<IContainer, FunctionContainer>()
                    .FunctionSources;
            
            listener_contentsset =
                obj
                    .CreateListen(
                            IOEvent.ObjectContentsSet,
                            Deserialize
                        );

            listener_childcontentsset =
                obj
                    .CreateListen(
                            IOEvent.ChildContentsSet,
                            (key, frag_objID) => {
                                int i;

                                if (int.TryParse(key, out i)) {
                                    fragments.Add(i, frag_objID);

                                    long size;
                                    using (var stream = file.Storage[frag_objID].OpenRead()) {
                                        size = stream.Length;
                                    }

                                    fragments_sizes.Add(i, size);
                                }
                            }
                        );

            listener_childadded =
                obj
                    .CreateListen(
                            IOEvent.ChildAdded,
                            (key, frag_objID) => {
                                if (key == "function") {
                                    FunctionSource.Value = functionsources[frag_objID];
                                }
                            }
                        );

            listener_childremoved =
                obj
                    .CreateListen(
                            IOEvent.ChildRemoved,
                            (key, frag_objID) => {
                                int i;

                                if (int.TryParse(key, out i)) {
                                    fragments.Remove(i);
                                    fragments_sizes.Remove(i);
                                }
                                else if (key == "function") {
                                    if (FunctionSource.Value?.StorageObjectID == frag_objID)
                                        FunctionSource.Value = null;
                                }
                            }
                        );
        }

        public override void Bind() {
            SampleRate.AfterChange += SampleRate_AfterChange;
            BitsPerSample.AfterChange += BitsPerSample_AfterChange;
            Start.AfterChange += Start_AfterChange;
            Length.AfterChange += Length_AfterChange;
            FunctionSource.AfterChange += FunctionSource_AfterChange;

            File.Storage.Listeners.Add(listener_contentsset);
            File.Storage.Listeners.Add(listener_childadded);
            File.Storage.Listeners.Add(listener_childremoved);
            File.Storage.Listeners.Add(listener_childcontentsset);

            base.Bind();
        }
        
        private void SampleRate_AfterChange(float old, float @new) {
            Serialize();
        }

        private void BitsPerSample_AfterChange(int old, int @new) {
            Serialize();
        }

        private void Start_AfterChange(float old, float @new) {
            Serialize();
        }

        private void Length_AfterChange(float old, float @new) {
            Serialize();
        }

        private void FunctionSource_AfterChange(FunctionSource old, FunctionSource @new) {
            obj.Set("function", @new);
        }

        void Serialize() {
            using (var writer = new BinaryWriter(obj.OpenWrite())) {
                writer.Write(SampleRate.Value);
                writer.Write(BitsPerSample.Value);
                writer.Write(Start.Value);
                writer.Write(Length.Value);
            }

            totalsamples = TotalSamples;
        }

        void Deserialize() {
            using (var reader = new BinaryReader(obj.OpenRead())) {
                SampleRate.Value = reader.ReadSingle();
                BitsPerSample.Value = reader.ReadInt32();
                Start.Value = reader.ReadSingle();
                Length.Value = reader.ReadSingle();
            }

            totalsamples = TotalSamples;
        }

        public override void Unbind() {
            SampleRate.AfterChange -= SampleRate_AfterChange;
            BitsPerSample.AfterChange -= BitsPerSample_AfterChange;
            Start.AfterChange -= Start_AfterChange;
            Length.AfterChange -= Length_AfterChange;
            FunctionSource.AfterChange -= FunctionSource_AfterChange;

            File.Storage.Listeners.Remove(listener_contentsset);
            File.Storage.Listeners.Remove(listener_childadded);
            File.Storage.Listeners.Remove(listener_childremoved);
            File.Storage.Listeners.Remove(listener_childcontentsset);

            base.Unbind();
        }

        public Stream OpenRead() =>
            new PrivateStream(this);

        public static IFactory<FunctionWave> FactoryInstance { get; } =
            new CtorFactory<FunctionWave, FunctionWave>(
                    ItemName,
                    false
                );

        private class PrivateStream : Stream {
            long current_offset_global;
            long current_offset;
            long current_length;
            Stream current_stream;
            int current_i;

            readonly FunctionWave wave;

            internal PrivateStream(FunctionWave wave) {
                this.wave = wave;
            }

            public override bool CanRead {
                get { return current_stream.CanRead || (current_i != wave.fragments_sizes.Count - 1); }
            }

            public override bool CanSeek {
                get { return wave.fragments_sizes.Count != 0; }
            }

            public override bool CanWrite {
                get { return false; }
            }

            public override long Length {
                get { return wave.TotalBytes; }
            }

            public override long Position {
                get { return current_offset_global + current_offset; }
                set { Seek(value, SeekOrigin.Begin); }
            }

            public override void Flush() {
                throw new InvalidOperationException();
            }

            public override int Read(byte[] buffer, int offset, int count) {
                EnsureOneFragmentIsLoaded();

                if (current_offset + current_length == wave.TotalBytes)
                    return 0;

                if (count <= current_length - current_offset) {
                    current_stream.Read(buffer, offset, count);
                    current_offset += count;
                }
                else {
                    var readsize = 0;

                    while (count > 0) {
                        var local_count = Math.Min(count, (int)(current_length - current_offset));

                        var read = current_stream.Read(buffer, offset, local_count);
                        if (read != local_count)
                            throw new InvalidOperationException();

                        readsize += local_count;
                        offset += local_count;
                        current_offset += local_count;

                        if (current_offset == current_length) {
                            if (current_offset_global + current_length == wave.TotalBytes)
                                break;

                            current_stream.Close();
                            current_stream.Dispose();
                            current_i++;
                            current_offset_global += current_length;

                            WaitForFragmentToLoad(current_i);

                            current_stream = wave.File.Storage[wave.fragments[current_i]].OpenRead();
                            current_length = wave.fragments_sizes[current_i];
                            current_offset = 0;
                        }

                        count -= local_count;
                    }
                    
                    return readsize;
                }

                return 0;
            }

            public override long Seek(long offset, SeekOrigin origin) {
                long position;

                EnsureOneFragmentIsLoaded();

                switch (origin) {
                    case SeekOrigin.Begin:
                        position = offset;
                        break;
                    case SeekOrigin.Current:
                        position = offset + Position;
                        break;
                    case SeekOrigin.End:
                        position = Length - offset;
                        break;
                    default:
                        position = 0;
                        break;
                }

                if (position >= wave.totalsamples)
                    position = wave.totalsamples - 1;

                if (position < current_offset_global) {
                    current_stream.Close();
                    current_stream.Dispose();

                    while (position < current_offset_global) {
                        WaitForFragmentToLoad(current_i - 1);
                        current_offset_global -= current_length = wave.fragments_sizes[--current_i];
                    }
                    current_stream = wave.File.Storage[wave.fragments[current_i]].OpenRead();
                }
                else if (position >= current_offset_global + current_length) {
                    current_stream.Close();
                    current_stream.Dispose();

                    while (position >= current_offset_global + current_length) {
                        if (current_offset_global + current_length == wave.TotalBytes)
                            break;

                        current_offset_global += current_length;
                        current_i++;
                        WaitForFragmentToLoad(current_i);
                        current_length = wave.fragments_sizes[current_i];
                    }
                    current_stream = wave.File.Storage[wave.fragments[current_i]].OpenRead();
                }

                current_offset = position - current_offset_global;
                current_stream.Seek(current_offset, SeekOrigin.Begin);

                return position;
            }

            void WaitForFragmentToLoad(int i) {
                while (!wave.fragments.ContainsKey(i))
                    Thread.Sleep(20);
            }

            void EnsureOneFragmentIsLoaded() {
                if (current_stream == null) {
                    WaitForFragmentToLoad(0);

                    current_stream = wave.File.Storage[wave.fragments[0]].OpenRead();
                    current_length = wave.fragments_sizes[0];
                    current_offset = 0;
                    current_offset_global = 0;
                    current_i = 0;
                }
            }

            public override void SetLength(long value) {
                throw new InvalidOperationException();
            }

            public override void Write(byte[] buffer, int offset, int count) {
                throw new InvalidOperationException();
            }
        }
    }
}
