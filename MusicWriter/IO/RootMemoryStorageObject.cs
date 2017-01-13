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
            readonly Dictionary<string, HashSet<StorageObjectID>> branches =
                new Dictionary<string, HashSet<StorageObjectID>>();
            readonly Dictionary<StorageObjectID, HashSet<string>> branches_inverse =
                new Dictionary<StorageObjectID, HashSet<string>>();

            public StorageObjectID this[string key] {
                get {
                    try {
                        return branches[key].Single();
                    }
                    catch (InvalidOperationException) {
                        throw new KeyNotFoundException();
                    }
                }
            }

            public IEnumerable<StorageObjectID> Children {
                get { return branches.Values.SelectMany(v => v).Distinct(); }
            }

            public IEnumerable<KeyValuePair<string, StorageObjectID>> RelationalChildren {
                get { return branches.SelectMany(kvp => kvp.Value.Select(child => new KeyValuePair<string, StorageObjectID>(kvp.Key, child))); }
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

            readonly List<StorageObjectChildChangedDelegate> ChildAdded_responders = new List<StorageObjectChildChangedDelegate>();
            public event StorageObjectChildChangedDelegate ChildAdded {
                add {
                    ChildAdded_responders.Add(value);

                    foreach (var branchset in branches)
                        foreach (var branch in branchset.Value)
                            value(ID, branch, branchset.Key);
                }
                remove {
                    ChildAdded_responders.Remove(value);
                }
            }
            readonly List<StorageObjectChildChangedDelegate> ChildContentsSet_responders = new List<StorageObjectChildChangedDelegate>();
            public event StorageObjectChildChangedDelegate ChildContentsSet {
                add {
                    ChildContentsSet_responders.Add(value);

                    foreach (var branchset in branches)
                        foreach (var branch in branchset.Value)
                            value(ID, branch, branchset.Key);
                }
                remove {
                    ChildContentsSet_responders.Remove(value);
                }
            }
            public event StorageObjectChildChangedDelegate ChildRemoved;
            public event StorageObjectChildRekeyedDelegate ChildRenamed;
            public event StorageObjectChangedDelegate ContentsSet {
                add { }
                remove { }
            }
            public event StorageObjectChangedDelegate Deleted {
                add { }
                remove { }
            }

            public RootMemoryStorageObject(MemoryStorageGraph graph) {
                this.graph = graph;

                graph.ArrowAdded += (sourceID, sinkID, key) => {
                    if (sourceID == ID) {
                        foreach (var responder in ChildAdded_responders)
                            responder(sourceID, sinkID, key);

                        branches.Lookup(key).Add(sinkID);
                        branches_inverse.Lookup(sinkID).Add(key);
                    }
                };

                graph.ArrowRenamed += (sourceID, sinkID, oldkey, newkey) => {
                    if (sourceID == ID) {
                        ChildRenamed?.Invoke(sourceID, sinkID, oldkey, newkey);

                        branches.Lookup(oldkey).Remove(sinkID);
                        branches.Lookup(newkey).Add(sinkID);

                        branches_inverse.Lookup(sinkID).Remove(oldkey);
                        branches_inverse.Lookup(sinkID).Add(newkey);
                    }
                };

                graph.ArrowRemoved += (sourceID, sinkID, key) => {
                    if (sourceID == ID) {
                        ChildRemoved?.Invoke(sourceID, sinkID, key);

                        branches.Lookup(key).Remove(sinkID);
                        branches_inverse.Lookup(sinkID).Remove(key);
                    }
                };

                graph.NodeContentsSet += (nodeID) => {
                    var keys =
                        branches_inverse.Lookup(nodeID);

                    foreach (var key in keys)
                        foreach (var responder in ChildContentsSet_responders)
                            responder(ID, nodeID, key);
                };
            }

            public void Add(string key, StorageObjectID id) =>
                graph.AddArrow(ID, id, key);

            public void Delete() {
                throw new InvalidOperationException();
            }

            public string GetRelation(StorageObjectID child) =>
                branches_inverse[child].SingleOrDefault();

            public IEnumerable<string> GetRelations(StorageObjectID child) =>
                branches_inverse[child];

            public bool HasChild(string relation) =>
                branches.ContainsKey(relation);

            public IStorageObject Open(string child) =>
                graph[branches[child].SingleOrDefault()];

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
                graph.RemoveArrow(ID, branches[key].SingleOrDefault());

            public void Rename(StorageObjectID child, string newkey) =>
                graph.RenameArrow(ID, child, newkey);

            public void Rename(string oldkey, string newkey) =>
                graph.RenameArrow(
                        ID,
                        branches[oldkey].SingleOrDefault(),
                        newkey
                    );
        }
    }
}
