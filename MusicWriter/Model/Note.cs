using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicWriter {
    public sealed class Note :
        IDuratedItem<Note> {
        readonly NoteID id;
        readonly Duration duration;
        readonly SemiTone tone;

        public NoteID ID {
            get { return id; }
        }

        public Duration Duration {
            get { return duration; }
        }

        public SemiTone Tone {
            get { return tone; }
        }

        public Note Value {
            get { return this; }
        }

        public Note(
                    NoteID id,
                    Duration duration,
                    SemiTone tone
                ) {
            this.id = id;
            this.duration = duration;
            this.tone = tone;
        }
    }
}
