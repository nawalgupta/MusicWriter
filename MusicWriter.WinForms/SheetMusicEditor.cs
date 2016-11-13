using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms {
    public partial class SheetMusicEditor :
        Control,
        IPerceptualCog<RenderedSheetMusicItem> {
        public float ScrollX { get; set; } = 0;
        public NoteCaret Caret { get; } = new NoteCaret();
        public MusicBrain Brain { get; set; }
        public SheetMusicRenderSettings Settings { get; set; } = new SheetMusicRenderSettings();

        public void LoadFrom(MusicEditorFile file, string track) {
            Settings = new SheetMusicRenderSettings();
            Brain = file.BrainFor(track);

            Brain.InsertCog(this);
            Brain.Invalidate(Duration.Eternity);

            Invalidate();
        }

        public Track Track { get; set; }

        public IDurationField<RenderedSheetMusicItem> Knowledge {
            get { return items; }
        }

        readonly DurationField<RenderedMeasure> items_measure =
            new DurationField<RenderedMeasure>();
        readonly DurationField<RenderedTimeSignatureSimple> items_timesigsimple =
            new DurationField<RenderedTimeSignatureSimple>();
        readonly DurationField<RenderedClefSymbol> items_clefsymbols =
            new DurationField<RenderedClefSymbol>();

        readonly IDurationField<RenderedSheetMusicItem> items;

        public SheetMusicEditor() {
            InitializeComponent();

            items =
                new AggregateDurationField<RenderedSheetMusicItem>(
                        items_clefsymbols,
                        items_measure,
                        items_timesigsimple
                    );
        }

        protected override void OnPaint(PaintEventArgs pe) {
            // draw staff
            if (Settings?.Staff != null)
                for (int line = 0; line < Settings.Staff.Lines; line++)
                    pe.Graphics.DrawLine(
                            Pens.Black,
                            0,
                            Settings.YVal(line * 2),
                            Width,
                            Settings.YVal(line * 2)
                        );

            // draw sheet items
            var rendered = new List<RenderedSheetMusicItem>();
            
            var starttimes =
                items
                    .Intersecting(Duration.Eternity)
                    .Select(item => item.Duration)
                    .Select(duration => duration.Start)
                    .Distinct()
                    .ToList();

            starttimes.Sort();

            if (starttimes.Count != 0) {
                var focusstarttime =
                    starttimes[0];

                var focusitems = new List<RenderedSheetMusicItem>();

                var x = -ScrollX;

                while (x < Width) {
                    if (focusitems.Count == 0) {
                        if (starttimes.Count == 0)
                            break;

                        focusstarttime = starttimes[0];
                        starttimes.RemoveAt(0);
                        focusitems.AddRange(items.Intersecting(focusstarttime).Select(focusitem => focusitem.Value));
                        focusitems.Sort((a, b) => a.Priority.CompareTo(b.Priority));
                    }

                    var item = focusitems[0];
                    focusitems.RemoveAt(0);

                    var width = item.Width(Settings);
                    if (x + width > 0)
                        pe.Graphics.DrawImageUnscaled(item.Draw(Settings), (int)x, 0);

                    x += width;
                }
            }

            base.OnPaint(pe);
        }

        int GetLeft(Time time) {
            var duration =
                new Duration {
                    Start = Time.Zero,
                    Length = time
                };

            var skippedmeasureswidth =
                items
                    .Intersecting(duration)
                    .Sum(measure_item => measure_item.Value.Width(Settings));

            return (int)skippedmeasureswidth;
        }

        public bool Analyze(Duration delta, MusicBrain brain) {
            bool flag = false;

            var layoutmeasures =
                brain.Anlyses<MeasureLayout>(delta);

            foreach (var layoutmeasure in layoutmeasures) {
                if (items_measure.AnyItemIn(layoutmeasure.Duration))
                    continue;

                items_measure.Add(new RenderedMeasure(layoutmeasure.Value), layoutmeasure.Duration);
                flag = true;
            }

            var layoutsimpletimesignatures =
                brain.Anlyses<TimeSignatureSimpleLayout>(delta);

            foreach (var layoutsimpletimesignature in layoutsimpletimesignatures) {
                if (items_timesigsimple.AnyItemIn(layoutsimpletimesignature.Duration))
                    continue;

                items_timesigsimple.Add(new RenderedTimeSignatureSimple(layoutsimpletimesignature.Value), layoutsimpletimesignature.Duration);
                flag = true;
            }

            return flag;
        }

        public void Forget(Duration delta) {
            foreach (var item in items.Intersecting(delta)) {
                item.Value.Dispose();

                var item_clefsymbol = item as IDuratedItem<RenderedClefSymbol>;
                var item_measure = item as IDuratedItem<RenderedMeasure>;
                var item_timesigsimple = item as IDuratedItem<RenderedTimeSignatureSimple>;

                if (item_clefsymbol != null)
                    items_clefsymbols.Remove(item_clefsymbol);
                else if (item_measure != null)
                    items_measure.Remove(item_measure);
                else if (item_timesigsimple != null)
                    items_timesigsimple.Remove(item_timesigsimple);
                else {
                    // this is the disadvantage of not using c++ macros
                    throw new NotSupportedException();
                }
            }
        }
    }
}
