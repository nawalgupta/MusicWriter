using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public struct SemiTone {
        public ChromaticPitchClass PitchClass;
        public int Octave;
        
        public int Semitones {
            get { return Octave * 12 + (int)PitchClass; }
        }
        
        public SemiTone(int semitones) {
            Octave = semitones / 12;
            PitchClass = (ChromaticPitchClass)(semitones % 12);
        }

        public SemiTone(ChromaticPitchClass pitch, int octave) {
            PitchClass = pitch;
            Octave = octave;
        }

        public static readonly SemiTone C5 = new SemiTone(ChromaticPitchClass.C, 5);
        public static readonly SemiTone Zero = new SemiTone(ChromaticPitchClass.C, 0);
        public static readonly SemiTone NegativeOne = new SemiTone(-1);
        public static readonly SemiTone PositiveOne = new SemiTone(+1);

        public static SemiTone operator +(SemiTone a, SemiTone delta) =>
            new SemiTone(a.Semitones + delta.Semitones);

        public static SemiTone operator -(SemiTone a, SemiTone b) =>
            new SemiTone(a.Semitones - b.Semitones);
    }
}
