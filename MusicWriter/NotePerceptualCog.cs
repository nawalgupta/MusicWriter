using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class NotePerceptualCog : IPerceptualCog<PerceptualNote> {
        readonly DurationField<PerceptualNote> knowledge =
            new DurationField<PerceptualNote>();
        readonly Dictionary<Note, PerceptualNote[]> perceptualnotes_map =
            new Dictionary<Note, PerceptualNote[]>();
        
        public IDurationField<PerceptualNote> Knowledge {
            get { return knowledge; }
        }

        public void Analyze(
                Duration duration,
                MusicBrain brain
            ) {
            var notes =
                brain.Anlyses<Note>(duration);
            
            foreach (var note in notes) {
                var cells =
                    brain.Anlyses<Cell>(note.Duration);

                if (perceptualnotes_map.ContainsKey(note)) {
                    foreach (var perceptualnote in perceptualnotes_map[note])
                        knowledge.Remove(perceptualnote);

                    perceptualnotes_map.Remove(note);
                }

                var i = 0;

                var perceptualnotes =
                    new List<PerceptualNote>();

                foreach (var cell in cells) {
                    var cellcutduration = cell.Duration.Intersection(note.Duration);

                    var lengths =
                        PerceptualTime.Decompose(cellcutduration.Length);

                    foreach (var length in lengths) {
                        var cutduration =
                            new Duration {
                                Start = length.Value,
                                Length = length.Key.TimeLength()
                            };

                        var perceptualnote =
                            new PerceptualNote(
                                new PerceptualNoteID(note.ID, i++),
                                cutduration,
                                length.Key,
                                cell
                            );

                        knowledge.Add(perceptualnote, cutduration);

                        perceptualnotes.Add(perceptualnote);
                    }
                }

                perceptualnotes_map.Add(note, perceptualnotes.ToArray());
            }
        }
    }
}
