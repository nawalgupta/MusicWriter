using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Staff {
        public Clef Clef { get; set; }
        public int Shift { get; set; }
        public int Lines { get; set; } = 5;
        public int MiddleHalfLine { get; set; } = 4;

        // Line 0 is the bottom line.
        // Half-line 4 would be the center of 5 whole-lines.
        
        public int GetHalfLine(Key key) =>
            key - Clef.BottomKey + Shift;

        public NoteStemDirection GetStemDirection(Key key) =>
            GetHalfLine(key) >= MiddleHalfLine ?
                NoteStemDirection.Down :
                NoteStemDirection.Up;
        
        public Staff(
                Clef clef = default(Clef),
                int lines = 5
            ) {
            Clef = clef;
            Lines = lines;
        }

        public static readonly Staff Custom = new Staff(Clef.Custom, 8);
        public static readonly Staff Treble = new Staff(Clef.Treble);
        public static readonly Staff CClef = new Staff(Clef.CClef);
        public static readonly Staff Bass = new Staff(Clef.Bass);
    }
}
