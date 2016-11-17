using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface IMemoryModule<out T> {
        IDurationField<T> Knowledge { get; }

        void Forget(Duration duration);
    }
}
