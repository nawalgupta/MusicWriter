using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class ObservableList<T> : IObservableList<T>
    {
        readonly ObservableCollection<T> intern =
            new ObservableCollection<T>();

        public ObservableCollection<T> ObservableCollection {
            get { return intern; }
        }

        readonly List<ObservableListDelegates<T>.ItemAdded> ItemAdded_responders = new List<ObservableListDelegates<T>.ItemAdded>();
        public event ObservableListDelegates<T>.ItemAdded ItemAdded {
            add {
                int i;
                lock (intern) {
                    i = intern.Count - 1;
                    ItemAdded_responders.Add(value);
                }
                for (; i >= 0; i--)
                    value(intern[i]);
            }
            remove {
                ItemAdded_responders.Remove(value);
            }
        }

        readonly List<ObservableListDelegates<T>.ItemInserted> ItemInserted_responders = new List<ObservableListDelegates<T>.ItemInserted>();
        public event ObservableListDelegates<T>.ItemInserted ItemInserted {
            add {
                int i;
                lock (intern) {
                    i = intern.Count - 1;
                    ItemInserted_responders.Add(value);
                }
                for (; i >= 0; i--)
                    value(intern[i], i);
            }
            remove {
                ItemInserted_responders.Remove(value);
            }
        }

        public event ObservableListDelegates<T>.ItemRemoved ItemRemoved;

        public event ObservableListDelegates<T>.ItemWithdrawn ItemWithdrawn;

        public event ObservableListDelegates<T>.ItemMoved ItemMoved;

        public T this[int index] {
            get {return intern[index];}
            set { intern[index] = value; }
        }

        public int Count {
            get { return intern.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public void Add(T item) {
            int i;

            lock (intern) {
                i = intern.Count;
                intern.Add(item);
            }

            foreach (var responder in ItemAdded_responders)
                responder(item);
            
            foreach (var responder in ItemInserted_responders)
                responder(item, i);
        }

        public void Clear() {
            var items = intern.ToArray();
            intern.Clear();

            for (int i = intern.Count - 1; i >= 0; i--) {
                var item = intern[i];

                ItemRemoved?.Invoke(item);
                ItemWithdrawn?.Invoke(item, i);
            }
        }

        public bool Contains(T item) =>
            intern.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) =>
            intern.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() =>
            intern.GetEnumerator();

        public int IndexOf(T item) =>
            intern.IndexOf(item);

        public void Insert(int index, T item) {
            lock (intern) {
                while (index > intern.Count)
                    intern.Add(default(T));

                intern.Insert(index, item);
            }

            foreach (var responder in ItemAdded_responders)
                responder(item);

            foreach (var responder in ItemInserted_responders)
                responder(item, index);

            for (int j = index + 1; j < intern.Count; j++)
                ItemMoved(intern[j], j - 1, j);
        }

        public bool Remove(T item) {
            var i = intern.IndexOf(item);
            if (i != -1) {
                RemoveAt(i);

                return true;
            }

            return false;
        }

        public void RemoveAt(int index) {
            var item = intern[index];
            intern.RemoveAt(index);

            ItemRemoved?.Invoke(item);
            ItemWithdrawn?.Invoke(item, index);

            for (int j = index; j < intern.Count; j++)
                ItemMoved(intern[j], j, j - 1);
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            intern.GetEnumerator();
    }
}
