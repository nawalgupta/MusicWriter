using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public partial class MemoryStorageGraph : IStorageGraph {
        readonly Dictionary<StorageObjectID, StorageObject> storage =
            new Dictionary<StorageObjectID, StorageObject>();
        readonly Dictionary<StorageObjectID, ArchivalState> archivalstates =
            new Dictionary<StorageObjectID, ArchivalState>();

        enum ArchivalState {
            Archived,
            Unarchived
        }

        readonly Dictionary<StorageObjectID, Dictionary<string, List<StorageObjectID>>> arrows_to_source =
            new Dictionary<StorageObjectID, Dictionary<string, List<StorageObjectID>>>(); // sink -> source
        readonly Dictionary<StorageObjectID, Dictionary<StorageObjectID, List<string>>> arrows_to_source_inverse =
            new Dictionary<StorageObjectID, Dictionary<StorageObjectID, List<string>>>(); // sink -> source
        readonly Dictionary<StorageObjectID, Dictionary<string, List<StorageObjectID>>> arrows_to_sink =
            new Dictionary<StorageObjectID, Dictionary<string, List<StorageObjectID>>>(); // source -> sink
        readonly Dictionary<StorageObjectID, Dictionary<StorageObjectID, List<string>>> arrows_to_sink_inverse =
            new Dictionary<StorageObjectID, Dictionary<StorageObjectID, List<string>>>(); // source -> sink
        readonly Dictionary<StorageObjectID, int> node_refcount =
            new Dictionary<StorageObjectID, int>();
        readonly IIOMessageReactor messagestore;
        
        public StorageObjectID Root {
            get { return root.ID; }
        }
        readonly RootMemoryStorageObject root;

        public IStorageObject this[StorageObjectID id] {
            get {
                if (id == root.ID)
                    return root;

                if (archivalstates[id] == ArchivalState.Archived)
                    Unarchive(id);

                return storage[id];
            }
        }

        public IEnumerable<StorageObjectID> ObjectIDs {
            get {
                yield return root.ID;

                foreach (var id in storage.Keys)
                    yield return id;
            }
        }

        public IObservableList<IOMessage> Messages { get; } =
            new ObservableList<IOMessage>();

        public IObservableList<IOListener> Listeners { get; } =
            new ObservableList<IOListener>();

        public MemoryStorageGraph() {
            usedIDs.Add(StorageObjectID.Zero.ID);
            usedIDs.Add(StorageObjectID.Any.ID);

            root = new RootMemoryStorageObject(this);

            messagestore =
                new BruteIOMessageReactor(
                        Messages,
                        Listeners    
                    );
        }

        public IEnumerable<KeyValuePair<string, StorageObjectID>> Incoming(StorageObjectID sink) =>
            from kvp in arrows_to_source[sink]
            from source in kvp.Value
            select new KeyValuePair<string, StorageObjectID>(kvp.Key, source);

        public IEnumerable<KeyValuePair<string, StorageObjectID>> Outgoing(StorageObjectID source) =>
            from kvp in arrows_to_sink[source]
            from sink in kvp.Value
            select new KeyValuePair<string, StorageObjectID>(kvp.Key, sink);

        public void Load(StorageObjectID id, bool isarchived = true) {
            if (usedIDs.Contains(id.ID))
                throw new InvalidOperationException("ID is already in use.");
            usedIDs.Add(id.ID);

            archivalstates.Add(id, isarchived ? ArchivalState.Archived : ArchivalState.Unarchived);

            arrows_to_sink.Add(id, new Dictionary<string, List<StorageObjectID>>());
            arrows_to_source.Add(id, new Dictionary<string, List<StorageObjectID>>());
            arrows_to_sink_inverse.Add(id, new Dictionary<StorageObjectID, List<string>>());
            arrows_to_source_inverse.Add(id, new Dictionary<StorageObjectID, List<string>>());

            var obj = new StorageObject(this, id);
            storage.Add(id, obj);

            node_refcount.Add(id, 0);

            obj.Init();

            Messages.Add(new IOMessage(id, IOEvent.ObjectCreated));
        }

        public virtual bool Contains(StorageObjectID id) =>
            usedIDs.Contains(id.ID);

        HashSet<Guid> usedIDs = new HashSet<Guid>();
        public virtual StorageObjectID Create() {
            Guid internalID;

            do internalID = Guid.NewGuid();
            while (usedIDs.Contains(internalID));

            usedIDs.Add(internalID);

            var id = new StorageObjectID(internalID);
            var obj = new StorageObject(this, id);

            storage.Add(id, obj);

            node_refcount.Add(id, 0);
            archivalstates.Add(id, ArchivalState.Unarchived);
            arrows_to_sink.Add(id, new Dictionary<string, List<StorageObjectID>>());
            arrows_to_source.Add(id, new Dictionary<string, List<StorageObjectID>>());
            arrows_to_sink_inverse.Add(id, new Dictionary<StorageObjectID, List<string>>());
            arrows_to_source_inverse.Add(id, new Dictionary<StorageObjectID, List<string>>());

            obj.Init();

            Messages.Add(new IOMessage(id, IOEvent.ObjectCreated));

            return id;
        }

        public void Delete(StorageObjectID id) {
            Messages.Add(new IOMessage(id, IOEvent.ObjectDeleted));
            usedIDs.Remove(id.ID);

            foreach (var sink_map in arrows_to_sink[id]) {
                foreach (var sink in sink_map.Value) {
                    //TODO: this will fail if the arrow is pointing to its source

                    Messages.Add(new IOMessage(id, IOEvent.ChildRemoved, sink_map.Key, sink));

                    arrows_to_source[sink][sink_map.Key].Remove(id);
                    arrows_to_source_inverse[sink][id].Remove(sink_map.Key);
                }
            }

            foreach (var source_map in arrows_to_source[id]) {
                foreach (var source in source_map.Value) {
                    //TODO: this will fail if the arrow is pointing to its source

                    Messages.Add(new IOMessage(source, IOEvent.ChildRemoved, source_map.Key, id));

                    arrows_to_sink[source][source_map.Key].Remove(id);
                    arrows_to_sink_inverse[source][id].Remove(source_map.Key);
                }
            }

            node_refcount.Remove(id);
            storage.Remove(id);
            archivalstates.Remove(id);

            arrows_to_sink.Remove(id);
            arrows_to_source.Remove(id);
            arrows_to_sink_inverse.Remove(id);
            arrows_to_source_inverse.Remove(id);
        }

        public IEnumerable<StorageObjectID> Isolated() =>
            from refcount in node_refcount
            where refcount.Value == 0
            select refcount.Key;

        protected virtual void AddArrow(StorageObjectID source, StorageObjectID sink, string key) {
            node_refcount[sink]++;

            arrows_to_sink[source].Lookup(key).Add(sink);
            arrows_to_source[sink].Lookup(key).Add(source);
            arrows_to_sink_inverse[source].Lookup(sink).Add(key);
            arrows_to_source_inverse[source].Lookup(source).Add(key);

            Messages.Add(new IOMessage(source, IOEvent.ChildAdded, key, sink));
        }

        protected virtual void RemoveArrow(StorageObjectID source, StorageObjectID sink) {
            node_refcount[sink]--;

            foreach (var key in arrows_to_sink_inverse[source].Lookup(sink)) {
                arrows_to_sink[source][key].Remove(sink);
                arrows_to_source[sink][key].Remove(source);

                Messages.Add(new IOMessage(source, IOEvent.ChildRemoved, key, sink));
            }

            arrows_to_sink_inverse[source].Remove(sink);
            arrows_to_source_inverse[sink].Remove(source);
        }

        protected virtual void RenameArrow(StorageObjectID source, StorageObjectID sink, string newkey) {
            var source_arrows_to_sink_inverse =
                arrows_to_sink_inverse[source].Lookup(sink);

            var sink_arrows_to_source_inverse =
                arrows_to_source_inverse[sink].Lookup(source);

            foreach (var oldkey in source_arrows_to_sink_inverse) {
                Messages.Add(new IOMessage(source, IOEvent.ChildRekeyed, oldkey, newkey, sink));

                EnumerableExtensions.RenameMerge<string, List<StorageObjectID>, StorageObjectID>(
                        arrows_to_sink[source],
                        oldkey,
                        newkey
                    );

                EnumerableExtensions.RenameMerge<string, List<StorageObjectID>, StorageObjectID>(
                        arrows_to_source[sink],
                        oldkey,
                        newkey
                    );
            }

            source_arrows_to_sink_inverse.Clear();
            source_arrows_to_sink_inverse.Add(newkey);
            sink_arrows_to_source_inverse.Clear();
            sink_arrows_to_source_inverse.Add(newkey);
        }

        protected virtual void SetContents(StorageObjectID id) {
            Messages.Add(new IOMessage(id, IOEvent.ObjectContentsSet));

            foreach (var source_map in arrows_to_source[id])
                foreach (var source in source_map.Value)
                    Messages.Add(new IOMessage(source, IOEvent.ChildContentsSet, source_map.Key, id));
        }

        protected virtual void Unarchive(StorageObjectID id) {
            archivalstates[id] = ArchivalState.Unarchived;
        }

        protected virtual void Archive(StorageObjectID id) {
            archivalstates[id] = ArchivalState.Archived;
        }

        public StorageObject GetSpecialStorageObject(StorageObjectID id) =>
            storage[id];

        public virtual string GetRelation(
                StorageObjectID source,
                StorageObjectID sink
            ) =>
            arrows_to_sink_inverse
                [source]
                .Lookup(sink)
                .Single();

        public virtual IEnumerable<string> GetRelations(
                StorageObjectID source,
                StorageObjectID sink
            ) =>
            arrows_to_sink_inverse
                [source]
                .Lookup(sink);

        public virtual bool HasChild(StorageObjectID source, string relation) =>
            arrows_to_sink[source].ContainsKey(relation);

        public virtual bool HasChild(StorageObjectID source, StorageObjectID sink) =>
            arrows_to_sink_inverse[source].ContainsKey(sink);

        public virtual bool HasChild(StorageObjectID source, StorageObjectID sink, string relation) =>
            arrows_to_sink_inverse[source].Lookup(sink).Contains(relation);

        public virtual void Flush() {
        }
    }
}
