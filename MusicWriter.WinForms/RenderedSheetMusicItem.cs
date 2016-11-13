using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms {
    public abstract class RenderedSheetMusicItem : IDisposable {
        Bitmap bmp = null;

        public abstract int Priority { get; }

        public Bitmap Draw(SheetMusicRenderSettings settings) {
            if (bmp == null) {
                bmp =
                new Bitmap(
                        (int)Width(settings),
                        (int)settings.Height
                    );

                using (var gfx = Graphics.FromImage(bmp)) {
                    gfx.CompositingQuality = CompositingQuality.HighQuality;
                    gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gfx.SmoothingMode = SmoothingMode.HighQuality;
                    gfx.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    Render(gfx, settings);
                }
            }

            return bmp;
        }

        protected abstract void Render(Graphics gfx, SheetMusicRenderSettings settings);

        public abstract float Width(SheetMusicRenderSettings settings);

        public void Dispose() {
            if (bmp != null)
                bmp.Dispose();
        }
    }
}
