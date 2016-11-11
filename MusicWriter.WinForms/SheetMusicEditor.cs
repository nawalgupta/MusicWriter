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

            Invalidate();
        }

        public Track Track { get; set; }

        public IDurationField<RenderedSheetMusicItem> Knowledge {
            get { return items; }
        }

        readonly DurationField<RenderedSheetMusicItem> items =
            new DurationField<RenderedSheetMusicItem>();

        public SheetMusicEditor() {
            InitializeComponent();
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
                    .AllDurations
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

        public void Analyze(Duration delta, MusicBrain brain) {
            var layoutmeasures =
                brain.Anlyses<MeasureLayout>(delta);

            foreach (var layoutmeasure in layoutmeasures)
                items.Add(new RenderedMeasure(layoutmeasure.Value), layoutmeasure.Duration);

            var layoutsimpletimesignatures =
                brain.Anlyses<TimeSignatureSimpleLayout>(delta);

            foreach (var layoutsimpletimesignature in layoutsimpletimesignatures)
                items.Add(new RenderedTimeSignatureSimple(layoutsimpletimesignature.Value), layoutsimpletimesignature.Duration);

        }

        public void Forget(Duration delta) {
            foreach (var item in items.Intersecting(delta)) {
                item.Value.Dispose();
                items.Remove(item);
            }
        }
    }
}
