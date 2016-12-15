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
                Color fieldcolor,
                ChordLayout chord,
                bool drawfield,
                int width
            ) {
            var x = chord.X * width;

            var notewidth = chord.Width * width;

            foreach (var note in chord.Notes)
                DrawNote(
                        graphics,
                        color,
                        fieldcolor,
                        note,
                        chord.StemDirection,
                        chord.StemSide,
                        x,
                        notewidth,
                        settings.YVal(chord.StemStartHalfLines),
                        drawfield,
                        settings
                    );

            DrawFlags(
                    graphics,
                    color,
                    x,
                    chord.FlagDirection,
                    chord,
                    chord.FlagLength,
                    settings.YVal(chord.StemStartHalfLines),
                    chord.FlagSlope,
                    chord.TiedFlags,
                    chord.FreeFlags,
                    chord.StemDirection,
                    chord.StemSide,
                    settings,
                    width,
                    chord.Past2nd
                );
        }

        static void DrawFlags(
                Graphics gfx,
                Color color,
                float x,
                FlagDirection direction,
                ChordLayout chord,
                float length,
                float y_start,
                float slope,
                int flags_tied,
                int flags_free,
                NoteStemDirection stemdirection,
                NoteStemSide side,
                SheetMusicRenderSettings settings,
                int width,
                bool past2nd
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

            if(past2nd) {
                if (side == NoteStemSide.Left)
                    length += settings.NoteHeadRadius / width;
                else if (side == NoteStemSide.Right)
                    length -= settings.NoteHeadRadius / width;
            }

            if (flags_tied > 0) {
                for (int i = 0; i < flags_tied + flags_free; i++) {
                    gfx.DrawLine(
                            new Pen(color, 5.0f),
                            startX,
                            y_start + diff * i * settings.LinesBetweenFlags * settings.PixelsPerLine,
                            x + length * dir_scale * width,
                            y_start + diff * i * settings.LinesBetweenFlags * settings.PixelsPerLine + 0.5f * (slope * settings.PixelsPerLine) * ((startX - (x + length * dir_scale * width)) / width)
                        );

                    if (i == flags_free && past2nd) {
                        if (chord.LastLengthClass < chord.Length.Length) {
                            length /= 2f;
                        }
                    }
                }
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
                Color fieldcolor,
                NoteLayout note,
                NoteStemDirection direction,
                NoteStemSide side,
                float x,
                float notewidth,
                float stem_end,
                bool drawfield,
                SheetMusicRenderSettings settings
            ) {
            var halfline =
                settings.Staff.GetHalfLine(note.Key);

            for (int i = halfline / 2; i < 0; i++)
                DrawLedger(gfx, x, 2 * i, settings);

            for (int i = (halfline / 2 - settings.Staff.Lines + 1); i > 0; i--)
                DrawLedger(gfx, x, 2 * (settings.Staff.Lines + i - 1), settings);

            var y =
                settings.YVal(note.HalfLine);

            var fill = note.Core.Length.Length > LengthClass.Half;
            var dots = note.Core.Length.Dots;

            var y_dots =
                settings.YVal(note.HalfLine + (note.HalfLine + 1) % 2);

            var selected_start =
                settings.Selection.Selected_Start.Contains(note.Core.Note.ID);
            var selected_end =
                settings.Selection.Selected_End.Contains(note.Core.Note.ID);
            var selected_tone =
                settings.Selection.Selected_Tone.Contains(note.Core.Note.ID);

            var fieldcolorinterpolation =
                (selected_end ? 1 : 0) +
                (selected_start ? 1 : 0) +
                (selected_tone ? 1 : 0);

            if (note.Core.ID.Instance == 0) {
                if (drawfield) {
                    var fieldbcolor = color.Lerp(fieldcolor, fieldcolorinterpolation / 3f).Alpha(0.5);
                    var fieldfcolor = fieldcolorinterpolation == 3 ? fieldcolor : color;
                    var thumbpen = new Pen(fieldfcolor, settings.ThumbWidth);

                    var fieldy = y;

                    gfx.FillRectangle(
                            new SolidBrush(fieldbcolor),
                            x,
                            fieldy - settings.PixelsPerHalfLine / 2,
                            notewidth,
                            settings.PixelsPerHalfLine
                        );

                    gfx.DrawLine(
                            thumbpen,
                            x + settings.ThumbPadding,
                            fieldy,
                            x + notewidth - 2 * settings.ThumbPadding,
                            fieldy
                        );

                    gfx.DrawLine(
                            thumbpen,
                            x + notewidth - settings.ThumbMarginX,
                            fieldy - settings.PixelsPerHalfLine / 2 + settings.ThumbMarginY,
                            x + notewidth - settings.ThumbMarginX,
                            fieldy + settings.PixelsPerHalfLine / 2 - settings.ThumbMarginY
                        );
                }
            }

            DrawNoteHead(gfx, fieldcolorinterpolation == 3 ? fieldcolor : color, x, y, y_dots, fill, dots, settings);
            
            if (side == NoteStemSide.Left)
                DrawNoteStem_L(gfx, color, x, y, direction, stem_end, settings);
            else if (side == NoteStemSide.Right)
                DrawNoteStem_R(gfx, color, x, y, direction, stem_end, settings);
        }

        static void DrawLedger(Graphics gfx, float x, float halfline, SheetMusicRenderSettings settings) =>
            gfx.DrawLine(
                    settings.StaffLinePen,
                    x - settings.LedgerPixelWidth / 2,
                    settings.YVal(halfline),
                    x + settings.LedgerPixelWidth / 2,
                    settings.YVal(halfline)
                );

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
