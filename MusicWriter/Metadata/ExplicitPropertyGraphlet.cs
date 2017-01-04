using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class ExplicitPropertyGraphlet<K> : IPropertyGraphlet<K> {
        readonly Dictionary<Property, Dictionary<K, object>> properties =
            new Dictionary<Property, Dictionary<K, object>>();

        public IEnumerable<KeyValuePair<K, KeyValuePair<Property, object>[]>> Extract(params K[] subset) {
            var buckets =
                new Dictionary<K, List<KeyValuePair<Property, object>>>();

            foreach (var property in properties) {
                List<KeyValuePair<Property, object>> list;

                foreach (var member in property.Value) {
                    if (!buckets.TryGetValue(member.Key, out list))
                        buckets.Add(member.Key, list = new List<KeyValuePair<Property, object>>());

                    list.Add(new KeyValuePair<Property, object>(property.Key, member.Value));
                }
            }

            return
                buckets
                    .Select(
                            kvp =>
                                new KeyValuePair<K, KeyValuePair<Property, object>[]>(
                                        kvp.Key,
                                        kvp.Value.ToArray()
                                    )
                        );
        }

        public void Inject(IEnumerable<KeyValuePair<K, KeyValuePair<Property, object>[]>> data) {
            Dictionary<K, object> dict;

            foreach (var member in data) {
                foreach (var property in member.Value) {
                    if (!properties.TryGetValue(property.Key, out dict))
                        properties.Add(property.Key, dict = new Dictionary<K, object>());

                    dict.Add(member.Key, property.Value);
                }
            }
        }

        public object Get(K item, Property property) {
            Dictionary<K, object> lookup;

            if (!properties.TryGetValue(property, out lookup))
                properties.Add(property, lookup = new Dictionary<K, object>());

            object value;
            if (!lookup.TryGetValue(item, out value))
                value = property.Default;

            return value;
        }

        public void Set(K item, Property property, object value) {
            Dictionary<K, object> lookup;

            if (!properties.TryGetValue(property, out lookup))
                properties.Add(property, lookup = new Dictionary<K, object>());

            if (lookup.ContainsKey(item))
                lookup[item] = value;
            else
                lookup.Add(item, value);
        }
    }
}
