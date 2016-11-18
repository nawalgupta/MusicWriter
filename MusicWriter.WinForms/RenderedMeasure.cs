using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms {
    public sealed class RenderedMeasure : RenderedSheetMusicItem {
        readonly MeasureLayout layoutmeasure;

        static readonly Pen pen_stem = new Pen(Color.Black, 3.0f);
        static readonly Pen pen_flag = new Pen(Color.Black, 5.0f);
        static readonly Pen pen_tie = new Pen(Color.Black, 2.5f);
        static readonly Pen pen_note = new Pen(Color.Black, 2.5f);
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

        public override float Width(SheetMusicRenderSettings settings) =>
            layoutmeasure.ScaleX * settings.PixelsPerX * settings.PixelsScale + 2 * Margin;

        protected override void Render(Graphics gfx, SheetMusicRenderSettings settings) {
            gfx.TranslateTransform(Margin, 0);

            foreach (var chord in layoutmeasure.Chords)
                DrawChord(gfx, chord, settings);

            DrawMeasureDivision(gfx, settings);
        }

        void DrawMeasureDivision(Graphics gfx, SheetMusicRenderSettings settings) {
            var w = Width(settings);

            gfx.DrawLine(
                    pen_measuredivision,
                    w - 2 * pen_measuredivision.Width - Margin,
                    settings.YVal(settings.Staff.Lines * 2 - 2),
                    w - 2 * pen_measuredivision.Width - Margin,
                    settings.YVal(0)
                );
        }
        
        void DrawChord(Graphics gfx, ChordLayout chord, SheetMusicRenderSettings settings) {
            var x = chord.X * Width(settings);

            foreach (var note in chord.Notes)
                DrawNote(
                        gfx,
                        note,
                        chord.StemDirection,
                        x,
                        settings.YVal(chord.StemStartHalfLines),
                        settings
                    );

            DrawFlags(
                    gfx,
                    x,
                    chord.FlagDirection,
                    chord.FlagLength,
                    settings.YVal(chord.StemStartHalfLines),
                    chord.FlagSlope,
                    chord.Flags,
                    chord.StemDirection,
                    settings
                );
        }

        void DrawFlags(
                Graphics gfx,
                float x,
                FlagDirection direction,
                float width,
                float y_start,
                float slope,
                int flags,
                NoteStemDirection stemdirection,
                SheetMusicRenderSettings settings
            ) {
            var diff = stemdirection == NoteStemDirection.Down ? -1 : 1;
            var dir_scale = direction == FlagDirection.Left ? -1 : 1;

            for (int i = 0; i < flags; i++)
                gfx.DrawLine(
                        pen_flag,
                        x,
                        y_start + diff * i * 0.25F * settings.PixelsPerLine,
                        x + width * dir_scale * Width(settings),
                        y_start + diff * i * 0.25F * settings.PixelsPerLine + slope * width * Width(settings)
                    );
        }

        void DrawNote(
                Graphics gfx,
                NoteLayout note,
                NoteStemDirection direction,
                float x,
                float stem_end,
                SheetMusicRenderSettings settings
            ) {
            var halfline =
                settings.Staff.GetHalfLine(note.Key);

            var y =
                settings.YVal(note.HalfLine);

            var fill = note.Core.Length.Length > LengthClass.Half;
            var dots = note.Core.Length.Dots;

            var y_dots =
                settings.YVal(note.HalfLine + (note.HalfLine + 1) % 2);

            if (direction == NoteStemDirection.Up) {
                DrawNoteHead_L(gfx, x, y, y_dots, fill, dots, settings);
            }
            else {
                DrawNoteHead_R(gfx, x, y, y_dots, fill, dots, settings);
            }

            DrawNoteStem(gfx, x, y, direction, stem_end);
        }

        void DrawNoteHead_L(Graphics gfx, float x, float y, float y_dots, bool fill, int dots, SheetMusicRenderSettings settings) {
            gfx.DrawEllipse(pen_note, x - settings.NoteHeadRadius * 2, y - settings.NoteHeadRadius, 2 * settings.NoteHeadRadius, 2 * settings.NoteHeadRadius);

            if (fill)
                gfx.FillEllipse(Brushes.Black, x, y - settings.NoteHeadRadius, 2 * settings.NoteHeadRadius, 2 * settings.NoteHeadRadius);

            for (int i = 0; i < dots; i++)
                gfx.FillEllipse(Brushes.Black, x + i * settings.DotSpacing + settings.DotInitialSpacing, y_dots - settings.DotRadius, settings.DotRadius * 2F, settings.DotRadius * 2F);
        }

        void DrawNoteHead_R(Graphics gfx, float x, float y, float y_dots, bool fill, int dots, SheetMusicRenderSettings settings) {
            gfx.DrawEllipse(pen_note, x, y - settings.NoteHeadRadius, 2 * settings.NoteHeadRadius, 2 * settings.NoteHeadRadius);

            if (fill)
                gfx.FillEllipse(Brushes.Black, x, y - settings.NoteHeadRadius, 2 * settings.NoteHeadRadius, 2 * settings.NoteHeadRadius);

            for (int i = 0; i < dots; i++)
                gfx.FillEllipse(Brushes.Black, x + settings.NoteHeadRadius * 2 + i * settings.DotSpacing + settings.DotInitialSpacing, y_dots - settings.DotRadius, settings.DotRadius * 2F, settings.DotRadius * 2F);
        }

        void DrawNoteStem(Graphics gfx, float x, float y, NoteStemDirection direction, float y_end) {
            if (direction != NoteStemDirection.None)
                gfx.DrawLine(pen_stem, x, y, x, y_end);
        }

        void DrawTie(Graphics gfx, float x0, float x1, float y) {

        }
    }
}
