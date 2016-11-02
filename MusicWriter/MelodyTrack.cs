using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class MelodyTrack {
        readonly DurationField<NoteID> notes_field = new DurationField<NoteID>();
        readonly Dictionary<NoteID, Note> notes_lookup = new Dictionary<NoteID, Note>();
        readonly RhythmTrack rhythm;
        readonly DurationField<KeySignature> keysignatures;

        public RhythmTrack Rhythm {
            get { return rhythm; }
        }

        public DurationField<KeySignature> KeySignatures {
            get { return keysignatures; }
        }

        public MelodyTrack(
                RhythmTrack rhythm,
                DurationField<KeySignature> keysignatures
            ) {
            this.rhythm = rhythm;
            this.keysignatures = keysignatures;
        }

        public IEnumerable<Note> NotesInTime(Duration duration) =>
            notes_field
                .Intersecting(duration)
                .Select(
                        noteID =>
                            notes_lookup[noteID]
                    );

        public void UpdateNote(Note note) {
            notes_field[note.ID] = note.Duration;
        }

        public void DeleteNote(Note note) {
            notes_lookup.Remove(note.ID);
            notes_field.Remove(note.ID);
        }
    }
}
