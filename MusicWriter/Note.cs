using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicWriter {
    public sealed class Note {
        readonly NoteID id;

        public NoteID ID {
            get { return id; }
        }

        public Duration Duration { get; }
        public float Velocity { get; set; } = 0.5f;
        public Tone Tone { get; set; } = new Tone();

        public Note(NoteID id) {
            this.id = id;
        }
    }
}
