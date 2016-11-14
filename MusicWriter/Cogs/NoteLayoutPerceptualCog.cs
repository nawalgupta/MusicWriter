using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class NoteLayoutPerceptualCog : IPerceptualCog<NoteLayout> {
        readonly DurationField<NoteLayout> knowledge =
            new DurationField<NoteLayout>();

        readonly List<PerceptualNoteID> known_notes =
            new List<PerceptualNoteID>();
        
        public IDurationField<NoteLayout> Knowledge {
            get { return knowledge; }
        }

        public bool Analyze(Duration delta, MusicBrain brain) {
            bool flag = false;

            var perceptualnotes_items =
                brain.Anlyses<PerceptualNote>(delta);

            foreach (var perceptualnote_item in perceptualnotes_items) {
                if (known_notes.Contains(perceptualnote_item.Value.ID))
                    continue;

                known_notes.Add(perceptualnote_item.Value.ID);

                var staff =
                    brain
                        .Anlyses<Staff>(perceptualnote_item.Duration)
                        .Single()
                        .Value;

                var keysignature =
                    brain
                        .Anlyses<KeySignature>(perceptualnote_item.Duration)
                        .Single()
                        .Value;

                PitchTransform transform;

                var key =
                    new DiatonicTone(
                            keysignature
                                .KeyOfPitchClass(
                                        perceptualnote_item.Value.Note.Tone.PitchClass,
                                        out transform
                                    ),
                            perceptualnote_item.Value.Note.Tone.Octave
                        );

                var halfline =
                    staff.GetHalfLine(key);

                var x =
                    0.01F *
                    (perceptualnote_item.Duration.Start / Time.Note_128th_3rd_5th_7th);
                
                var notelayout =
                    new NoteLayout(
                            perceptualnote_item.Value,
                            halfline,
                            x,
                            key,
                            transform
                        );

                knowledge.Add(notelayout, perceptualnote_item.Duration);

                flag = true;
            }

            return flag;
        }

        public void Forget(Duration delta) {
            foreach (var item in knowledge.Intersecting(delta).ToArray()) {
                knowledge.Remove(item);
                known_notes.Remove(item.Value.Core.ID);
            }
        }
    }
}
