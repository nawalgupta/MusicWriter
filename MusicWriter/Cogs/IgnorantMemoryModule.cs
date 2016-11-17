using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public class IgnorantMemoryModule<T> : IMemoryModule<T> {
        readonly IDurationField<T> knowledge;

        public IDurationField<T> Knowledge {
            get { return knowledge; }
        }

        public IgnorantMemoryModule(IDurationField<T> knowledge) {
            this.knowledge = knowledge;
        }

        public virtual void Forget(Duration duration) {
        }
    }
}
