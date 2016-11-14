using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    internal static class InternalExtensions {
        public static V Lookup<K, V>(
                this Dictionary<K, V> dictionary,
                K key
            ) where V : new() {
            V value;

            if (!dictionary.TryGetValue(key, out value))
                dictionary.Add(key, value = new V());

            return value;
        }
    }
}
