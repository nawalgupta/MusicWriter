using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Cursor {
        public SemiTone Tone { get; set; } = SemiTone.C4;

        public Caret Caret { get; } = new Caret();
    }
}
