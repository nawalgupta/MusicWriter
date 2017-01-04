using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface IPropertyGraphlet<K> {
        object Get(K item, Property property);

        void Set(K item, Property property, object value);

        void Inject(IEnumerable<KeyValuePair<K, KeyValuePair<Property, object>[]>> data);

        IEnumerable<KeyValuePair<K, KeyValuePair<Property, object>[]>> Extract(params K[] subset);
    }
}
