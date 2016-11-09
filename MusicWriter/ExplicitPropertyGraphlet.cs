using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class ExplicitPropertyGraphlet<K> : IPropertyGraphlet<K> {
        readonly Dictionary<Type, Dictionary<K, object>> properties =
            new Dictionary<Type, Dictionary<K, object>>();

        public T Get<T>(K item) {
            var type = typeof(T);
            Dictionary<K, object> lookup;

            if (!properties.TryGetValue(type, out lookup))
                properties.Add(type, lookup = new Dictionary<K, object>());

            T value = default(T);
            object outval;
            if (lookup.TryGetValue(item, out outval))
                value = (T)outval;

            return value;
        }

        public void Set<T>(K item, T value) {
            var type = typeof(T);
            Dictionary<K, object> lookup;

            if (!properties.TryGetValue(type, out lookup))
                properties.Add(type, lookup = new Dictionary<K, object>());

            if (lookup.ContainsKey(item))
                lookup[item] = value;
            else
                lookup.Add(item, value);
        }
    }
}
