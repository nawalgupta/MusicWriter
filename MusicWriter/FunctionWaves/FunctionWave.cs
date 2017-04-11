using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FunctionWave : BoundObject<FunctionWave>
    {
        public const string ItemName = "musicwriter.function-waves.object";

        public ObservableProperty<float> SamplesPerUnit { get; } =
            new ObservableProperty<float>(44100);

        public ObservableProperty<FunctionSource> FunctionSource { get; } =
            new ObservableProperty<FunctionSource>();

        readonly Dictionary<int, StorageObjectID> fragments =
            new Dictionary<int, StorageObjectID>();

        readonly Dictionary<int, long> fragments_sizes =
            new Dictionary<int, long>();
        
        readonly IStorageObject obj;
        readonly IOListener listener_contentsset;
        readonly IOListener listener_childadded;
        readonly IOListener listener_childremoved;

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
                            () => {
                                using (var reader = new BinaryReader(obj.OpenRead())) {
                                    SamplesPerUnit.Value = reader.ReadSingle();
                                }
                            }
                        );

            listener_childadded =
                obj
                    .CreateListen(
                            IOEvent.ChildAdded,
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
                                else if (key == "function") {
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
                                    if (FunctionSource.Value.StorageObjectID == frag_objID)
                                        FunctionSource.Value = null;
                                }
                            }
                        );
        }

        public override void Bind() {
            SamplesPerUnit.AfterChange += SamplesPerUnit_AfterChange;
            FunctionSource.AfterChange += FunctionSource_AfterChange;

            File.Storage.Listeners.Add(listener_contentsset);
            File.Storage.Listeners.Add(listener_childadded);
            File.Storage.Listeners.Add(listener_childremoved);

            base.Bind();
        }

        private void FunctionSource_AfterChange(FunctionSource old, FunctionSource @new) {
            obj.Set("function", @new);
        }

        private void SamplesPerUnit_AfterChange(float old, float @new) {
            using (var writer = new BinaryWriter(obj.OpenWrite())) {
                writer.Write(SamplesPerUnit.Value);
            }
        }

        public override void Unbind() {
            SamplesPerUnit.AfterChange -= SamplesPerUnit_AfterChange;
            FunctionSource.AfterChange -= FunctionSource_AfterChange;

            File.Storage.Listeners.Remove(listener_contentsset);
            File.Storage.Listeners.Remove(listener_childadded);
            File.Storage.Listeners.Remove(listener_childremoved);

            base.Unbind();
        }

        public Stream OpenRead() =>
            new PrivateStream(this);

        public static IFactory<FunctionWave> FactoryInstance { get; } =
            new CtorFactory<FunctionWave, FunctionWave>(
                    ItemName,
                    false
                );

        private class PrivateStream : Stream
        {
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
                get { return wave.fragments_sizes.Values.Sum(); }
            }

            public override long Position {
                get { return current_offset_global + current_offset; }
                set { Seek(value, SeekOrigin.Begin); }
            }

            public override void Flush() {
                throw new InvalidOperationException();
            }

            public override int Read(byte[] buffer, int offset, int count) {
                if (count <= current_length - current_offset) {
                    current_stream.Read(buffer, offset, count);
                    current_offset += count;
                }
                else {
                    var part_count = (int)(current_length - current_offset);

                    current_stream.Read(buffer, offset, part_count);
                    current_stream.Close();
                    current_stream.Dispose();
                    if (current_i + 1 == wave.fragments.Count)
                        return part_count;

                    count -= part_count;

                    current_offset_global += current_length;
                    current_i++;
                    current_stream = wave.File.Storage[wave.fragments[current_i]].OpenRead();
                    current_length = wave.fragments_sizes[current_i];
                    current_offset = count;

                    current_stream.Read(buffer, 0, count);

                    return count + part_count;
                }

                return 0;
            }

            public override long Seek(long offset, SeekOrigin origin) {
                long position;

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

                if (position < current_offset_global) {
                    current_stream.Close();
                    current_stream.Dispose();

                    while (position < current_offset_global)
                        current_offset_global -= current_length = wave.fragments_sizes[--current_i];
                    current_stream = wave.File.Storage[wave.fragments[current_i]].OpenRead();
                }
                else if (position >= current_offset_global + current_length) {
                    current_stream.Close();
                    current_stream.Dispose();

                    while (position >= current_offset_global + current_length) {
                        current_offset_global += current_length;
                        current_length = wave.fragments_sizes[++current_i];
                    }
                    current_stream = wave.File.Storage[wave.fragments[current_i]].OpenRead();
                }

                current_offset = position - current_offset_global;
                current_stream.Seek(current_offset, SeekOrigin.Begin);

                return position;
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
