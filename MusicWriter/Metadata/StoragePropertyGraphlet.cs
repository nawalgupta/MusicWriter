using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class StoragePropertyGraphlet<K> : IPropertyGraphlet<K>
    {
        readonly IStorageObject storage;
        readonly Dictionary<Property, IStorageObject> properties =
            new Dictionary<Property, IStorageObject>();
        readonly PropertyManager propertymanager;

        public IStorageObject Storage {
            get { return storage; }
        }

        public Dictionary<Property, Func<IStorageObject, object>> Deserializers =
            new Dictionary<Property, Func<IStorageObject, object>>();

        public Dictionary<Property, Action<IStorageObject, object>> Serializers =
            new Dictionary<Property, Action<IStorageObject, object>>();

        public PropertyManager PropertyManager {
            get { return propertymanager; }
        }

        public StoragePropertyGraphlet(
                IStorageObject storage,
                PropertyManager propertymanager
            ) {
            this.storage = storage;
            this.propertymanager = propertymanager;
        }

        public IEnumerable<KeyValuePair<K, KeyValuePair<Property, object>[]>> Extract(params K[] subset) =>
            subset
                .Select(
                        k => {
                            var k_key = k.ToString();

                            return
                            new KeyValuePair<K, KeyValuePair<Property, object>[]>(
                                    k,
                                    storage
                                        .Children
                                        .Select(
                                                property_objID => {
                                                    var propertyname =
                                                        storage.GetRelation(property_objID);

                                                    var property =
                                                        propertymanager.Access(propertyname);

                                                    var value =
                                                        Deserializers[property](storage.Graph[property_objID].Get(k_key));

                                                    var kvp =
                                                        new KeyValuePair<Property, object>(
                                                                property,
                                                                value
                                                            );

                                                    return kvp;
                                                }
                                            )
                                        .ToArray()
                                );
                        }
                    );

        public void Inject(IEnumerable<KeyValuePair<K, KeyValuePair<Property, object>[]>> data) {
            foreach (var datum in data) {
                var k_key = datum.Key.ToString();

                foreach (var kvp in datum.Value) {
                    var property_obj =
                        GetProperty(kvp.Key);

                    var k_obj =
                        property_obj.GetOrMake(k_key);

                    Serializers[kvp.Key](k_obj, kvp.Value);
                }
            }
        }

        IStorageObject GetProperty(Property property) {
            IStorageObject obj;

            if (!properties.TryGetValue(property, out obj)) {
                properties.Add(property, obj = storage.Graph.CreateObject());
                storage.Add(property.Name, obj.ID);
            }

            return obj;
        }

        public object Get(K item, Property property) {
            var property_obj = GetProperty(property);
            var k_key = item.ToString();

            if (!property_obj.HasChild(k_key))
                return property.Default;

            var k_obj = property_obj.Get(k_key);

            return Deserializers[property](k_obj);
        }

        public void Set(K item, Property property, object value) {
            var property_obj = GetProperty(property);
            var k_key = item.ToString();
            var k_obj = property_obj.GetOrMake(k_key);

            Serializers[property](k_obj, value);
        }
    }
}
