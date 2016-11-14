using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class SemiTone {
        public ChromaticPitchClass PitchClass { get; set; } = ChromaticPitchClass.C;
        public int Octave { get; set; } = 0;

        public static readonly SemiTone C5 = new SemiTone { PitchClass = ChromaticPitchClass.C, Octave = 5 };
    }
}
