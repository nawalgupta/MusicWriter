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

        public float Margin { get; set; } = 25f;

        public override int Priority {
            get { return 2; }
        }

        public MeasureLayout LayoutMeasure {
            get { return layoutmeasure; }
        }
        
        public RenderedMeasure(MeasureLayout layoutmeasure) {
            this.layoutmeasure = layoutmeasure;
        }

        public override void Select(
                NoteSelection selection,
                RectangleF rectangle,
                SheetMusicRenderSettings settings,
                float width
            ) {
            rectangle.X -= Margin;

            foreach (var chord in layoutmeasure.Chords) {
                var x = chord.X * width;
                var w = chord.Width * width;

                foreach (var note in chord.Notes) {
                    var y = settings.YVal(note.HalfLine);

                    var noteID = note.Core.Note.ID;

                    var rect_head =
                        new RectangleF(
                                x - settings.NoteHeadRadius,
                                y - settings.NoteHeadRadius,
                                2 * settings.NoteHeadRadius,
                                2 * settings.NoteHeadRadius
                            );

                    if (rect_head.IntersectsWith(rectangle)) {
                        selection.Selected_Start.Add(noteID);
                        selection.Selected_End.Add(noteID);
                        selection.Selected_Tone.Add(noteID);
                    }
                    else {
                        var boxy = y;
                        boxy -= settings.PixelsPerHalfLine * note.Transform.Steps;

                        var rect_tail =
                            new RectangleF(
                                    x + w - 2 * settings.ThumbMarginX,
                                    boxy - settings.PixelsPerHalfLine / 2,
                                    2 * settings.ThumbMarginX,
                                    settings.PixelsPerHalfLine
                                );

                        if (rect_tail.IntersectsWith(rectangle))
                            selection.Selected_End.Add(noteID);
                    }
                }
            }
        }

        public override float PixelAtTime(Time offset, float width, SheetMusicRenderSettings settings) =>
            Time.FloatDiv(offset, layoutmeasure.Duration.Length) * (width + 0.75f * Margin) + Margin + 0 * settings.NoteHeadRadius;

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
