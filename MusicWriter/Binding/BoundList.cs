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
        : BoundObject<BoundList<T>>,
        IObservableList<T>
        where T : IBoundObject<T>
    {
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
            ) :
            base(
                    storageobjectID,
                    file,
                    null //TODO
                ) {
            this.factoryset = factoryset;
            this.viewerset = viewerset;

            var hub_obj = File.Storage[StorageObjectID];

            var propertybinders =
                new Dictionary<string, PropertyBinder<string>>();

            listener_add =
                hub_obj.CreateListen(
                        IOEvent.ChildAdded,
                        (key, objID) => {
                            if (key != "")
                                throw new InvalidOperationException();

                            var obj =
                                FactorySet.Load(objID, File);

                            var namedobj =
                                obj as INamedObject;

                            if (namedobj != null) {
                                var name_obj =
                                    File
                                        .Storage
                                        [objID]
                                        .GetOrMake("name");

                                var binder =
                                    namedobj.Name.Bind(name_obj);

                                namedobj.Name.AfterChange += propertybinders.Rename;
                                propertybinders.Add(binder.Property.Value, binder);
                            }

                            if (!Objects.Contains(obj)) {
                                Objects.Add(obj);

                                obj.Bind();
                            }
                        }
                    );

            listener_remove =
                hub_obj.CreateListen(
                        IOEvent.ChildRemoved,
                        (key, objID) => {
                            if (key != "")
                                throw new InvalidOperationException();

                            var obj =
                                Objects.FirstOrDefault(_ => _.StorageObjectID == objID);

                            if (obj != null) {
                                var namedobj =
                                    obj as INamedObject;

                                if (namedobj != null) {
                                    namedobj.Name.BeforeChange -= Object_Renaming;
                                    namedobj.Name.AfterChange -= Object_Renamed;

                                    namedobj.Name.AfterChange -= propertybinders.Rename;

                                    propertybinders[namedobj.Name.Value].Dispose();
                                    propertybinders.Remove(namedobj.Name.Value);
                                }

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
                    obj.Bind();
                }
                
                var namedobj =
                    obj as INamedObject;

                if (namedobj != null) {
                    namedobj.Name.BeforeChange += Object_Renaming;
                    namedobj.Name.AfterChange += Object_Renamed;

                    map_name.Add(namedobj.Name.Value, obj);
                    map_name_inverse.Add(obj, namedobj.Name.Value);
                }

                map_storageobjectID.Add(obj.StorageObjectID, obj);
                map_storageobjectID_inverse.Add(obj, obj.StorageObjectID);
            };

            Objects.ItemRemoved += obj => {
                if (hub_obj.HasChild(obj.StorageObjectID)) {
                    obj.Unbind();
                    hub_obj.Remove(obj.StorageObjectID);

                    var namedobj =
                        obj as INamedObject;

                    if (namedobj != null) {
                        namedobj.Name.BeforeChange -= Object_Renaming;
                        namedobj.Name.AfterChange -= Object_Renamed;

                        namedobj.Name.AfterChange -= propertybinders.Rename;

                        propertybinders[namedobj.Name.Value].Dispose();
                        propertybinders.Remove(namedobj.Name.Value);

                        map_name.Remove(namedobj.Name.Value);
                        map_name_inverse.Remove(obj);
                    }

                    map_storageobjectID.Remove(obj.StorageObjectID);
                    map_storageobjectID_inverse.Remove(obj);
                }
            };
        }

        public override void Bind() {
            File.Storage.Listeners.Add(listener_add);
            File.Storage.Listeners.Add(listener_remove);

            base.Bind();
        }

        public override void Unbind() {
            File.Storage.Listeners.Remove(listener_add);
            File.Storage.Listeners.Remove(listener_remove);

            base.Unbind();
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
                FactorySet.Init(type, StorageObjectID, File);

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
