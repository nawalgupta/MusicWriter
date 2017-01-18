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

            var measures =
                memory.Analyses<Measure>(delta);

            foreach (var measure in measures) {
                var duration =
                    measure.Duration;

                if (memorymodule.Knowledge.AnyItemIn(duration))
                    continue;

                var measure_notes =
                    memory
                        .Analyses<PerceptualNote>(duration)
                        .Select(note => note.Value)
                        .ToArray();

                var keysignature =
                    memory
                        .Analyses<KeySignature>(duration)
                        .First()
                        .Value;

                var staff =
                    memory
                        .Analyses<Staff>(duration)
                        .First()
                        .Value;

                var measurelayout =
                    new MeasureLayout(
                            duration,
                            measure_notes,
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
