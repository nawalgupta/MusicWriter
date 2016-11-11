using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface IDurationField<out T> {
        IEnumerable<IDuratedItem<T>> Intersecting(Time point);

        IEnumerable<IDuratedItem<T>> Intersecting(Duration duration);
    }
}
