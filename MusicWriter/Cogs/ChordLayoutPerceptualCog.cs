using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class ChordLayoutPerceptualCog : IPerceptualCog<ChordLayout> {
        readonly DurationField<ChordLayout> knowledge =
            new DurationField<ChordLayout>();

        public IDurationField<ChordLayout> Knowledge {
            get { return knowledge; }
        }

        public bool Analyze(Duration delta, MusicBrain brain) {
            bool flag = false;

            var builder =
                new Dictionary<Duration, List<NoteLayout>>();

            foreach (var notelayout in brain.Anlyses<NoteLayout>(delta)) {
                List<NoteLayout> list;

                if (!builder.TryGetValue(notelayout.Duration, out list))
                    builder.Add(notelayout.Duration, list = new List<NoteLayout>());

                list.Add(notelayout.Value);
            }

            foreach (var noteset in builder) {
                var chordlayout =
                    new ChordLayout(noteset.Value.ToArray());

                knowledge.Add(chordlayout, noteset.Key);

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
