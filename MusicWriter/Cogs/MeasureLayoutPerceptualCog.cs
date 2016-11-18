using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class MeasureLayoutPerceptualCog : IPerceptualCog<MeasureLayout> {
        public bool Analyze(
                Duration delta,
                MusicBrain brain,
                PerceptualMemory memory
            ) {
            bool flag = false;

            var memorymodule =
                (EditableMemoryModule<MeasureLayout>)
                memory.MemoryModule<MeasureLayout>();

            var notes =
                memory.Analyses<PerceptualNote>(delta);

            var measures =
                notes
                    .GroupBy(
                            note => memory.Analyses<Measure>(note.Duration).Single().Duration
                        );

            foreach (var measure in measures) {
                var duration =
                    measure.Key;

                if (memorymodule.Knowledge.AnyItemIn(measure.Key))
                    continue;

                var measure_notes =
                    measure;

                var keysignature =
                    memory
                        .Analyses<KeySignature>(duration)
                        .Single()
                        .Value;

                var staff =
                    memory
                        .Analyses<Staff>(duration)
                        .Single()
                        .Value;

                var measurelayout =
                    new MeasureLayout(
                            duration,
                            measure_notes.Select(note => note.Value).ToArray(),
                            staff,
                            keysignature
                        );

                memorymodule.Editable.Add(measurelayout, duration);

                flag = true;
            }

            return flag;
        }
    }
}
