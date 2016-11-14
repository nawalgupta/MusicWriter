using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class MeasureLayoutPerceptualCog : IPerceptualCog<MeasureLayout> {
        readonly DurationField<MeasureLayout> knowledge =
            new DurationField<MeasureLayout>();

        public IDurationField<MeasureLayout> Knowledge {
            get { return knowledge; }
        }

        public bool Analyze(Duration delta, MusicBrain brain) {
            bool flag = false;

            var notes =
                brain.Anlyses<PerceptualNote>(delta);

            var measures =
                notes
                    .GroupBy(
                            note => brain.Anlyses<Measure>(note.Duration).Single()
                        );

            foreach (var measure in measures) {
                var duration =
                    measure.Key.Duration;

                if (knowledge.AnyItemIn(measure.Key.Duration))
                    continue;

                var measure_notes =
                    measure;

                var track =
                    brain.Track;

                var staff =
                    brain
                        .Anlyses<Staff>(duration)
                        .Single()
                        .Value;

                var measurelayout =
                    new MeasureLayout(
                            duration,
                            measure_notes.Select(note => note.Value).ToArray(),
                            track,
                            staff
                        );

                knowledge.Add(measurelayout, duration);

                flag = true;
            }

            return flag;
        }

        public void Forget(Duration delta) {
            foreach (var item in knowledge.Intersecting(delta).ToArray())
                knowledge.Remove(item);
        }
    }
}
