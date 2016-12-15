using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class MelodyTrack :
        IDurationField<Note> {
        readonly DurationField<NoteID> notes_field = new DurationField<NoteID>();
        readonly Dictionary<NoteID, Note> notes_lookup = new Dictionary<NoteID, Note>();
        int next_noteID;

        public int Next_NoteID {
            get { return next_noteID; }
        }

        public event Action FieldChanged;

        public Note this[NoteID noteID] {
            get { return notes_lookup[noteID]; }
            set { UpdateNote(noteID, value.Duration, value.Tone); }
        }

        public MelodyTrack(int next_noteID = 0) {
            this.next_noteID = next_noteID;
        }

        public IEnumerable<Note> NotesInTime(Duration duration) =>
            notes_field
                .Intersecting(duration)
                .Select(
                        noteID_item =>
                            notes_lookup[noteID_item.Value]
                    );

        public IEnumerable<Note> AllNotes() =>
            notes_lookup.Values;

        public void AddNote(Note note) {
            notes_field.Add(note.ID, note.Duration);
            notes_lookup.Add(note.ID, note);

            next_noteID = Math.Max(next_noteID, note.ID.ID + 1);

            FieldChanged?.Invoke();
        }

        public Note AddNote(SemiTone tone, Duration duration) {
            var noteID = new NoteID(next_noteID++);

            notes_field.Add(noteID, duration);

            var note =
                new Note(
                        noteID,
                        duration,
                        tone
                    );

            notes_lookup.Add(noteID, note);

            FieldChanged?.Invoke();

            return note;
        }

        public void UpdateNote(NoteID noteID, Duration newduration, SemiTone newtone) {
            var newnote =
                new Note(
                        noteID,
                        newduration,
                        newtone
                    );

            var oldnoteduration =
                notes_lookup[noteID].Duration;

            notes_lookup[noteID] = newnote;

            notes_field.Remove(noteID, oldnoteduration);
            notes_field.Add(noteID, newduration);

            FieldChanged?.Invoke();
        }

        public void DeleteNote(NoteID noteID) {
            notes_field.Remove(noteID, notes_lookup[noteID].Duration);
            notes_lookup.Remove(noteID);

            FieldChanged?.Invoke();
        }

        public IEnumerable<IDuratedItem<Note>> Intersecting(Time point) =>
            notes_field
                .Intersecting(point)
                .Select(noteID => notes_lookup[noteID.Value]);

        public IEnumerable<IDuratedItem<Note>> Intersecting(Duration duration) =>
            notes_field
                .Intersecting(duration)
                .Select(noteID => notes_lookup[noteID.Value]);

        public bool HasNoteID(NoteID noteID) =>
            notes_lookup.ContainsKey(noteID);

        public bool TryGetNote(NoteID noteID, out Note note) =>
            notes_lookup.TryGetValue(noteID, out note);
    }
}
