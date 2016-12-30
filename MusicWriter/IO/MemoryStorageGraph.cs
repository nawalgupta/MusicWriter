using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public partial class MemoryStorageGraph : IStorageGraph
    {
        readonly Dictionary<StorageObjectID, StorageObject> storage =
            new Dictionary<StorageObjectID, StorageObject>();
        readonly Dictionary<StorageObjectID, ArchivalState> archivalstates =
            new Dictionary<StorageObjectID, ArchivalState>();

        enum ArchivalState
        {
            Archived,
            Unarchived
        }

        readonly Dictionary<StorageObjectID, List<StorageObjectID>> arrows_to_source; // sink -> source
        readonly Dictionary<StorageObjectID, List<KeyValuePair<string, StorageObjectID>>> arrows_to_sink; // source -> sink
        readonly HashSet<StorageObjectID> isolated_nodes = new HashSet<StorageObjectID>();
        
        readonly List<StorageObjectChildChangedDelegate> ArrowAdded_list = new List<StorageObjectChildChangedDelegate>();
        public event StorageObjectChildChangedDelegate ArrowAdded {
            add {
                ArrowAdded_list.Add(value);

                foreach (var sink in arrows_to_source.Keys)
                    foreach (var source in arrows_to_source[sink])
                        value(source, sink);
            }
            remove {
                ArrowAdded_list.Remove(value);
            }
        }
        public event StorageObjectChildChangedDelegate ArrowRemoved;
        public event StorageObjectChildChangedDelegate ArrowRenamed;

        readonly List<StorageObjectChangedDelegate> NodeCreated_list = new List<StorageObjectChangedDelegate>();
        public event StorageObjectChangedDelegate NodeCreated {
            add {
                NodeCreated_list.Add(value);

                foreach (var id in storage.Keys)
                    value(id);
            }
            remove {
                NodeCreated_list.Remove(value);
            }
        }
        readonly List<StorageObjectChangedDelegate> NodeContentsSet_list = new List<StorageObjectChangedDelegate>();
        public event StorageObjectChangedDelegate NodeContentsSet {
            add {
                NodeContentsSet_list.Add(value);

                foreach (var id in storage.Keys)
                    value(id);
            }
            remove {
                NodeContentsSet_list.Remove(value);
            }
        }
        public event StorageObjectChangedDelegate NodeDeleted;

        public StorageObjectID Root {
            get { return StorageObjectID.Zero; }
        }

        public IStorageObject this[StorageObjectID id] {
            get {
                if (archivalstates[id] == ArchivalState.Archived)
                    Unarchive(id);

                return storage[id];
            }
        }

        public IEnumerable<StorageObjectID> Objects {
            get { return storage.Keys; }
        }

        public MemoryStorageGraph() {
            usedIDs.Add(StorageObjectID.Zero.ID);
        }

        public IEnumerable<StorageObjectID> Incoming(StorageObjectID sink) =>
            arrows_to_source.Lookup(sink);

        public IEnumerable<KeyValuePair<string, StorageObjectID>> Outgoing(StorageObjectID source) =>
            arrows_to_sink[source];

        public void Load(StorageObjectID id, bool isarchived = true) {
            if (usedIDs.Contains(id.ID))
                throw new InvalidOperationException("ID is already in use.");
            usedIDs.Add(id.ID);

            archivalstates.Add(id, isarchived ? ArchivalState.Archived : ArchivalState.Unarchived);
        }
        
        public bool Contains(StorageObjectID id) =>
            usedIDs.Contains(id.ID);

        HashSet<Guid> usedIDs = new HashSet<Guid>();
        public StorageObjectID Create() {
            Guid internalID;

            do internalID = Guid.NewGuid();
            while (usedIDs.Contains(internalID));
            
            usedIDs.Add(internalID);
            
            var id = new StorageObjectID(internalID);

            storage.Add(id, new StorageObject(this));

            foreach (var responder in NodeCreated_list)
                responder(id);

            archivalstates.Add(id, ArchivalState.Unarchived);

            return id;
        }

        public void Delete(StorageObjectID id) {
            NodeDeleted?.Invoke(id);
            usedIDs.Remove(id.ID);

            foreach(var kvp in arrows_to_sink.Lookup(id))
                RemoveArrow(id, kvp.Value);

            foreach (var source in arrows_to_source.Lookup(id))
                RemoveArrow(source, id);
        }

        public IEnumerable<StorageObjectID> Isolated() =>
            isolated_nodes;

        protected virtual void AddArrow(StorageObjectID source, StorageObjectID sink, string key) {
            if (isolated_nodes.Contains(source))
                isolated_nodes.Remove(source);

            var kvp =
                new KeyValuePair<string, StorageObjectID>(key, sink);

            arrows_to_sink.Lookup(source).Add(kvp);
            arrows_to_source.Lookup(sink).Add(source);

            foreach (var responder in ArrowAdded_list)
                responder(source, sink);
        }

        protected virtual void RemoveArrow(StorageObjectID source, StorageObjectID sink) {
            if (!this[source].Children.Any())
                isolated_nodes.Add(source);

            var list = arrows_to_sink.Lookup(source);
            for (int i = 0; i < list.Count; i++) {
                if (list[i].Value == sink) {
                    list.RemoveAt(i);
                    break;
                }
            }

            arrows_to_source.Lookup(sink).Remove(source);

            ArrowRemoved?.Invoke(source, sink);
        }

        protected virtual void RenameArrow(StorageObjectID source, StorageObjectID sink, string newkey) {
            var list = arrows_to_sink.Lookup(source);
            for (int i = 0; i < list.Count; i++) {
                if (list[i].Value == sink) {
                    list[i] = new KeyValuePair<string, StorageObjectID>(newkey, sink);
                    break;
                }
            }

            ArrowRenamed?.Invoke(source, sink);
        }

        protected virtual void SetContents(StorageObjectID id) {
            foreach (var responder in NodeContentsSet_list)
                responder(id);
        }

        protected virtual void Unarchive(StorageObjectID id) {
            archivalstates[id] = ArchivalState.Unarchived;
        }

        protected virtual void Archive(StorageObjectID id) {
            archivalstates[id] = ArchivalState.Archived;
        }

        public StorageObject GetSpecialStorageObject(StorageObjectID id) =>
            storage[id];

        public string GetRelation(StorageObjectID source, StorageObjectID sink) =>
            arrows_to_sink.Lookup(source).FirstOrDefault(x => x.Value == sink).Key;
    }
}
