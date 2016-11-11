using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms {
    public sealed class RenderedMeasure : RenderedSheetMusicItem {
        readonly MeasureLayout layoutmeasure;

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
            layoutmeasure.ScaleX * settings.PixelsPerX * settings.PixelsScale;

        protected override void Render(Graphics gfx, SheetMusicRenderSettings settings) {
            foreach (var chord in layoutmeasure.Chords)
                DrawChord(gfx, chord, settings);
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
                    chord.StemStartHalfLines,
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

            for (int i = 0; i < flags; i++)
                gfx.DrawLine(Pens.Black,
                        x,
                        y_start + diff * i * 0.3F * settings.PixelsPerLine,
                        x + width,
                        y_start + diff * i * 0.3F * settings.PixelsPerLine + slope * width
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
                -settings.PixelsPerHalfLine * (halfline + (halfline % 2)) +
                settings.PixelsPerHalfLine * (settings.Staff.Lines * 2 + settings.MarginalTopHalfLines);

            if (direction == NoteStemDirection.Up) {
                DrawNoteHead_L(gfx, x, y, y_dots, fill, dots, settings);
            }
            else {
                DrawNoteHead_R(gfx, x, y, y_dots, fill, dots, settings);
            }

            DrawNoteStem(gfx, x, y, direction, stem_end);
        }

        void DrawNoteHead_L(Graphics gfx, float x, float y, float y_dots, bool fill, int dots, SheetMusicRenderSettings settings) {
            gfx.DrawEllipse(Pens.Black, x - settings.NoteHeadRadius * 2, y - settings.NoteHeadRadius, 2 * settings.NoteHeadRadius, 2 * settings.NoteHeadRadius);

            if (fill)
                gfx.FillEllipse(Brushes.Black, x - settings.NoteHeadRadius * 2, y - settings.NoteHeadRadius, settings.NoteHeadRadius * 2, settings.NoteHeadRadius * 2);

            for (int i = 0; i < dots; i++)
                gfx.FillEllipse(Brushes.Black, x + i * 3.5F + 1F, y_dots - 1.2F, 2.4F, 2.4F);
        }

        void DrawNoteHead_R(Graphics gfx, float x, float y, float y_dots, bool fill, int dots, SheetMusicRenderSettings settings) {
            gfx.DrawEllipse(Pens.Black, x, y - settings.NoteHeadRadius, 2 * settings.NoteHeadRadius, 2 * settings.NoteHeadRadius);

            if (fill)
                gfx.FillEllipse(Brushes.Black, x, y - settings.NoteHeadRadius, settings.NoteHeadRadius * 2, settings.NoteHeadRadius * 2);

            for (int i = 0; i < dots; i++)
                gfx.FillEllipse(Brushes.Black, x + settings.NoteHeadRadius * 2 + i * 3.5F + 2F, y_dots - 1.2F, 2.4F, 2.4F);
        }

        void DrawNoteStem(Graphics gfx, float x, float y, NoteStemDirection direction, float y_end) {
            if (direction != NoteStemDirection.None)
                gfx.DrawLine(Pens.Black, x, y, x, y_end);
        }

        void DrawTie(Graphics gfx, float x0, float x1, float y) {

        }
    }
}
