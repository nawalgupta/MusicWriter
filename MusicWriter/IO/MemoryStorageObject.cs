﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    partial class MemoryStorageGraph : IStorageGraph {
        public class StorageObject : IStorageObject
        {
            class MemoryFile
            {
                public byte[] data = new byte[0];

                internal event Action Written;

                internal void NotifyWritten() =>
                    Written?.Invoke();

                class FileStream : Stream
                {
                    readonly MemoryFile file;
                    long pos = 0;
                    long maxlength = 0;
                    bool writable;

                    internal FileStream(MemoryFile file, bool writable) {
                        this.file = file;
                        this.writable = writable;
                    }

                    public override bool CanRead {
                        get { return true; }
                    }

                    public override bool CanSeek {
                        get { return true; }
                    }

                    public override bool CanWrite {
                        get { return writable; }
                    }

                    public override long Length {
                        get { return file.data.LongLength; }
                    }

                    public override long Position {
                        get { return pos; }
                        set {
                            pos = value;

                            if (pos > maxlength)
                                maxlength = pos;
                        }
                    }

                    public override void Flush() {
                        if (!writable)
                            throw new InvalidOperationException();

                        if (maxlength != file.data.LongLength) {
                            Array.Resize(ref file.data, (int)maxlength);
                        }

                        file.NotifyWritten();
                    }

                    public override int Read(byte[] buffer, int offset, int count) {
                        if (writable)
                            throw new InvalidOperationException();

                        if (pos + count > file.data.LongLength)
                            count = (int)(file.data.LongLength - pos);

                        Array.Copy(file.data, pos, buffer, offset, count);
                        pos += count;

                        return count;
                    }

                    public override long Seek(long offset, SeekOrigin origin) {
                        switch (origin) {
                            case SeekOrigin.Begin:
                                pos = offset;
                                break;

                            case SeekOrigin.Current:
                                pos += offset;
                                break;

                            case SeekOrigin.End:
                                if (writable)
                                    pos = maxlength - offset;
                                else {
                                    pos = file.data.LongLength - offset;
                                }

                                break;

                            default:
                                return -1;
                        }

                        if (pos > maxlength)
                            maxlength = pos;

                        return pos;
                    }

                    public override void SetLength(long value) {
                        if (!writable)
                            throw new InvalidOperationException();

                        Array.Resize(ref file.data, (int)value);

                        if (value > maxlength)
                            maxlength = value;
                    }

                    public override void Write(byte[] buffer, int offset, int count) {
                        if (!writable)
                            throw new InvalidOperationException();

                        if (pos + count > file.data.LongLength)
                            Array.Resize(ref file.data, (int)(pos + count));

                        Array.Copy(buffer, offset, file.data, pos, count);
                        pos += count;

                        if (pos > maxlength)
                            maxlength = pos;
                    }
                }

                public Stream OpenRead() =>
                    new FileStream(this, false);

                public Stream OpenWrite() =>
                    new FileStream(this, true);
            }

            readonly MemoryFile filedata;
            readonly MemoryStorageGraph graph;
            readonly StorageObjectID id;

            readonly List<StorageObjectChildChangedDelegate> ChildAdded_responders = new List<StorageObjectChildChangedDelegate>();
            public event StorageObjectChildChangedDelegate ChildAdded {
                add {
                    ChildAdded_responders.Add(value);

                    foreach (var child in RelationalChildren.ToArray())
                        value(id, child.Value, child.Key);
                }
                remove {
                    ChildAdded_responders.Remove(value);
                }
            }
            public event StorageObjectChildChangedDelegate ChildRemoved;
            public event StorageObjectChildRekeyedDelegate ChildRenamed;

            readonly List<StorageObjectChildChangedDelegate> ChildContentsSet_responders = new List<StorageObjectChildChangedDelegate>();
            public event StorageObjectChildChangedDelegate ChildContentsSet {
                add {
                    ChildContentsSet_responders.Add(value);

                    foreach (var child in RelationalChildren.ToArray())
                        value(id, child.Value, child.Key);
                }
                remove {
                    ChildContentsSet_responders.Remove(value);
                }
            }

            readonly List<StorageObjectChangedDelegate> ContentsSet_responders = new List<StorageObjectChangedDelegate>(); 
            public event StorageObjectChangedDelegate ContentsSet {
                add {
                    ContentsSet_responders.Add(value);

                    value(id);
                }
                remove {
                    ContentsSet_responders.Remove(value);
                }
            }
            public event StorageObjectChangedDelegate Deleted;

            public IStorageGraph Graph {
                get { return graph; }
            }

            public StorageObjectID ID {
                get { return id; }
            }

            public IEnumerable<StorageObjectID> Children {
                get { return graph.Outgoing(id).Select(child => child.Value); }
            }

            public IEnumerable<KeyValuePair<string, StorageObjectID>> RelationalChildren {
                get { return graph.Outgoing(id); }
            }

            public bool IsEmpty {
                get { return filedata.data.Length == 0; }
            }

            public StorageObjectID this[string key] {
                get {
                    foreach (var kvp in graph.Outgoing(id))
                        if (kvp.Key == key)
                            return kvp.Value;

                    throw new KeyNotFoundException();
                }
            }

            internal StorageObject(
                    MemoryStorageGraph graph,
                    StorageObjectID id
                ) {
                this.graph = graph;
                this.id = id;

                filedata = new MemoryFile();
                filedata.Written += Filedata_Written;
            }

            internal void Init() {
                graph.ArrowAdded += Graph_ArrowAdded;
                graph.ArrowRemoved += Graph_ArrowRemoved;
                graph.ArrowRenamed += Graph_ArrowRenamed;

                graph.NodeContentsSet += Graph_NodeContentsSet;
                graph.NodeDeleted += Graph_NodeDeleted;
            }

            ~StorageObject() {
                graph.ArrowAdded -= Graph_ArrowAdded;
                graph.ArrowRemoved -= Graph_ArrowRemoved;
                graph.ArrowRenamed -= Graph_ArrowRenamed;

                graph.NodeContentsSet -= Graph_NodeContentsSet;
                graph.NodeDeleted -= Graph_NodeDeleted;
            }

            private void Graph_ArrowAdded(
                    StorageObjectID container,
                    StorageObjectID child, 
                    string key
                ) {
                if (container == id)
                    foreach (var responder in ChildAdded_responders)
                        responder(container, child, key);
            }

            private void Graph_ArrowRemoved(
                    StorageObjectID container,
                    StorageObjectID child, 
                    string key
                ) {
                if (container == id)
                    ChildRemoved?.Invoke(container, child, key);
            }

            private void Graph_ArrowRenamed(
                    StorageObjectID container,
                    StorageObjectID child,
                    string oldkey,
                    string newkey
                ) {
                if (container == id)
                    ChildRenamed?.Invoke(container, child, oldkey, newkey);
            }

            private void Graph_NodeContentsSet(
                    StorageObjectID affected
                ) {
                if (affected == id)
                    foreach (var responder in ContentsSet_responders)
                        responder(affected);

                foreach(var key in GetRelations(affected))
                    foreach (var responder in ChildContentsSet_responders)
                        responder(id, affected, key);
            }

            private void Graph_NodeDeleted(
                    StorageObjectID affected
                ) {
                if (affected == id)
                    Deleted?.Invoke(affected);
            }

            private void Filedata_Written() =>
                graph.SetContents(id);

            public bool HasChild(string child) =>
                graph.HasChild(id, child);

            public bool HasChild(StorageObjectID storageobjectID) =>
                graph.HasChild(id, storageobjectID);

            public IStorageObject Open(string child) =>
                graph[this[child]];

            public Stream OpenRead() =>
                filedata.OpenRead();

            public Stream OpenWrite() =>
                filedata.OpenWrite();

            public byte[] GetData() =>
                filedata.data;

            public void SetData(byte[] data) {
                filedata.data = data;

                graph.SetContents(id);
            }

            public string GetRelation(StorageObjectID child) =>
                graph.GetRelation(id, child);

            public IEnumerable<string> GetRelations(StorageObjectID child) =>
                graph.GetRelations(id, child);

            public void Add(string key, StorageObjectID id) =>
                graph.AddArrow(this.id, id, key);

            public void Rename(string oldkey, string newkey) =>
                graph.RenameArrow(id, this[oldkey], newkey);

            public void Rename(StorageObjectID child, string newkey) =>
                graph.RenameArrow(id, child, newkey);

            public void Remove(string key) =>
                graph.RemoveArrow(id, this[key]);

            public void Remove(StorageObjectID child) =>
                graph.RemoveArrow(id, child);

            public void Delete() =>
                graph.Delete(id);
        }
    }
}
