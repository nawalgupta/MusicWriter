using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public class EditableMemoryModule<T> : IMemoryModule<T> {
        readonly DurationField<T> knowledge =
            new DurationField<T>();

        public IDurationField<T> Knowledge {
            get { return knowledge; }
        }

        public DurationField<T> Editable {
            get { return knowledge; }
        }

        public virtual void Forget(Duration duration) {
            foreach (var item in knowledge.Intersecting(duration))
                knowledge.Remove(item);
        }
    }
}
