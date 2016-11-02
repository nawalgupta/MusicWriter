using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Key {
        public KeyClass KeyClass { get; set; }
        public int Octave { get; set; } = 0;

        public int Keys {
            get { return Octave * 7 + (int)KeyClass; }
        }

        public Key(
                KeyClass keyclass,
                int octave
            ) {
            KeyClass = keyclass;
            Octave = octave;
        }

        public Key(int keys) {
            KeyClass = (KeyClass)(keys % 7);
            Octave = keys / 7;
        }

        public static Key operator +(Key a, int b) =>
            new Key(a.Keys + b);

        public static Key operator -(Key a, int b) =>
            new Key(a.Keys - b);

        public static int operator -(Key a, Key b) =>
            a.Keys - b.Keys;
    }
}
