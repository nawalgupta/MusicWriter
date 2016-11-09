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
        public readonly Note Note;

        public PerceptualNote(
                PerceptualNoteID id,
                Duration duration,
                PerceptualTime length,
                Cell cell,
                Note note
            ) {
            ID = id;
            Duration = duration;
            Length = length;
            Cell = cell;
            Note = note;
        }
    }
}
