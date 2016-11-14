using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Clef {
        public ClefSymbol Symbol { get; set; }
        public DiatonicTone BottomKey { get; set; }

        public Clef(
                ClefSymbol symbol = default(ClefSymbol),
                DiatonicTone bottomkey = default(DiatonicTone)
            ) {
            Symbol = symbol;
            BottomKey = bottomkey;
        }

        public static readonly Clef Custom = new Clef(ClefSymbol.Custom, DiatonicToneClass.C.ToKey(4));
        public static readonly Clef Treble = new Clef(ClefSymbol.Treble, DiatonicToneClass.E.ToKey(4));
        public static readonly Clef Bass = new Clef(ClefSymbol.Bass, DiatonicToneClass.G.ToKey(2));
        public static readonly Clef CClef = new Clef(ClefSymbol.Cclef, DiatonicToneClass.F.ToKey(3));
    }
}
