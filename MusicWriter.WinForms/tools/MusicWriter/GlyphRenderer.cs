using MusicWriter.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms
{
    public static class GlyphRenderer
    {
        static readonly Dictionary<string, Image> glyphs =
            new Dictionary<string, Image>();

        static GlyphRenderer() {
            var glyphspath =
                Path.Combine(
                        Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                        "glyphs"
                    );

            foreach (var file in Directory.EnumerateFiles(glyphspath, "*.png")) {
                var image =
                    Image.FromFile(file);

                glyphs.Add(Path.GetFileNameWithoutExtension(file), image);
            }
        }

        public static void Draw(
                string name,
                float x,
                float y,
                Color color,
                Graphics graphics,
                SheetMusicRenderSettings settings
            ) {
            var matrix =
                new ColorMatrix();
            matrix.Matrix00 = color.R / 255f;
            matrix.Matrix11 = color.G / 255f;
            matrix.Matrix22 = color.B / 255f;
            matrix.Matrix33 = color.A / 255f;

            var attributes = new ImageAttributes();
            attributes.SetColorMatrix(matrix);

            var glyph =
                glyphs[name];

            // 2 lines should be the height of the glyph
            var scale =
                4 * settings.PixelsPerLine / glyph.Height;

            graphics.DrawImage(
                    glyph,
                    new Rectangle(
                            (int)(x - scale * glyph.Width / 2),
                            (int)(y - scale * glyph.Height / 2),
                            (int)(scale * glyph.Width),
                            (int)(scale * glyph.Height)
                        ),
                    0,
                    0,
                    glyph.Width,
                    glyph.Height,
                    GraphicsUnit.Pixel,
                    attributes
                );
        }
    }
}
