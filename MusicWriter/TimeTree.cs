using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class TimeTree<V> {
        readonly Time pivot;
        readonly List<V> items = new List<V>();
        TimeTree<V> left, right;

        public List<V> Items {
            get { return items; }
        }

        private TimeTree(Time pivot) {
            this.pivot = pivot;
        }

        public TimeTree()
            : this(default(Time)) {
        }

        public Time this[V item, Time time] {
            get { return time; }
            set {
                Remove(item, time);
                Add(item, time);
            }
        }

        public void Remove(V item, Time time) {
            if (time == pivot)
                items.Remove(item);
            else if (time > pivot)
                right.Remove(item, time);
            else // time < pivot
                left.Remove(item, time);
        }

        public void Clear() {
            left = null;
            right = null;
            items.Clear();
        }

        public void Add(V item, Time time) {
            if (time == pivot)
                items.Add(item);
            else if (time < pivot) {
                if (left == null)
                    left = new TimeTree<V>(time);

                left.Add(item, time);
            }
            else { // time > pivot
                if (right == null)
                    right = new TimeTree<V>(time);

                right.Add(item, time);
            }
        }

        public IEnumerable<KeyValuePair<Time, V>> All() {
            foreach (var item in items)
                yield return new KeyValuePair<Time, V>(pivot, item);

            if (left != null)
                foreach (var item in left.All())
                    yield return item;

            if (right != null)
                foreach (var item in right.All())
                    yield return item;
        }

        public IEnumerable<KeyValuePair<Time, V>> BeforeOrAt(Time bar) {
            if (pivot <= bar)
                foreach (var item in All())
                    yield return item;

            if (left != null) {
                if (pivot <= bar)
                    foreach (var item in left.All())
                        yield return item;
                else
                    foreach (var item in left.BeforeOrAt(bar))
                        yield return item;
            }

            if (right != null)
                foreach (var item in right.BeforeOrAt(bar))
                    yield return item;
        }

        public IEnumerable<KeyValuePair<Time, V>> AfterOrAt(Time bar) {
            if (pivot >= bar)
                foreach (var item in All())
                    yield return item;

            if (right != null) {
                if (pivot >= bar)
                    foreach (var item in right.All())
                        yield return item;
                else
                    foreach (var item in right.AfterOrAt(bar))
                        yield return item;
            }

            if (left != null)
                foreach (var item in left.AfterOrAt(bar))
                    yield return item;
        }

        public IEnumerable<KeyValuePair<Time, V>> Before(Time bar) {
            if (left != null) {
                if (pivot <= bar)
                    foreach (var item in left.All())
                        yield return item;
                else
                    foreach (var item in left.BeforeOrAt(bar))
                        yield return item;
            }

            if (right != null)
                foreach (var item in right.BeforeOrAt(bar))
                    yield return item;
        }

        public IEnumerable<KeyValuePair<Time, V>> After(Time bar) {
            if (right != null) {
                if (pivot >= bar)
                    foreach (var item in right.All())
                        yield return item;
                else
                    foreach (var item in right.After(bar))
                        yield return item;
            }

            if (left != null)
                foreach (var item in left.After(bar))
                    yield return item;
        }
    }
}
