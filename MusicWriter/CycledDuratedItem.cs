using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public struct CycledDuratedItem<T> : IDuratedItem<T> {
        readonly T value;
        readonly Duration duration;
        readonly int cycles;

        public T Value {
            get { return value; }
        }

        public Duration Duration {
            get { return duration; }
        }

        public int Cycles {
            get { return cycles; }
        }

        public CycledDuratedItem(
                T value,
                Duration duration,
                int cycles
            ) {
            this.value = value;
            this.duration = duration;
            this.cycles = cycles;
        }
    }
}
