using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class BoundList<T>
        : IObservableList<T>
        where T : IBoundObject<T>
    {
        readonly StorageObjectID storageobjectID;
        readonly EditorFile file;
        readonly FactorySet<T> factoryset;
        readonly ViewerSet<T> viewerset;

        readonly Dictionary<string, T> map_name = new Dictionary<string, T>();
        readonly Dictionary<T, string> map_name_inverse = new Dictionary<T, string>();
        readonly Dictionary<StorageObjectID, T> map_storageobjectID = new Dictionary<StorageObjectID, T>();
        readonly Dictionary<T, StorageObjectID> map_storageobjectID_inverse = new Dictionary<T, StorageObjectID>();
        readonly IOListener
            listener_add,
            listener_remove;
        
        public event Action<T> ItemAdded {
            add { Objects.ItemAdded += value; }
            remove { Objects.ItemAdded -= value; }
        }

        public event Action<T, int> ItemInserted {
            add { Objects.ItemInserted += value; }
            remove { Objects.ItemInserted -= value; }
        }

        public event Action<T> ItemRemoved {
            add { Objects.ItemRemoved += value; }
            remove { Objects.ItemRemoved -= value; }
        }

        public event Action<T, int> ItemWithdrawn {
            add { Objects.ItemWithdrawn += value; }
            remove { Objects.ItemWithdrawn -= value; }
        }

        public FactorySet<T> FactorySet {
            get { return factoryset; }
        }

        public ViewerSet<T> ViewerSet {
            get { return viewerset; }
        }

        public ObservableList<T> Objects { get; } =
            new ObservableList<T>();

        public StorageObjectID StorageObjectID {
            get { return storageobjectID; }
        }

        public EditorFile File {
            get { return file; }
        }

        public bool AutomaticallyAvoidNameCollisionsWithUnderlines { get; set; } = true;

        public int Count {
            get {
                return Objects.Count;
            }
        }

        public bool IsReadOnly {
            get {
                return Objects.IsReadOnly;
            }
        }

        public T this[int index] {
            get {
                return Objects[index];
            }

            set {
                Objects[index] = value;
            }
        }

        public T this[string name] {
            get { return map_name[name]; }
        }

        public T this[StorageObjectID storageobjectID] {
            get { return map_storageobjectID[storageobjectID]; }
        }

        public BoundList(
                StorageObjectID storageobjectID,
                EditorFile file,
                FactorySet<T> factoryset,
                ViewerSet<T> viewerset
            ) {
            this.storageobjectID = storageobjectID;
            this.file = file;
            this.factoryset = factoryset;
            this.viewerset = viewerset;
            
            var hub_obj = file.Storage[storageobjectID];

            var propertybinders =
                new Dictionary<string, PropertyBinder<string>>();

            var namelisteners =
                new Dictionary<string, IOListener>();

            listener_add =
                hub_obj.Listen(
                        IOEvent.ChildAdded,
                        (key, objID) => {
                            if (key != "")
                                throw new InvalidOperationException();

                            var obj =
                                FactorySet.Load(objID, file);

                            var name_obj =
                                file
                                    .Storage
                                    [objID]
                                    .GetOrMake("name");

                            var binder =
                                obj.Name.Bind(name_obj);

                            obj.Name.AfterChange += propertybinders.Rename;
                            propertybinders.Add(binder.Property.Value, binder);

                            if (!Objects.Contains(obj))
                                Objects.Add(obj);
                        }
                    );

            listener_remove =
                hub_obj.Listen(
                        IOEvent.ChildRemoved,
                        (key, objID) => {
                            if (key != "")
                                throw new InvalidOperationException();

                            var obj =
                                Objects.FirstOrDefault(_ => _.StorageObjectID == objID);

                            if (obj != null) {
                                obj.Name.BeforeChange -= Object_Renaming;
                                obj.Name.AfterChange -= Object_Renamed;

                                obj.Name.AfterChange -= propertybinders.Rename;

                                propertybinders[obj.Name.Value].Dispose();
                                propertybinders.Remove(obj.Name.Value);

                                obj.Unbind();
                                Objects.Remove(obj);
                            }
                        }
                    );

            Objects.ItemAdded += obj => {
                if (!hub_obj.HasChild(obj.StorageObjectID)) {
                    //TODO: not sure if this will work
                    // What's supposed to happen is that if a bound object already in one
                    // bound list is added to another bound list, the second bound list 
                    // should exhibit non-exclusive ownership of the object.

                    hub_obj.Add("", obj.StorageObjectID);
                }

                obj.Name.BeforeChange += Object_Renaming;
                obj.Name.AfterChange += Object_Renamed;

                map_name.Add(obj.Name.Value, obj);
                map_name_inverse.Add(obj, obj.Name.Value);
                map_storageobjectID.Add(obj.StorageObjectID, obj);
                map_storageobjectID_inverse.Add(obj, obj.StorageObjectID);
            };

            Objects.ItemRemoved += obj => {
                if (hub_obj.HasChild(obj.StorageObjectID)) {
                    obj.Unbind();
                    hub_obj.Remove(obj.StorageObjectID);

                    obj.Name.BeforeChange -= Object_Renaming;
                    obj.Name.AfterChange -= Object_Renamed;

                    obj.Name.AfterChange -= propertybinders.Rename;

                    propertybinders[obj.Name.Value].Dispose();
                    propertybinders.Remove(obj.Name.Value);

                    map_name.Remove(obj.Name.Value);
                    map_name_inverse.Remove(obj);
                    map_storageobjectID.Remove(obj.StorageObjectID);
                    map_storageobjectID_inverse.Remove(obj);
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

            map_name.Rename(old, @new);

            map_name_inverse[item] = @new;
        }

        public T Create(string type) {
            var storageobjectID =
                FactorySet.Init(type, StorageObjectID, file);

            while (!map_storageobjectID.ContainsKey(storageobjectID))
                Thread.Sleep(20);

            return this[storageobjectID];
        }

        public int IndexOf(T item) {
            return Objects.IndexOf(item);
        }

        public void Insert(int index, T item) {
            Objects.Insert(index, item);
        }

        public void RemoveAt(int index) {
            Objects.RemoveAt(index);
        }

        public void Add(T item) {
            Objects.Add(item);
        }

        public void Clear() {
            Objects.Clear();
        }

        public bool Contains(T item) {
            return Objects.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            Objects.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            return Objects.Remove(item);
        }

        public IEnumerator<T> GetEnumerator() {
            return Objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Objects.GetEnumerator();
        }
    }
}
