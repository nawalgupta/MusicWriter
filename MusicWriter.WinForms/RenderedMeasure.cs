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

        public override float MinWidth(SheetMusicRenderSettings settings) =>
            layoutmeasure.ScaleX * settings.PixelsPerX * settings.PixelsScale + 2 * Margin;

        protected override void Render(Graphics gfx, SheetMusicRenderSettings settings, int width) {
            gfx.TranslateTransform(Margin, 0);

            foreach (var chord in layoutmeasure.Chords)
                DrawChord(gfx, chord, settings, width);

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
        
        void DrawChord(Graphics gfx, ChordLayout chord, SheetMusicRenderSettings settings, int width) {
            var x = chord.X * width;

            foreach (var note in chord.Notes)
                DrawNote(
                        gfx,
                        note,
                        chord.StemDirection,
                        chord.StemSide,
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
                    chord.StemSide,
                    settings,
                    width
                );
        }

        void DrawFlags(
                Graphics gfx,
                float x,
                FlagDirection direction,
                float length,
                float y_start,
                float slope,
                int flags,
                NoteStemDirection stemdirection,
                NoteStemSide side,
                SheetMusicRenderSettings settings,
                int width
            ) {
            var diff = stemdirection == NoteStemDirection.Down ? -1 : 1;
            var dir_scale = direction == FlagDirection.Left ? -1 : 1;
            
            var startX = x;

            // line / vpx -> px[y] / px[x]
            // multiply by (px/line) / (px/vpx)
            

            if (side == NoteStemSide.Left)
                startX -= settings.NoteHeadRadius;
            else if (side == NoteStemSide.Right)
                startX += settings.NoteHeadRadius;
            
            for (int i = 0; i < flags; i++)
                gfx.DrawLine(
                        pen_flag,
                        startX,
                        y_start + diff * i * settings.PixelsPerLine,
                        x + length * dir_scale * width,
                        y_start + diff * i * settings.PixelsPerLine + (slope * (length - (startX - x) / width)) * -dir_scale * width / settings.PixelsPerLine
                    );
        }

        void DrawNote(
                Graphics gfx,
                NoteLayout note,
                NoteStemDirection direction,
                NoteStemSide side,
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
            
            DrawNoteHead(gfx, x, y, y_dots, fill, dots, settings);
            
            if (side == NoteStemSide.Left)
                DrawNoteStem_L(gfx, x, y, direction, stem_end, settings);
            else if(side == NoteStemSide.Right)
                DrawNoteStem_R(gfx, x, y, direction, stem_end, settings);
        }
        
        void DrawNoteHead(Graphics gfx, float x, float y, float y_dots, bool fill, int dots, SheetMusicRenderSettings settings) {
            gfx.DrawEllipse(pen_note, x - settings.NoteHeadRadius, y - settings.NoteHeadRadius, 2 * settings.NoteHeadRadius, 2 * settings.NoteHeadRadius);

            if (fill)
                gfx.FillEllipse(Brushes.Black, x - settings.NoteHeadRadius, y - settings.NoteHeadRadius, 2 * settings.NoteHeadRadius, 2 * settings.NoteHeadRadius);

            for (int i = 0; i < dots; i++)
                gfx.FillEllipse(Brushes.Black, x + settings.NoteHeadRadius + i * settings.DotSpacing + settings.DotInitialSpacing, y_dots - settings.DotRadius, settings.DotRadius * 2F, settings.DotRadius * 2F);
        }

        void DrawNoteStem_R(
                Graphics gfx, 
                float x, 
                float y, 
                NoteStemDirection direction, 
                float y_end,
                SheetMusicRenderSettings settings
            ) {
            if (direction != NoteStemDirection.None)
                gfx.DrawLine(pen_stem, x + settings.NoteHeadRadius, y, x + settings.NoteHeadRadius, y_end);
        }

        void DrawNoteStem_L(
                Graphics gfx,
                float x,
                float y,
                NoteStemDirection direction,
                float y_end,
                SheetMusicRenderSettings settings
            ) {
            if (direction != NoteStemDirection.None)
                gfx.DrawLine(pen_stem, x - settings.NoteHeadRadius, y, x - settings.NoteHeadRadius, y_end);
        }

        void DrawTie(Graphics gfx, float x0, float x1, float y) {

        }
    }
}
