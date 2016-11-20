using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class ToneCaret {
        public SemiTone Tone { get; set; }

        public Caret Caret { get; } = new Caret();
    }
}
