using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class DiatonicTone {
        public DiatonicToneClass KeyClass { get; set; }
        public int Octave { get; set; } = 0;

        public int Keys {
            get { return Octave * 7 + (int)KeyClass; }
        }

        public DiatonicTone(
                DiatonicToneClass keyclass,
                int octave
            ) {
            KeyClass = keyclass;
            Octave = octave;
        }

        public DiatonicTone(int keys) {
            KeyClass = (DiatonicToneClass)(keys % 7);
            Octave = keys / 7;
        }

        public static DiatonicTone operator +(DiatonicTone a, int b) =>
            new DiatonicTone(a.Keys + b);

        public static DiatonicTone operator -(DiatonicTone a, int b) =>
            new DiatonicTone(a.Keys - b);

        public static int operator -(DiatonicTone a, DiatonicTone b) =>
            a.Keys - b.Keys;
    }
}
