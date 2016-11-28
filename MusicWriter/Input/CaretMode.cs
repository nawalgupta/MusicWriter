using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    [Flags]
    public enum CaretMode {
        Delta = 0x01,
        Absolute = 0x02,
        SemiTones = 0x04,
        WholeTones = 0x08,
    }
}
