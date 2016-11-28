using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms
{
    public static class NoteRenderer
    {
        public static void DrawChord(
                Graphics graphics,
                SheetMusicRenderSettings settings,
                Color color,
                ChordLayout chord,
                int width
            ) {
            var x = chord.X * width;

            foreach (var note in chord.Notes)
                DrawNote(
                        graphics,
                        color,
                        note,
                        chord.StemDirection,
                        chord.StemSide,
                        x,
                        settings.YVal(chord.StemStartHalfLines),
                        settings
                    );

            DrawFlags(
                    graphics,
                    color,
                    x,
                    chord.FlagDirection,
                    chord.FlagLength,
                    settings.YVal(chord.StemStartHalfLines),
                    chord.FlagSlope,
                    chord.TiedFlags,
                    chord.FreeFlags,
                    chord.StemDirection,
                    chord.StemSide,
                    settings,
                    width
                );
        }

        static void DrawFlags(
                Graphics gfx,
                Color color,
                float x,
                FlagDirection direction,
                float length,
                float y_start,
                float slope,
                int flags_tied,
                int flags_free,
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

            if (flags_tied > 0) {
                for (int i = 0; i < flags_tied + flags_free; i++)
                    gfx.DrawLine(
                            new Pen(color, 5.0f),
                            startX,
                            y_start + diff * i * settings.LinesBetweenFlags * settings.PixelsPerLine,
                            x + length * dir_scale * width,
                            y_start + diff * i * settings.LinesBetweenFlags * settings.PixelsPerLine + 0.5f * (slope * settings.PixelsPerLine) * ((startX - (x + length * dir_scale * width)) / width)
                        );
            }
            else if (flags_free > 0) {
                for (int i = 0; i < flags_free; i++)
                    gfx.DrawLine(
                            new Pen(color, 5.0f),
                            startX,
                            y_start + diff * i * settings.LinesBetweenFlags * settings.PixelsPerLine,
                            startX + diff * 15,
                            y_start + diff * i * settings.LinesBetweenFlags * settings.PixelsPerLine + diff * 15
                        );
            }
        }

        static void DrawNote(
                Graphics gfx,
                Color color,
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

            DrawNoteHead(gfx, color, x, y, y_dots, fill, dots, settings);

            if (side == NoteStemSide.Left)
                DrawNoteStem_L(gfx, color, x, y, direction, stem_end, settings);
            else if (side == NoteStemSide.Right)
                DrawNoteStem_R(gfx, color, x, y, direction, stem_end, settings);
        }

        static void DrawNoteHead(Graphics gfx, Color color, float x, float y, float y_dots, bool fill, int dots, SheetMusicRenderSettings settings) {
            gfx.DrawEllipse(new Pen(color, 2.5f), x - settings.NoteHeadRadius, y - settings.NoteHeadRadius, 2 * settings.NoteHeadRadius, 2 * settings.NoteHeadRadius);

            if (fill)
                gfx.FillEllipse(new SolidBrush(color), x - settings.NoteHeadRadius, y - settings.NoteHeadRadius, 2 * settings.NoteHeadRadius, 2 * settings.NoteHeadRadius);

            for (int i = 0; i < dots; i++)
                gfx.FillEllipse(new SolidBrush(color), x + settings.NoteHeadRadius + i * settings.DotSpacing + settings.DotInitialSpacing, y_dots - settings.DotRadius, settings.DotRadius * 2F, settings.DotRadius * 2F);
        }

        static void DrawNoteStem_R(
                Graphics gfx,
                Color color,
                float x,
                float y,
                NoteStemDirection direction,
                float y_end,
                SheetMusicRenderSettings settings
            ) {
            if (direction != NoteStemDirection.None)
                gfx.DrawLine(new Pen(color, 3.0f), x + settings.NoteHeadRadius, y, x + settings.NoteHeadRadius, y_end);
        }

        static void DrawNoteStem_L(
                Graphics gfx,
                Color color,
                float x,
                float y,
                NoteStemDirection direction,
                float y_end,
                SheetMusicRenderSettings settings
            ) {
            if (direction != NoteStemDirection.None)
                gfx.DrawLine(new Pen(color, 3.0f), x - settings.NoteHeadRadius, y, x - settings.NoteHeadRadius, y_end);
        }
    }
}
