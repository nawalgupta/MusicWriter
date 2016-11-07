using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public struct PerceptualTime {
        public readonly TupletClass Tuplet;
        public readonly LengthClass Length;
        public readonly int Dots;
        
        public PerceptualTime(
                TupletClass tuplet,
                LengthClass length,
                int dots
            ) {
            Tuplet = tuplet;
            Length = length;
            Dots = dots;
        }
    }
}
