using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicWriter {
    public sealed class Note : IDuratedItem<Note> {
        readonly NoteID id;

        public NoteID ID {
            get { return id; }
        }

        public Duration Duration { get; set; }
        public float Velocity { get; set; } = 0.5f;
        public Tone Tone { get; set; } = new Tone { PitchClass = PitchClass.C, Octave = 5 };

        public Note Value {
            get { return this; }
        }

        public Note(NoteID id) {
            this.id = id;
        }
    }
}
