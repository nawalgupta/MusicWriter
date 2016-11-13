﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class NotePerceptualCog : IPerceptualCog<PerceptualNote> {
        readonly DurationField<PerceptualNote> knowledge =
            new DurationField<PerceptualNote>();
        readonly Dictionary<NoteID, PerceptualNote[]> perceptualnotes_map =
            new Dictionary<NoteID, PerceptualNote[]>();
        
        public IDurationField<PerceptualNote> Knowledge {
            get { return knowledge; }
        }

        public bool Analyze(
                Duration duration,
                MusicBrain brain
            ) {
            bool flag = false;

            var notes =
                brain.Anlyses<Note>(duration);
            
            foreach (Note note in notes) {
                if (perceptualnotes_map.ContainsKey(note.ID))
                    continue;

                var perceptualnotes =
                    new List<PerceptualNote>();

                var singlelength =
                    PerceptualTime.Decompose(note.Duration.Length).SingleOrDefault();

                if (singlelength.Key == default(PerceptualTime)) {
                    var cells =
                        brain.Anlyses<Cell>(note.Duration);

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

                            knowledge.Add(perceptualnote, cutduration);

                            perceptualnotes.Add(perceptualnote);
                        }
                    }

                    perceptualnotes_map.Add(note.ID, perceptualnotes.ToArray());
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
                                null,
                                note
                            );
                    
                    knowledge.Add(perceptualnote, note.Duration);

                    perceptualnotes_map.Add(note.ID, new[] { perceptualnote });
                }
                
                flag = true;
            }

            return flag;
        }

        public void Forget(Duration delta) {
            var notes =
                knowledge
                    .Intersecting(delta)
                    .Select(perceptualnote => perceptualnote.Value.Note)
                    .Distinct();

            foreach (var note in notes) {
                foreach (var perceptualnote in perceptualnotes_map[note.ID])
                    knowledge.Remove(perceptualnote);

                perceptualnotes_map.Remove(note.ID);
            }
        }
    }
}