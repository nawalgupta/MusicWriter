using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public struct CycledItem<T> {
        readonly T value;
        readonly Time offset;
        readonly int cycles;

        public T Value {
            get { return value; }
        }

        public Time Offset {
            get { return offset; }
        }

        public int Cycles {
            get { return cycles; }
        }

        public CycledItem(
                T value,
                Time offset,
                int cycles
            ) {
            this.value = value;
            this.offset = offset;
            this.cycles = cycles;
        }
    }
}
