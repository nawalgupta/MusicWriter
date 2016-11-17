using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface IPropertyGraphlet<K> {
        T Get<T>(K item);

        void Set<T>(K item, T value);

        void Inject(IEnumerable<KeyValuePair<K, KeyValuePair<Type, object>[]>> data);

        IEnumerable<KeyValuePair<K, KeyValuePair<Type, object>[]>> Extract(params K[] subset);
    }
}
