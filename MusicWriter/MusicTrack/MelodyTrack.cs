using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class MelodyTrack :
        IDurationField<Note> {
        readonly IStorageObject storage;
        IStorageObject notes_obj;

        readonly DurationField<NoteID> notes_field = new DurationField<NoteID>();
        readonly Dictionary<NoteID, Note> notes_lookup = new Dictionary<NoteID, Note>();

        IStorageObject next_noteID_obj = null;
        int next_noteID;

        public IStorageObject Storage {
            get { return storage; }
        }
        
        public event FieldChangedDelegate FieldChanged;

        public ObservableProperty<Time> Length { get; } =
            new ObservableProperty<Time>(Time.Zero);

        public Note this[NoteID noteID] {
            get { return notes_lookup[noteID]; }
            set { UpdateNote(noteID, value.Duration, value.Tone); }
        }

        public MelodyTrack(IStorageObject storage) {
            this.storage = storage;

            Setup();
        }

        void Setup() {
            next_noteID_obj = storage.GetOrMake("next_noteID");
            next_noteID_obj.ContentsSet += next_noteID_objID => {
                if (!int.TryParse(next_noteID_obj.ReadAllString(), out next_noteID))
                    next_noteID_obj.WriteAllString("0");
            };

            notes_obj = storage.GetOrMake("notes");
            notes_obj.ChildAdded += (notes_objID, new_note_objID, key) => {
                var noteID = new NoteID(int.Parse(key));
                var new_note_obj = notes_obj.Graph[new_note_objID];
                var contents = new_note_obj.ReadAllString().Split('\n');
                var duration = CodeTools.ReadDuration(contents[0]);
                var tone = new SemiTone(int.Parse(contents[1]));

                var note =
                    new Note(
                            noteID,
                            duration,
                            tone
                        );

                notes_field.Add(noteID, duration);
                notes_lookup.Add(noteID, note);
                FieldChanged?.Invoke(duration);
            };

            notes_obj.ChildContentsSet += (notes_objID, changed_note_objID, key) => {
                var noteID = new NoteID(int.Parse(key));
                var new_note_obj = notes_obj.Graph[changed_note_objID];
                var contents = new_note_obj.ReadAllString().Split('\n');
                var duration = CodeTools.ReadDuration(contents[0]);
                var tone = new SemiTone(int.Parse(contents[1]));

                Note oldnote;
                if (notes_lookup.TryGetValue(noteID, out oldnote)) {
                    if (oldnote.Duration != duration ||
                        oldnote.Tone != tone) {
                        var newnote =
                            new Note(
                                    noteID,
                                    duration,
                                    tone
                                );

                        var oldnoteduration =
                            oldnote.Duration;

                        notes_lookup[noteID] = newnote;
                        notes_field.Move(noteID, oldnoteduration, duration);
                        FieldChanged?.Invoke(oldnoteduration.Union(duration));
                    }
                }
            };

            notes_obj.ChildRemoved += (notes_objID, old_note_objID, key) => {
                var noteID = new NoteID(int.Parse(key));

                var oldnote = notes_lookup[noteID];

                notes_field.Remove(noteID, oldnote.Duration);
                notes_lookup.Remove(noteID);
                FieldChanged?.Invoke(oldnote.Duration);
            };

            notes_field.GeneralDuration.AfterChange += GeneralDuration_AfterChange;
        }

        private void GeneralDuration_AfterChange(Duration old, Duration @new) {
            Length.Value = @new.End;
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
        
        public NoteID AddNote(SemiTone tone, Duration duration) {
            var noteID = new NoteID(next_noteID++);
            next_noteID_obj.WriteAllString(next_noteID.ToString());

            var newnote_obj = notes_obj.Graph[notes_obj.Graph.Create()];

            newnote_obj.WriteAllString($"{tone.Semitones}\n{CodeTools.WriteDuration(duration)}");

            notes_obj.Add(noteID.ToString(), newnote_obj.ID);

            return noteID;
        }

        public void UpdateNote(NoteID noteID, Duration newduration, SemiTone newtone) {
            var note_obj = notes_obj.Get(noteID.ToString());

            note_obj.WriteAllString($"{newtone}\n{CodeTools.WriteDuration(newduration)}");
        }

        public void DeleteNote(NoteID noteID) {
            var note_objID = notes_obj[noteID.ToString()];

            notes_obj.Graph.Delete(note_objID);
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
