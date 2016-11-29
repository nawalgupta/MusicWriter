using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class NoteLayoutPerceptualCog : IPerceptualCog<NoteLayout> {
        public sealed class MemoryModule : EditableMemoryModule<NoteLayout> {
            internal readonly List<PerceptualNoteID> known_notes =
                new List<PerceptualNoteID>();

            public override void Forget(Duration duration) {
                foreach (var note in Knowledge.Intersecting(duration))
                    known_notes.Remove(note.Value.Core.ID);

                base.Forget(duration);
            }
        }

        public bool Analyze(
                Duration delta,
                MusicBrain brain,
                PerceptualMemory memory
            ) {
            bool flag = false;

            var memorymodule =
                (MemoryModule)
                memory.MemoryModule<NoteLayout>();

            var perceptualnotes_items =
                memory.Analyses<PerceptualNote>(delta);

            foreach (var perceptualnote_item in perceptualnotes_items) {
                if (memorymodule.known_notes.Contains(perceptualnote_item.Value.ID))
                    continue;

                memorymodule.known_notes.Add(perceptualnote_item.Value.ID);

                var staff =
                    memory
                        .Analyses<Staff>(perceptualnote_item.Duration)
                        .Single()
                        .Value;

                var keysignature =
                    memory
                        .Analyses<KeySignature>(perceptualnote_item.Duration)
                        .Single()
                        .Value;

                PitchTransform transform;

                var key =
                    keysignature.Key(
                            perceptualnote_item.Value.Note.Tone,
                            out transform
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
                            0,
                            key,
                            transform
                        );

                memorymodule.Editable.Add(notelayout, perceptualnote_item.Duration);

                flag = true;
            }

            return flag;
        }
    }
}
