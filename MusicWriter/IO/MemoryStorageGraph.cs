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

        readonly Dictionary<StorageObjectID, List<KeyValuePair<string, StorageObjectID>>> arrows_to_source =
            new Dictionary<StorageObjectID, List<KeyValuePair<string, StorageObjectID>>>(); // sink -> source
        readonly Dictionary<StorageObjectID, List<KeyValuePair<string, StorageObjectID>>> arrows_to_sink =
            new Dictionary<StorageObjectID, List<KeyValuePair<string, StorageObjectID>>>(); // source -> sink
        readonly HashSet<StorageObjectID> isolated_nodes =
            new HashSet<StorageObjectID>();
        readonly IOMessageStore messagestore;
        
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

        public IEnumerable<StorageObjectID> Objects {
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
                new IOMessageStore(
                        Messages,
                        Listeners    
                    );
        }

        public IEnumerable<KeyValuePair<string, StorageObjectID>> Incoming(StorageObjectID sink) =>
            arrows_to_source[sink];

        public IEnumerable<KeyValuePair<string, StorageObjectID>> Outgoing(StorageObjectID source) =>
            arrows_to_sink[source];

        public void Load(StorageObjectID id, bool isarchived = true) {
            if (usedIDs.Contains(id.ID))
                throw new InvalidOperationException("ID is already in use.");
            usedIDs.Add(id.ID);

            archivalstates.Add(id, isarchived ? ArchivalState.Archived : ArchivalState.Unarchived);

            arrows_to_sink.Add(id, new List<KeyValuePair<string, StorageObjectID>>());
            arrows_to_source.Lookup(id);

            var obj = new StorageObject(this, id);
            storage.Add(id, obj);

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

            isolated_nodes.Add(id);
            archivalstates.Add(id, ArchivalState.Unarchived);
            arrows_to_sink.Add(id, new List<KeyValuePair<string, StorageObjectID>>());
            arrows_to_source.Add(id, new List<KeyValuePair<string, StorageObjectID>>());

            obj.Init();

            Messages.Add(new IOMessage(id, IOEvent.ObjectCreated));

            return id;
        }

        public void Delete(StorageObjectID id) {
            Messages.Add(new IOMessage(id, IOEvent.ObjectDeleted));
            usedIDs.Remove(id.ID);
            
            foreach (var kvp in arrows_to_sink.Lookup(id).ToArray())
                RemoveArrow(id, kvp.Value);

            foreach (var source in arrows_to_source.Lookup(id).ToArray())
                RemoveArrow(source.Value, source.Key, id);

            if (isolated_nodes.Contains(id))
                isolated_nodes.Remove(id);

            storage.Remove(id);
            archivalstates.Remove(id);
            arrows_to_sink.Remove(id);
            arrows_to_source.Remove(id);
        }

        public IEnumerable<StorageObjectID> Isolated() =>
            isolated_nodes;

        protected virtual void AddArrow(StorageObjectID source, StorageObjectID sink, string key) {
            if (isolated_nodes.Contains(source))
                isolated_nodes.Remove(source);
            
            arrows_to_sink.Lookup(source).Add(new KeyValuePair<string, StorageObjectID>(key, sink));
            arrows_to_source.Lookup(sink).Add(new KeyValuePair<string, StorageObjectID>(key, source));

            Messages.Add(new IOMessage(source, IOEvent.ChildAdded, key, sink));
        }

        protected virtual void RemoveArrow(StorageObjectID source, StorageObjectID sink) {
            if (!this[source].Children.Any())
                isolated_nodes.Add(source);

            var list = arrows_to_sink.Lookup(source);
            for (int i = 0; i < list.Count; i++) {
                if (list[i].Value == sink) {
                    var key = list[i].Key;
                    list.RemoveAt(i--);
                    Messages.Add(new IOMessage(source, IOEvent.ChildRemoved, key, sink));
                }
            }

            var list2 = arrows_to_source[sink];
            for (int i = 0; i < list2.Count; i++)
                if (list2[i].Value == source)
                    list2.RemoveAt(i);
        }

        protected virtual void RenameArrow(StorageObjectID source, StorageObjectID sink, string newkey) {
            var list = arrows_to_sink.Lookup(source);
            for (int i = 0; i < list.Count; i++) {
                if (list[i].Value == sink) {
                    var oldkey = list[i].Key;
                    list[i] = new KeyValuePair<string, StorageObjectID>(newkey, sink);
                    Messages.Add(new IOMessage(source, IOEvent.ChildRekeyed, oldkey, newkey, sink));
                }
            }


        }

        protected virtual void SetContents(StorageObjectID id) {
            Messages.Add(new IOMessage(id, IOEvent.ObjectContentsSet));

            foreach(var source in arrows_to_source[id])
        }

        protected virtual void Unarchive(StorageObjectID id) {
            archivalstates[id] = ArchivalState.Unarchived;
        }

        protected virtual void Archive(StorageObjectID id) {
            archivalstates[id] = ArchivalState.Archived;
        }

        public StorageObject GetSpecialStorageObject(StorageObjectID id) =>
            storage[id];

        public virtual string GetRelation(StorageObjectID source, StorageObjectID sink) =>
            arrows_to_sink
                .Lookup(source)
                .FirstOrDefault(x => x.Value == sink)
                .Key;

        public virtual IEnumerable<string> GetRelations(
                StorageObjectID source,
                StorageObjectID sink
            ) =>
            from kvp in arrows_to_sink.Lookup(source)
            where kvp.Value == sink
            select kvp.Key;

        public virtual bool HasChild(StorageObjectID source, string relation) =>
            arrows_to_sink
                .Lookup(source)
                .Any(kvp => kvp.Key == relation);

        public virtual bool HasChild(StorageObjectID source, StorageObjectID sink) =>
            arrows_to_sink
                .Lookup(source)
                .Any(kvp => kvp.Value == sink);

        public virtual bool HasChild(StorageObjectID source, StorageObjectID sink, string relation) =>
            arrows_to_sink
                .Lookup(source)
                .Any(kvp => kvp.Key == relation && kvp.Value == sink);

        public virtual void Flush() {
        }
    }
}
