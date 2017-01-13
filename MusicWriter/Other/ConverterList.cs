using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class ConverterList<T, Special, Container>
        : IList<T> 
        where Special : T
        where Container : IList<Special> {
        readonly Container collection;
            
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

        public IList<T> RegularCollection {
            get { return this; }
        }

        public ConverterList(Container collection) {
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
