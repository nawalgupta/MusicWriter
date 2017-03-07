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

        readonly IStorageObject hub_obj;
        readonly Dictionary<string, T> map_name = new Dictionary<string, T>();
        readonly Dictionary<T, string> map_name_inverse = new Dictionary<T, string>();
        readonly Dictionary<StorageObjectID, T> map_storageobjectID = new Dictionary<StorageObjectID, T>();
        readonly Dictionary<T, StorageObjectID> map_storageobjectID_inverse = new Dictionary<T, StorageObjectID>();
        readonly IOListener
            listener_add,
            listener_remove,
            listener_move;
        readonly BoundList<T> master;
        bool isallowedtobindobjects = false;

        public event ObservableListDelegates<T>.ItemAdded ItemAdded {
            add { Objects.ItemAdded += value; }
            remove { Objects.ItemAdded -= value; }
        }

        public event ObservableListDelegates<T>.ItemInserted ItemInserted {
            add { Objects.ItemInserted += value; }
            remove { Objects.ItemInserted -= value; }
        }

        public event ObservableListDelegates<T>.ItemRemoved ItemRemoved {
            add { Objects.ItemRemoved += value; }
            remove { Objects.ItemRemoved -= value; }
        }

        public event ObservableListDelegates<T>.ItemWithdrawn ItemWithdrawn {
            add { Objects.ItemWithdrawn += value; }
            remove { Objects.ItemWithdrawn -= value; }
        }

        public event ObservableListDelegates<T>.ItemMoved ItemMoved {
            add { Objects.ItemMoved += value; }
            remove { Objects.ItemMoved -= value; }
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
            ):
            this(
                    storageobjectID,
                    file,
                    factoryset,
                    viewerset,
                    null
                ) {
        }

        public BoundList(
                StorageObjectID storageobjectID,
                EditorFile file,
                BoundList<T> master
            ) :
            this(
                    storageobjectID,
                    file,
                    master.factoryset,
                    master.viewerset,
                    master
                ) {
        }

        private BoundList(
                StorageObjectID storageobjectID,
                EditorFile file,
                FactorySet<T> factoryset,
                ViewerSet<T> viewerset,
                BoundList<T> master
            ) :
            base(
                    storageobjectID,
                    file,
                    null //TODO
                ) {
            this.factoryset = factoryset;
            this.viewerset = viewerset;
            this.master = master;

            hub_obj = File.Storage[StorageObjectID];

            var propertybinders =
                new Dictionary<string, PropertyBinder<string>>();

            listener_add =
                hub_obj.CreateListen(
                        IOEvent.ChildAdded,
                        (key, objID) => {
                            int i = int.Parse(key);

                            if (Objects.Count > i &&
                                Objects.HasItemAt(i) &&
                                Objects[i].StorageObjectID == objID)
                                return;

                            var obj =
                                master == null ?
                                    FactorySet.Load(objID, File) :
                                    master[objID];

                            var namedobj =
                                obj as INamedObject;

                            if (namedobj != null) {
                                if (master == null) {
                                    var name_obj =
                                        File
                                            .Storage
                                            [objID]
                                            .GetOrMake("name");

                                    var name_val = name_obj.ReadAllString();

                                    if (map_name.ContainsKey(name_val)) {
                                        if (master == null) {
                                            if (AutomaticallyAvoidNameCollisionsWithUnderlines) {
                                                name_val += "_";
                                            }
                                            else throw new ArgumentException($"Name \"{name_val}\" already in use.");
                                        }
                                    }

                                    name_obj.WriteAllString(name_val);

                                    var binder = namedobj.Name.Bind(name_obj);
                                    namedobj.Name.AfterChange += propertybinders.Rename;
                                    propertybinders.Add(binder.Property.Value, binder);
                                    binder.Bind();
                                }
                            }

                            if (Objects.Contains(obj))
                                throw new InvalidOperationException();

                            Objects.Insert(i, obj);

                            if (master == null)
                                if (isallowedtobindobjects)
                                    obj.Bind();
                        }
                    );

            listener_remove =
                hub_obj.CreateListen(
                        IOEvent.ChildRemoved,
                        (key, objID) => {
                            var i = int.Parse(key);

                            var obj =
                                Objects.FirstOrDefault(_ => _.StorageObjectID == objID);

                            if (obj != null) {
                                var namedobj =
                                    obj as INamedObject;

                                if (namedobj != null) {
                                    if (master == null) {
                                        namedobj.Name.AfterChange -= propertybinders.Rename;

                                        propertybinders[namedobj.Name.Value].Unbind();
                                        propertybinders.Remove(namedobj.Name.Value);
                                    }
                                }

                                if (master == null)
                                    obj.Unbind();

                                Objects.Remove(obj);
                            }
                        }
                    );

            listener_move =
                hub_obj
                    .Graph
                    .CreateListen(
                            msg => {
                                var old_i = int.Parse(msg.Relation);
                                var new_i = int.Parse(msg.NewRelation);
                                
                                Objects.Move(old_i, new_i);
                            },
                            hub_obj.ID,
                            IOEvent.ChildRekeyed
                        );

            Objects.ItemInserted += (obj, i) => {
                if (!hub_obj.HasChild(obj.StorageObjectID)) {
                    if (master == null)
                        throw new InvalidOperationException();

                    hub_obj.Add(i.ToString(), obj.StorageObjectID);
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

            Objects.ItemWithdrawn += (obj, i) => {
                if (hub_obj.HasChild(obj.StorageObjectID))
                    hub_obj.Remove(obj.StorageObjectID);

                var namedobj =
                    obj as INamedObject;

                if (namedobj != null) {
                    namedobj.Name.BeforeChange -= Object_Renaming;
                    namedobj.Name.AfterChange -= Object_Renamed;

                    namedobj.Name.AfterChange -= propertybinders.Rename;

                    map_name.Remove(namedobj.Name.Value);
                    map_name_inverse.Remove(obj);
                }

                map_storageobjectID.Remove(obj.StorageObjectID);
                map_storageobjectID_inverse.Remove(obj);
            };

            Objects.ItemMoved += (item, oldindex, newindex) => {
                var sign = Math.Sign(newindex - oldindex);

                for (int i = oldindex; i != newindex; i += sign) {
                    if (isallowedtobindobjects)
                        throw new NotImplementedException();
                    else {
                        // The bound list is still loading items from storage.
                        // The 'moving' is really just initialization to sync with
                        // the back-end store, if the code ran this else clause.
                    }
                }
            };
        }

        public override void Bind() {
            isallowedtobindobjects = false;
            File.Storage.Listeners.Add(listener_add);
            File.Storage.Listeners.Add(listener_remove);
            File.Storage.Listeners.Add(listener_move);
            if (master == null) {
                foreach (var @object in Objects)
                    @object.Bind();
            }
            isallowedtobindobjects = true;

            base.Bind();
        }

        public override void Unbind() {
            File.Storage.Listeners.Remove(listener_add);
            File.Storage.Listeners.Remove(listener_remove);
            File.Storage.Listeners.Remove(listener_move);

            base.Unbind();
        }

        private void Object_Renaming(ObservableProperty<string>.PropertyChangingEventArgs args) {
            if (map_name.ContainsKey(args.NewValue)) {
                if (master == null) {
                    if (AutomaticallyAvoidNameCollisionsWithUnderlines) {
                        args.NewValue += "_";
                        args.Altered = true;
                    }
                    else throw new ArgumentException($"Name \"{args.NewValue}\" already in use.");
                }
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

            hub_obj.Add(Objects.Count.ToString(), storageobjectID);

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

        public void Move(int oldindex, int newindex) {
            Objects.Move(oldindex, newindex);
        }

        public IEnumerator<T> GetEnumerator() {
            return Objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Objects.GetEnumerator();
        }

        public bool HasItemAt(int i) {
            return Objects.HasItemAt(i);
        }
    }
}
