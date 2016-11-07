using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class PerceptualNote {
        public readonly PerceptualNoteID ID;
        public readonly Duration Duration;
        public readonly PerceptualTime Length;
        public readonly Cell Cell;
        
        public PerceptualNote(
                PerceptualNoteID id,
                Duration duration,
                PerceptualTime length,
                Cell cell
            ) {
            ID = id;
            Duration = duration;
            Length = length;
            Cell = cell;
        }
    }
}
