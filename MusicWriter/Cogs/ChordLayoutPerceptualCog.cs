using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class ChordLayoutPerceptualCog : IPerceptualCog<ChordLayout> {
        public bool Analyze(
                Duration delta,
                MusicBrain brain,
                PerceptualMemory memory
            ) {
            bool flag = false;

            var memorymodule =
                (EditableMemoryModule<ChordLayout>)
                memory.MemoryModule<ChordLayout>();

            var builder =
                new Dictionary<Duration, List<NoteLayout>>();

            foreach (var notelayout in memory.Analyses<NoteLayout>(delta)) {
                List<NoteLayout> list;

                if (!builder.TryGetValue(notelayout.Duration, out list))
                    builder.Add(notelayout.Duration, list = new List<NoteLayout>());

                list.Add(notelayout.Value);
            }

            foreach (var noteset in builder) {
                var chordlayout =
                    new ChordLayout(noteset.Value.ToArray());

                memorymodule.Editable.Add(chordlayout, noteset.Key);

                flag = true;
            }

            return flag;
        }
    }
}
