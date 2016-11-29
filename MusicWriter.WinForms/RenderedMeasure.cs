using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms {
    public sealed class RenderedMeasure : RenderedSheetMusicItem {
        readonly MeasureLayout layoutmeasure;
        
        static readonly Pen pen_tie = new Pen(Color.Black, 2.5f);
        static readonly Pen pen_measuredivision = new Pen(Color.Black, 1.5f);

        public float Margin { get; set; } = 15f;

        public override int Priority {
            get { return 2; }
        }

        public MeasureLayout LayoutMeasure {
            get { return layoutmeasure; }
        }
        
        public RenderedMeasure(MeasureLayout layoutmeasure) {
            this.layoutmeasure = layoutmeasure;
        }

        public override float PixelAtTime(Time offset, float width, SheetMusicRenderSettings settings) =>
            Time.FloatDiv(offset, layoutmeasure.Duration.Length) * (width + 1 * Margin) + Margin + 0 * settings.NoteHeadRadius;

        public override float MinWidth(SheetMusicRenderSettings settings) =>
            layoutmeasure.ScaleX * settings.PixelsPerX * settings.PixelsScale + 2 * Margin;

        protected override void Render(Graphics gfx, SheetMusicRenderSettings settings, int width) {
            gfx.TranslateTransform(Margin, 0);

            foreach (var chord in layoutmeasure.Chords)
                NoteRenderer.DrawChord(gfx, settings, Color.Black, Color.Blue, chord, true, width);

            DrawMeasureDivision(gfx, settings, width);
        }

        void DrawMeasureDivision(Graphics gfx, SheetMusicRenderSettings settings, int width) {
            gfx.DrawLine(
                    pen_measuredivision,
                    width - 2 * pen_measuredivision.Width - Margin,
                    settings.YVal(settings.Staff.Lines * 2 - 2),
                    width - 2 * pen_measuredivision.Width - Margin,
                    settings.YVal(0)
                );
        }

        void DrawTie(Graphics gfx, float x0, float x1, float y) {

        }
    }
}
