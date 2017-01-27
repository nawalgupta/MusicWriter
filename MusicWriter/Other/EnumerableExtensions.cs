using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public static class EnumerableExtensions {
        public static V Lookup<K, V>(
                this Dictionary<K, V> dictionary,
                K key
            ) where V : new() {
            V value;

            if (!dictionary.TryGetValue(key, out value))
                dictionary.Add(key, value = new V());

            return value;
        }

        public static void Rename<K, V>(
                this Dictionary<K, V> dictionary,
                K oldkey,
                K newkey
            ) {
            V value = dictionary[oldkey];
            dictionary.Remove(oldkey);
            dictionary.Add(newkey, value);
        }

        public static void RenameMerge<K, V, V2>(
                this Dictionary<K, V> dictionary,
                K oldkey,
                K newkey
            ) where V : ICollection<V2> {
            V value = dictionary[oldkey];
            dictionary.Remove(oldkey);

            V collection;

            if (dictionary.TryGetValue(newkey, out collection)) {
                foreach (V2 item in value)
                    collection.Add(item);
            }
            else {
                dictionary.Add(newkey, value);
            }
        }

        public static T OneOrNothing<T>(this IEnumerable<T> sequence) {
            var iter =
                sequence.GetEnumerator();

            if (!iter.MoveNext())
                return default(T);

            var item =
                iter.Current;

            if (iter.MoveNext())
                return default(T);

            return item;
        }

        public static R MaxOrDefault<T, R>(this IEnumerable<T> sequence, Func<T, R> selector)
            where R : IComparable<R> =>
            MaxOrDefault(sequence.Select(selector));

        public static T MaxOrDefault<T>(this IEnumerable<T> sequence)
            where T : IComparable<T> {
            var iter =
                sequence.GetEnumerator();

            T best = default(T);

            while (iter.MoveNext())
                if (iter.Current.CompareTo(best) > 0)
                    best = iter.Current;

            return best;
        }

        public static R MinOrDefault<T, R>(this IEnumerable<T> sequence, Func<T, R> selector)
            where R : IComparable<R> =>
            MinOrDefault(sequence.Select(selector));

        public static T MinOrDefault<T>(this IEnumerable<T> sequence)
            where T : IComparable<T> {
            var iter =
                sequence.GetEnumerator();

            T best = default(T);

            while (iter.MoveNext())
                if (iter.Current.CompareTo(best) < 0)
                    best = iter.Current;

            return best;
        }
    }
}
