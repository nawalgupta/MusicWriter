using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    partial class MemoryStorageGraph
    {
        public sealed class RootMemoryStorageObject : IStorageObject
        {
            readonly MemoryStorageGraph graph;

            public StorageObjectID this[string key] {
                get {
                    return graph.Outgoing(ID).First(_ => _.Key == key).Value;
                }
            }

            public IEnumerable<StorageObjectID> Children {
                get { return graph.Outgoing(ID).Select(_ => _.Value); }
            }

            public IEnumerable<KeyValuePair<string, StorageObjectID>> RelationalChildren {
                get { return graph.Outgoing(ID); }
            }

            public IStorageGraph Graph {
                get { return graph; }
            }

            public StorageObjectID ID {
                get { return StorageObjectID.Zero; }
            }

            public bool IsEmpty {
                get { return true; }
            }

            public RootMemoryStorageObject(MemoryStorageGraph graph) {
                this.graph = graph;
            }

            public void Add(string key, StorageObjectID id) =>
                graph.AddArrow(ID, id, key);

            public void Delete() {
                throw new InvalidOperationException();
            }

            public string GetRelation(StorageObjectID child) =>
                graph.GetRelation(ID, child);

            public IEnumerable<string> GetRelations(StorageObjectID child) =>
                graph.GetRelations(ID, child);

            public bool HasChild(string relation) =>
                graph.HasChild(ID, relation);

            public bool HasChild(StorageObjectID storageobjectID) =>
                graph.HasChild(ID, storageobjectID);

            public IStorageObject Open(string child) =>
                graph[this[child]];

            private sealed class EmptyStream : Stream
            {
                public override bool CanRead {
                    get { return true; }
                }

                public override bool CanSeek {
                    get { return true; }
                }

                public override bool CanWrite {
                    get { return true; }
                }

                public override long Length {
                    get { return 0; }
                }

                public override long Position {
                    get { return 0; }
                    set { throw new IOException(); }
                }

                public override void Flush() {
                }

                public override int Read(byte[] buffer, int offset, int count) {
                    return 0;
                }

                public override long Seek(long offset, SeekOrigin origin) {
                    if (offset != 0)
                        throw new IOException();

                    return 0;
                }

                public override void SetLength(long value) {
                    if (value != 0)
                        throw new IOException();
                }

                public override void Write(byte[] buffer, int offset, int count) {
                    if (count > 0)
                        throw new IOException();
                }
            }

            public Stream OpenRead() =>
                new EmptyStream();

            public Stream OpenWrite() =>
                new EmptyStream();

            public void Remove(StorageObjectID child) =>
                graph.RemoveArrow(ID, child);

            public void Remove(string key) =>
                graph.RemoveArrow(ID, this[key]);

            public void Rename(StorageObjectID child, string newkey) =>
                graph.RenameArrow(ID, child, newkey);

            public void Rename(string oldkey, string newkey) =>
                graph.RenameArrow(
                        ID,
                        this[oldkey],
                        newkey
                    );
        }
    }
}
