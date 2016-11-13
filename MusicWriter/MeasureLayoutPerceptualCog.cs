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

            foreach (var measure in brain.Anlyses<Measure>(delta)) {
                if (knowledge.AnyItemIn(measure.Duration))
                    continue;

                var notes =
                    brain.Anlyses<PerceptualNote>(measure.Duration);

                var track =
                    brain.Track;

                var staff =
                    brain
                        .Anlyses<Staff>(measure.Duration)
                        .Single()
                        .Value;

                var measurelayout =
                    new MeasureLayout(
                            measure.Duration,
                            notes.Select(note => note.Value).ToArray(),
                            track,
                            staff
                        );

                knowledge.Add(measurelayout, measure.Duration);

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
