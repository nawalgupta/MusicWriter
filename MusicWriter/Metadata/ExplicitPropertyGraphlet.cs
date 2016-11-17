using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class ExplicitPropertyGraphlet<K> : IPropertyGraphlet<K> {
        readonly Dictionary<Type, Dictionary<K, object>> properties =
            new Dictionary<Type, Dictionary<K, object>>();

        public IEnumerable<KeyValuePair<K, KeyValuePair<Type, object>[]>> Extract(params K[] subset) {
            var buckets =
                new Dictionary<K, List<KeyValuePair<Type, object>>>();

            foreach (var property in properties) {
                List<KeyValuePair<Type, object>> list;

                foreach (var member in property.Value) {
                    if (!buckets.TryGetValue(member.Key, out list))
                        buckets.Add(member.Key, list = new List<KeyValuePair<Type, object>>());

                    list.Add(new KeyValuePair<Type, object>(property.Key, member.Value));
                }
            }

            return
                buckets
                    .Select(
                            kvp =>
                                new KeyValuePair<K, KeyValuePair<Type, object>[]>(
                                        kvp.Key,
                                        kvp.Value.ToArray()
                                    )
                        );
        }

        public void Inject(IEnumerable<KeyValuePair<K, KeyValuePair<Type, object>[]>> data) {
            Dictionary<K, object> dict;

            foreach (var member in data) {
                foreach (var property in member.Value) {
                    if (!properties.TryGetValue(property.Key, out dict))
                        properties.Add(property.Key, dict = new Dictionary<K, object>());

                    dict.Add(member.Key, property.Value);
                }
            }
        }

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
