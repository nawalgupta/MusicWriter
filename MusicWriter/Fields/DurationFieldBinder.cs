using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class DurationFieldBinder<T>
    {
        readonly DurationField<T> field;
        readonly IStorageObject storage;

        IOListener
            listener_added,
            listener_rekeyed,
            listener_contentsset,
            listener_removed;

        readonly object locker = new object();
        readonly HashSet<Duration> events = new HashSet<Duration>();
        //TOOD: support multiple events occuring at the same time in the field

        public Func<IStorageObject, T> Deserializer { get; set; }
        public Action<IStorageObject, T> Serializer { get; set; }
        
        public DurationField<T> Field {
            get { return field; }
        }

        public IStorageObject Storage {
            get { return storage; }
        }

        public DurationFieldBinder(
                DurationField<T> field,
                IStorageObject storage
            ) {
            this.field = field;
            this.storage = storage;
        }

        bool started = false;
        public void Start() {
            if (!started)
                started = true;
            else throw new InvalidOperationException();

            Setup();
        }

        public void Unbind() {
            if (started) {
                storage.Graph.Listeners.Remove(listener_added);
                storage.Graph.Listeners.Remove(listener_rekeyed);
                storage.Graph.Listeners.Remove(listener_contentsset);
                storage.Graph.Listeners.Remove(listener_removed);
            }
        }

        void Setup() {
            field.ItemAdded += Field_ItemAdded;
            field.ItemMoved += Field_ItemMoved;
            field.ItemChanged += Field_ItemChanged;
            field.ItemRemoved += Field_ItemRemoved;

            listener_added = storage.Listen(IOEvent.ChildAdded, Storage_ChildAdded);
            listener_rekeyed = storage.Graph.Listen(Storage_ChildRekeyed, storage.ID, IOEvent.ChildRekeyed);
            listener_contentsset = storage.Listen(IOEvent.ChildContentsSet, Storage_ChildContentsSet);
            listener_removed = storage.Listen(IOEvent.ChildRemoved, Storage_ChildRemoved);
        }

        private void Storage_ChildAdded(string key, StorageObjectID child) {
            var duration =
                CodeTools.ReadDuration(key);

            if (!events.Contains(duration)) {
                lock (locker) {
                    if (events.Contains(duration))
                        return;
                    events.Add(duration);
                }

                var contents = Deserializer(storage.Graph[child]);
                field.Add(contents, duration);
            }
        }

        private void Storage_ChildRekeyed(IOMessage message) {
            var oldduration = CodeTools.ReadDuration(message.Relation);
            var newduration = CodeTools.ReadDuration(message.NewRelation);

            field.TryMoveUnique(oldduration, newduration);
        }

        private void Storage_ChildContentsSet(string key, StorageObjectID child) {
            var duration =
                CodeTools.ReadDuration(key);

            var contents =
                Deserializer(storage.Graph[child]);

            if (field.HasItem(duration))
                field.UpdateUnique(duration, contents);
        }

        private void Storage_ChildRemoved(string key, StorageObjectID child) {
            var duration =
                CodeTools.ReadDuration(key);

            if (field.HasItem(duration))
                field.RemoveUnique(duration);
        }

        private void Field_ItemAdded(
                Duration duration,
                T value
            ) {
            var relation =
                CodeTools.WriteDuration(duration);

            events.Add(duration);

            if (!storage.HasChild(relation)) {
                var item_objID =
                    storage.Graph.Create();

                Serializer(storage.Graph[item_objID], value);

                storage.Add(relation, item_objID);
            }
        }

        private void Field_ItemMoved(
                Duration oldduration,
                Duration newduration,
                T value
            ) {
            var oldrelation =
                CodeTools.WriteDuration(oldduration);

            events.Remove(oldduration);
            events.Add(newduration);

            if (storage.HasChild(oldrelation))
                storage.Rename(oldrelation, CodeTools.WriteDuration(newduration));
        }

        private void Field_ItemChanged(
                Duration duration,
                T oldvalue,
                T newvalue
            ) {
            var relation =
                CodeTools.WriteDuration(duration);

            Serializer(storage.Get(relation), newvalue);
        }

        private void Field_ItemRemoved(
                Duration duration,
                T value
            ) {
            var relation =
                CodeTools.WriteDuration(duration);

            events.Remove(duration);

            if (storage.HasChild(relation))
                storage.Get(relation).Delete();
        }
    }
}
