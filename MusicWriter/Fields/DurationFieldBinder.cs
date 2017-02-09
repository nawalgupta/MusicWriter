using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class DurationFieldBinder<T> : 
        BoundObject<DurationFieldBinder<T>>
    {
        readonly DurationField<T> field;
        readonly IStorageObject obj;

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

        public DurationFieldBinder(
                StorageObjectID storageobjectID,
                EditorFile file,
                DurationField<T> field
            ) :
            base(
                    storageobjectID,
                    file,
                    null //TODO
                ) {
            this.field = field;

            obj = this.Object();

            field.ItemAdded += Field_ItemAdded;
            field.ItemMoved += Field_ItemMoved;
            field.ItemChanged += Field_ItemChanged;
            field.ItemRemoved += Field_ItemRemoved;

            listener_added = obj.CreateListen(IOEvent.ChildAdded, Storage_ChildAdded);
            listener_rekeyed = obj.Graph.CreateListen(Storage_ChildRekeyed, storageobjectID, IOEvent.ChildRekeyed);
            listener_contentsset = obj.CreateListen(IOEvent.ChildContentsSet, Storage_ChildContentsSet);
            listener_removed = obj.CreateListen(IOEvent.ChildRemoved, Storage_ChildRemoved);
        }
        
        public override void Bind() {
            File.Storage.Listeners.Add(listener_added);
            File.Storage.Listeners.Add(listener_rekeyed);
            File.Storage.Listeners.Add(listener_contentsset);
            File.Storage.Listeners.Add(listener_removed);

            base.Bind();
        }

        public override void Unbind() {
            File.Storage.Listeners.Remove(listener_added);
            File.Storage.Listeners.Remove(listener_rekeyed);
            File.Storage.Listeners.Remove(listener_contentsset);
            File.Storage.Listeners.Remove(listener_removed);
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

                var contents = Deserializer(File.Storage[child]);
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
                Deserializer(File.Storage[child]);

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

            if (!obj.HasChild(relation)) {
                var item_objID =
                    File.Storage.Create();

                Serializer(File.Storage[item_objID], value);

                obj.Add(relation, item_objID);
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

            if (obj.HasChild(oldrelation))
                obj.Rename(oldrelation, CodeTools.WriteDuration(newduration));
        }

        private void Field_ItemChanged(
                Duration duration,
                T oldvalue,
                T newvalue
            ) {
            var relation =
                CodeTools.WriteDuration(duration);

            Serializer(obj.Get(relation), newvalue);
        }

        private void Field_ItemRemoved(
                Duration duration,
                T value
            ) {
            var relation =
                CodeTools.WriteDuration(duration);

            events.Remove(duration);

            if (obj.HasChild(relation))
                obj.Get(relation).Delete();
        }
    }
}
