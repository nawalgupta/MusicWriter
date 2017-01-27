using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class BoundList<T>
        where T : IBoundObject<T>
    {
        readonly StorageObjectID storageobjectID;
        readonly EditorFile file;

        readonly Dictionary<string, T> map_name = new Dictionary<string, T>();
        readonly Dictionary<T, string> map_name_inverse = new Dictionary<T, string>();
        readonly Dictionary<StorageObjectID, T> map_storageobjectID = new Dictionary<StorageObjectID, T>();
        readonly Dictionary<T, StorageObjectID> map_storageobjectID_inverse = new Dictionary<T, StorageObjectID>();

        public FactorySet<T> FactorySet { get; } =
            new FactorySet<T>();

        public ObservableList<T> Objects { get; } =
            new ObservableList<T>();

        public StorageObjectID StorageObjectID {
            get { return storageobjectID; }
        }

        public EditorFile File {
            get { return file; }
        }

        public bool AutomaticallyAvoidNameCollisionsWithUnderlines { get; set; } = true;

        public T this[string name] {
            get { return map_name[name]; }
        }

        public T this[StorageObjectID storageobjectID] {
            get { return map_storageobjectID[storageobjectID]; }
        }

        public BoundList(
                StorageObjectID storageobjectID,
                EditorFile file
            ) {
            this.storageobjectID = storageobjectID;
            this.file = file;

            Setup();
        }

        void Setup() {
            var hub_obj = file.Storage[storageobjectID];

            hub_obj.ChildAdded += (hub_objID, objID, key) => {
                if (key != "")
                    throw new InvalidOperationException();

                var obj =
                    FactorySet.Load(objID, file);

                Objects.Add(obj);
            };

            hub_obj.ChildRemoved += (hub_objID, objID, key) => {
                if (key != "")
                    throw new InvalidOperationException();

                var obj =
                    Objects.FirstOrDefault(_ => _.StorageObjectID == objID);

                if (obj != null)
                    Objects.Remove(obj);
            };

            Objects.ItemAdded += obj => {
                if (!hub_obj.HasChild(obj.StorageObjectID))
                    throw new InvalidOperationException();

                obj.Name.BeforeChange += Object_Renaming;
                obj.Name.AfterChange += Object_Renamed;
            };

            Objects.ItemRemoved += obj => {
                if (hub_obj.HasChild(obj.StorageObjectID)) {
                    hub_obj.Remove(obj.StorageObjectID);

                    obj.Name.BeforeChange -= Object_Renaming;
                    obj.Name.AfterChange -= Object_Renamed;
                }
            };
        }

        private void Object_Renaming(ObservableProperty<string>.PropertyChangingEventArgs args) {
            if (map_name.ContainsKey(args.NewValue)) {
                if (AutomaticallyAvoidNameCollisionsWithUnderlines) {
                    args.NewValue += "_";
                    args.Altered = true;
                }
                else throw new ArgumentException($"Name \"{args.NewValue}\" already in use.");
            }
        }

        private void Object_Renamed(string old, string @new) {
            var item = map_name[old];

            map_name.Remove(old);
            map_name.Add(@new, item);

            map_name_inverse[item] = @new;
        }

        public T Create(string type) {
            var storageobjectID =
                FactorySet.Init(type, StorageObjectID, file);

            while (!map_storageobjectID.ContainsKey(storageobjectID))
                Thread.Sleep(20);

            return this[storageobjectID];
        }
    }
}
