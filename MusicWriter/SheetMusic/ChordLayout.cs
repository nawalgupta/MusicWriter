using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class ChordLayout {
        readonly NoteLayout[] notes;

        public NoteLayout[] Notes {
            get { return notes; }
        }
        
        public float X {
            get { return notes[0].X; }
        }

        public Duration Duration {
            get { return notes[0].Core.Duration; }
        }

        public PerceptualTime Length {
            get { return notes[0].Core.Length; }
        }

        public NoteStemDirection StemDirection { get; set; }
        public float StemStartHalfLines { get; set; }
        public float FlagSlope { get; set; }
        public int Flags { get; set; }
        public float FlagLength { get; set; }
        public FlagDirection FlagDirection { get; set; }

        public ChordLayout(NoteLayout[] notes) {
            this.notes = notes;
        }
    }
}
