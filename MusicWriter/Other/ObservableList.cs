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
    public sealed class ObservableList<T> : IList<T>
    {
        readonly ObservableCollection<T> intern =
            new ObservableCollection<T>();

        public ObservableCollection<T> ObservableCollection {
            get { return intern; }
        }

        readonly List<Action<T>> ItemAdded_responders = new List<Action<T>>();
        public event Action<T> ItemAdded {
            add {
                foreach (var item in intern)
                    value(item);

                ItemAdded_responders.Add(value);
            }
            remove {
                ItemAdded_responders.Remove(value);
            }
        }

        public event Action<T> ItemRemoved;

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
            foreach (var responder in ItemAdded_responders)
                responder(item);

            intern.Add(item);
        }

        public void Clear() {
            foreach (var item in intern)
                ItemRemoved?.Invoke(item);

            intern.Clear();
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
            foreach (var responder in ItemAdded_responders)
                responder(item);

            intern.Insert(index, item);
        }

        public bool Remove(T item) {
            if (intern.Remove(item)) {
                ItemRemoved?.Invoke(item);
                return true;
            }

            return false;
        }

        public void RemoveAt(int index) {
            var item = intern[index];
            ItemRemoved?.Invoke(item);

            intern.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            intern.GetEnumerator();
    }
}
