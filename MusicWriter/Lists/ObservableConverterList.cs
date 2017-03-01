using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class ObservableConverterList<T, Special, Container>
        : IObservableList<T>
        where Special : T
        where Container : IObservableList<Special>
    {
        readonly Container collection;

        readonly Dictionary<ObservableListDelegates<T>.ItemAdded, ObservableListDelegates<Special>.ItemAdded> ItemAdded_responders_map =
            new Dictionary<ObservableListDelegates<T>.ItemAdded, ObservableListDelegates<Special>.ItemAdded>();
        public event ObservableListDelegates<T>.ItemAdded ItemAdded {
            add {
                ObservableListDelegates<Special>.ItemAdded converter =
                    (Special special) =>
                        value(special);

                ItemAdded_responders_map.Add(value, converter);

                collection.ItemAdded += converter;
            }
            remove {
                var special =
                    ItemAdded_responders_map[value];

                ItemAdded_responders_map.Remove(value);

                collection.ItemAdded -= special;
            }
        }

        readonly Dictionary<ObservableListDelegates<T>.ItemInserted, ObservableListDelegates<Special>.ItemInserted> ItemInserted_responders_map =
            new Dictionary<ObservableListDelegates<T>.ItemInserted, ObservableListDelegates<Special>.ItemInserted>();
        public event ObservableListDelegates<T>.ItemInserted ItemInserted {
            add {
                ObservableListDelegates<Special>.ItemInserted converter =
                    (Special special, int index) =>
                        value(special, index);

                ItemInserted_responders_map.Add(value, converter);

                collection.ItemInserted += converter;
            }
            remove {
                var special =
                    ItemInserted_responders_map[value];

                ItemInserted_responders_map.Remove(value);

                collection.ItemInserted -= special;
            }
        }

        readonly Dictionary<ObservableListDelegates<T>.ItemRemoved, ObservableListDelegates<Special>.ItemRemoved> ItemRemoved_responders_map =
            new Dictionary<ObservableListDelegates<T>.ItemRemoved, ObservableListDelegates<Special>.ItemRemoved>();
        public event ObservableListDelegates<T>.ItemRemoved ItemRemoved {
            add {
                ObservableListDelegates<Special>.ItemRemoved converter =
                    (Special special) =>
                        value(special);

                ItemRemoved_responders_map.Add(value, converter);

                collection.ItemRemoved += converter;
            }
            remove {
                var special =
                    ItemRemoved_responders_map[value];

                ItemRemoved_responders_map.Remove(value);

                collection.ItemRemoved -= special;
            }
        }

        readonly Dictionary<ObservableListDelegates<T>.ItemWithdrawn, ObservableListDelegates<Special>.ItemWithdrawn> ItemWithdrawn_responders_map =
            new Dictionary<ObservableListDelegates<T>.ItemWithdrawn, ObservableListDelegates<Special>.ItemWithdrawn>();
        public event ObservableListDelegates<T>.ItemWithdrawn ItemWithdrawn {
            add {
                ObservableListDelegates<Special>.ItemWithdrawn converter =
                    (Special special, int index) =>
                        value(special, index);

                ItemWithdrawn_responders_map.Add(value, converter);

                collection.ItemWithdrawn += converter;
            }
            remove {
                var special =
                    ItemWithdrawn_responders_map[value];

                ItemWithdrawn_responders_map.Remove(value);

                collection.ItemWithdrawn -= special;
            }
        }

        readonly Dictionary<ObservableListDelegates<T>.ItemMoved, ObservableListDelegates<Special>.ItemMoved> ItemMoved_responders_map =
            new Dictionary<ObservableListDelegates<T>.ItemMoved, ObservableListDelegates<Special>.ItemMoved>();
        public event ObservableListDelegates<T>.ItemMoved ItemMoved {
            add {
                ObservableListDelegates<Special>.ItemMoved converter =
                    (Special special, int oldindex, int newindex) =>
                        value(special, oldindex, newindex);

                ItemMoved_responders_map.Add(value, converter);

                collection.ItemMoved += converter;
            }
            remove {
                var special =
                    ItemMoved_responders_map[value];

                ItemMoved_responders_map.Remove(value);

                collection.ItemMoved -= special;
            }
        }

        public T this[int index] {
            get {
                return collection[index];
            }

            set {
                collection[index] = (Special)value;
            }
        }

        public int Count {
            get {
                return collection.Count;
            }
        }

        public bool IsReadOnly {
            get {
                return ((IList<T>)collection).IsReadOnly;
            }
        }

        public Container SpecialCollection {
            get { return collection; }
        }

        public IObservableList<T> RegularCollection {
            get { return this; }
        }

        public ObservableConverterList(Container collection) {
            this.collection = collection;
        }

        public void Add(T item) {
            collection.Add((Special)item);
        }

        public void Clear() {
            collection.Clear();
        }

        public bool Contains(T item) {
            return collection.Contains((Special)item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            for (int i = arrayIndex + collection.Count - 1, j = collection.Count - 1; i >= 0; i--, j--)
                array[i] = collection[j];
        }

        public IEnumerator<T> GetEnumerator() {
            return collection.Cast<T>().GetEnumerator();
        }

        public int IndexOf(T item) {
            return collection.IndexOf((Special)item);
        }

        public void Insert(int index, T item) {
            collection.Insert(index, (Special)item);
        }

        public bool Remove(T item) {
            return collection.Remove((Special)item);
        }

        public void RemoveAt(int index) {
            collection.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return collection.GetEnumerator();
        }

        public void Move(int oldindex, int newindex) =>
            collection.Move(oldindex, newindex);

        public bool HasItemAt(int i) {
            return RegularCollection.HasItemAt(i);
        }
    }
}
