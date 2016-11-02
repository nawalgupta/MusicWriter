using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Clef {
        public ClefSymbol Symbol { get; set; }
        public Key BottomKey { get; set; }

        public Clef(
                ClefSymbol symbol = default(ClefSymbol),
                Key bottomkey = default(Key)
            ) {
            Symbol = symbol;
            BottomKey = bottomkey;
        }

        public static readonly Clef Custom = new Clef(ClefSymbol.Custom, KeyClass.C.ToKey(4));
        public static readonly Clef Treble = new Clef(ClefSymbol.Treble, KeyClass.E.ToKey(4));
        public static readonly Clef Bass = new Clef(ClefSymbol.Bass, KeyClass.G.ToKey(2));
        public static readonly Clef CClef = new Clef(ClefSymbol.Cclef, KeyClass.F.ToKey(3));
    }
}
