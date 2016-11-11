using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class PerceptualNote : IDuratedItem<PerceptualNote> {
        readonly Duration duration;

        public readonly PerceptualNoteID ID;
        public readonly PerceptualTime Length;
        public readonly IDuratedItem<Cell> Cell;
        public readonly Note Note;

        public Duration Duration {
            get { return duration; }
        }

        PerceptualNote IDuratedItem<PerceptualNote>.Value {
            get { return this; }
        }

        public PerceptualNote(
                PerceptualNoteID id,
                Duration duration,
                PerceptualTime length,
                IDuratedItem<Cell> cell,
                Note note
            ) {
            ID = id;
            this.duration = duration;
            Length = length;
            Cell = cell;
            Note = note;
        }
    }
}
