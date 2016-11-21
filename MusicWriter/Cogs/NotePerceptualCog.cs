using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class NotePerceptualCog : IPerceptualCog<PerceptualNote> {
        public sealed class MemoryModule : EditableMemoryModule<PerceptualNote> {
            internal readonly Dictionary<NoteID, PerceptualNote[]> perceptualnotes_map =
                new Dictionary<NoteID, PerceptualNote[]>();

            public override void Forget(Duration duration) {
                var notes =
                    Editable
                        .Intersecting(duration)
                        .Select(perceptualnote => perceptualnote.Value.Note)
                        .Distinct();
                
                foreach (var note in notes)
                    perceptualnotes_map.Remove(note.ID);

                base.Forget(duration);
            }
        }

        public bool Analyze(
                Duration duration,
                MusicBrain brain,
                PerceptualMemory memory
            ) {
            bool flag = false;

            var memorymodule =
                (MemoryModule)
                memory.MemoryModule<PerceptualNote>();

            var notes =
                memory.Analyses<Note>(duration);
            
            foreach (Note note in notes) {
                if (memorymodule.perceptualnotes_map.ContainsKey(note.ID))
                    continue;

                var perceptualnotes =
                    new List<PerceptualNote>();

                var singlelength =
                    PerceptualTime.Decompose(note.Duration.Length).OneOrNothing();

                var cells =
                    memory.Analyses<Cell>(note.Duration);

                if (singlelength.Key == default(PerceptualTime)) {
                    var i = 0;

                    foreach (var cell_durateditem in cells) {
                        var cell = cell_durateditem.Value;
                        var cellduration = cell_durateditem.Duration;

                        var cellcutduration = cellduration.Intersection(note.Duration);

                        var lengths =
                            PerceptualTime.Decompose(cellcutduration.Length);

                        foreach (var length in lengths) {
                            var cutduration =
                                new Duration {
                                    Start = length.Value + cellduration.Start,
                                    Length = length.Key.TimeLength()
                                };

                            var perceptualnote =
                                new PerceptualNote(
                                    new PerceptualNoteID(note.ID, i++),
                                    cutduration,
                                    length.Key,
                                    cell_durateditem,
                                    note
                                );

                            memorymodule.Editable.Add(perceptualnote, cutduration);

                            perceptualnotes.Add(perceptualnote);
                        }
                    }

                    memorymodule.perceptualnotes_map.Add(note.ID, perceptualnotes.ToArray());
                }
                else {
                    var perceptualnote =
                        new PerceptualNote(
                                new PerceptualNoteID(
                                        note.ID,
                                        0
                                    ),
                                note.Duration,
                                singlelength.Key,
                                cells.First(),
                                note
                            );
                    
                    memorymodule.Editable.Add(perceptualnote, note.Duration);

                    memorymodule.perceptualnotes_map.Add(note.ID, new[] { perceptualnote });
                }
                
                flag = true;
            }

            return flag;
        }
    }
}
