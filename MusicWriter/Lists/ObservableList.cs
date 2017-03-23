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
        readonly object locker = new object();
        readonly List<T> intern = new List<T>();
        readonly ShiftableBitArray status = new ShiftableBitArray();
        
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
                for (int j = 0; j <= i; j++)
                    value(intern[j], j);
            }
            remove {
                ItemInserted_responders.Remove(value);
            }
        }

        public event ObservableListDelegates<T>.ItemRemoved ItemRemoved;

        public event ObservableListDelegates<T>.ItemWithdrawn ItemWithdrawn;

        public event ObservableListDelegates<T>.ItemMoved ItemMoved;

        public T this[int index] {
            get {
                if (!status[index])
                    throw new IndexOutOfRangeException();

                return intern[index];
            }
            set {
                if (!status[index])
                    throw new IndexOutOfRangeException();

                intern[index] = value;
            }
        }

        public int Count {
            get { return intern.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public void Add(T item) {
            int i;

            lock (locker) {
                i = intern.Count;
                intern.Add(item);
                status[i] = true;
            }

            foreach (var responder in ItemAdded_responders)
                responder(item);
            
            foreach (var responder in ItemInserted_responders)
                responder(item, i);
        }

        public void Clear() {
            T[] items;

            lock (locker) {
                items = intern.ToArray();
                intern.Clear();
                status.Clear();
            }

            for (int i = items.Length - 1; i >= 0; i--) {
                var item = items[i];

                ItemRemoved?.Invoke(item);
                ItemWithdrawn?.Invoke(item, i);
            }
        }

        public bool Contains(T item) =>
            intern.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) =>
            intern.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() {
            for (int i = 0; i < intern.Count; i++)
                if (status[i])
                    yield return intern[i];
        }

        public int IndexOf(T item) =>
            intern.IndexOf(item);

        public void Insert(int index, T item) {
            lock (locker) {
                while (index > intern.Count)
                    intern.Add(default(T));

                if (!status[index] && index < intern.Count) {
                    intern[index] = item;
                    status[index] = true;
                }
                else {
                    intern.Insert(index, item);
                    status.Insert(index, true);
                }
            }

            foreach (var responder in ItemAdded_responders)
                responder(item);

            foreach (var responder in ItemInserted_responders)
                responder(item, index);

            for (int j = index + 1; j < intern.Count && status[j]; j++)
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
            T item;

            lock (locker) {
                item = intern[index];
                intern.RemoveAt(index);
                status.Withdraw(index);
            }

            ItemRemoved?.Invoke(item);
            ItemWithdrawn?.Invoke(item, index);

            for (int j = index; j < intern.Count; j++)
                ItemMoved(intern[j], j, j - 1);
        }

        public void Move(int oldindex, int newindex) {
            T item;

            lock (locker) {
                item = intern[oldindex];
                status[oldindex] = false;

                while (newindex >= intern.Count)
                    intern.Add(default(T));

                if (status[newindex] == true) {
                    var olditem = intern[newindex];

                    ItemRemoved?.Invoke(olditem);
                    ItemWithdrawn?.Invoke(olditem, newindex);
                }
                else {
                    status[newindex] = true;
                }

                intern[newindex] = item;
            }

            ItemMoved?.Invoke(item, oldindex, newindex);
        }

        public bool HasItemAt(int i) =>
            status[i];

        IEnumerator IEnumerable.GetEnumerator() =>
            intern.GetEnumerator();
    }
}
