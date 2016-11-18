using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms {
    public sealed class RenderedSheetMusicItemPerceptualCog :
        IPerceptualCog<RenderedSheetMusicItem> {
        public sealed class MemoryModule : IMemoryModule<RenderedSheetMusicItem> {
            public IDurationField<RenderedSheetMusicItem> Knowledge {
                get { return items; }
            }

            internal readonly DurationField<RenderedMeasure> items_measure =
                new DurationField<RenderedMeasure>();
            internal readonly DurationField<RenderedTimeSignatureSimple> items_timesigsimple =
                new DurationField<RenderedTimeSignatureSimple>();
            internal readonly DurationField<RenderedClefSymbol> items_clefsymbols =
                new DurationField<RenderedClefSymbol>();

            readonly IDurationField<RenderedSheetMusicItem> items;

            public MemoryModule() {
                items =
                    new AggregateDurationField<RenderedSheetMusicItem>(
                            items_clefsymbols,
                            items_measure,
                            items_timesigsimple
                        );
            }

            public void Forget(Duration duration) {
                foreach(var item in items.Intersecting(duration))
                    item.Value.Dispose();

                items_measure.Clear(duration);
                items_timesigsimple.Clear(duration);
                items_clefsymbols.Clear(duration);
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
                memory.MemoryModule<RenderedSheetMusicItem>();

            var layoutmeasures =
                memory.Analyses<MeasureLayout>(delta);

            foreach (var layoutmeasure in layoutmeasures) {
                if (memorymodule.items_measure.AnyItemIn(layoutmeasure.Duration))
                    continue;

                memorymodule.items_measure.Add(new RenderedMeasure(layoutmeasure.Value), layoutmeasure.Duration);
                flag = true;
            }

            var layoutsimpletimesignatures =
                memory.Analyses<TimeSignatureSimpleLayout>(delta);

            foreach (var layoutsimpletimesignature in layoutsimpletimesignatures) {
                if (memorymodule.items_timesigsimple.AnyItemIn(layoutsimpletimesignature.Duration))
                    continue;

                memorymodule.items_timesigsimple.Add(new RenderedTimeSignatureSimple(layoutsimpletimesignature.Value), layoutsimpletimesignature.Duration);
                flag = true;
            }

            return flag;
        }
    }
}
