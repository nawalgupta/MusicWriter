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

        void Setup() {
            field.ItemAdded += Field_ItemAdded;
            field.ItemMoved += Field_ItemMoved;
            field.ItemChanged += Field_ItemChanged;
            field.ItemRemoved += Field_ItemRemoved;

            storage.ChildAdded += Storage_ChildAdded;
            storage.ChildRenamed += Storage_ChildRenamed;
            storage.ChildContentsSet += Storage_ChildContentsChanged;
            storage.ChildRemoved += Storage_ChildRemoved;
        }

        private void Storage_ChildAdded(StorageObjectID container, StorageObjectID child) {
            var duration =
                CodeTools.ReadDuration(storage.GetRelation(child));

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

        private void Storage_ChildRenamed(
                StorageObjectID container,
                StorageObjectID child,
                string oldkey,
                string newkey
            ) {
            var oldduration = CodeTools.ReadDuration(oldkey);
            var newduration = CodeTools.ReadDuration(newkey);

            field.TryMoveUnique(oldduration, newduration);
        }

        private void Storage_ChildContentsChanged(StorageObjectID container, StorageObjectID child) {
            var duration =
                CodeTools.ReadDuration(storage.GetRelation(child));

            var contents =
                Deserializer(storage.Graph[child]);

            if (field.HasItem(duration))
                field.UpdateUnique(duration, contents);
        }

        private void Storage_ChildRemoved(StorageObjectID container, StorageObjectID child) {
            var duration =
                CodeTools.ReadDuration(storage.GetRelation(child));

            if (field.HasItem(duration))
                field.RemoveUnique(duration);
        }

        private void Field_ItemAdded(
                Duration duration,
                T value
            ) {
            var relation =
                CodeTools.WriteDuration(duration);

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

            if (storage.HasChild(relation))
                storage.Get(relation).Delete();
        }
    }
}
