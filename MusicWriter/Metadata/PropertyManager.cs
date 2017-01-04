using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class PropertyManager
    {
        readonly IStorageObject storage;

        readonly Dictionary<string, Property> properties =
            new Dictionary<string, Property>();

        public IStorageObject Storage {
            get { return storage; }
        }

        int next_id {
            get { return int.Parse(storage.Get("next_id").ReadAllString()); }
            set { storage.Get("next_id").WriteAllString(value.ToString()); }
        }

        public PropertyManager(IStorageObject storage) {
            this.storage = storage;

            if (!storage.HasChild("next_id"))
                storage.GetOrMake("next_id").WriteAllString("0");
        }

        public Property Access(string name, object @default = null) {
            Property property;

            if (!properties.TryGetValue(name, out property)) {
                var obj = storage.GetOrMake(name);

                if (obj.IsEmpty)
                    obj.WriteAllString(next_id++.ToString());

                properties.Add(name, property = new Property(name, @default, int.Parse(obj.ReadAllString())));
            }

            return property;
        }
    }
}
