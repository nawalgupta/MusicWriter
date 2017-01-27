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

        readonly Dictionary<Action<T>, Action<Special>> ItemAdded_responders_map =
            new Dictionary<Action<T>, Action<Special>>();
        public event Action<T> ItemAdded {
            add {
                Action<Special> converter =
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

        readonly Dictionary<Action<T, int>, Action<Special, int>> ItemInserted_responders_map =
            new Dictionary<Action<T, int>, Action<Special, int>>();
        public event Action<T, int> ItemInserted {
            add {
                Action<Special, int> converter =
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

        readonly Dictionary<Action<T>, Action<Special>> ItemRemoved_responders_map =
            new Dictionary<Action<T>, Action<Special>>();
        public event Action<T> ItemRemoved {
            add {
                Action<Special> converter =
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

        readonly Dictionary<Action<T, int>, Action<Special, int>> ItemWithdrawn_responders_map =
            new Dictionary<Action<T, int>, Action<Special, int>>();
        public event Action<T, int> ItemWithdrawn {
            add {
                Action<Special, int> converter =
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
    }
}
