using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class PerceptualNote {
        public readonly PerceptualNoteID ID;
        public readonly LengthClass Length;
        public readonly TupletClass Tuplet;
        public readonly int Dots;
        public readonly Cell Cell;
        
        public PerceptualNote(
                PerceptualNoteID id,
                LengthClass length,
                TupletClass tuplet,
                int dots,
                Cell cell
            ) {
            ID = id;
            Length = length;
            Tuplet = tuplet;
            Dots = dots;
            Cell = cell;
        }
    }
}
