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

        public virtual bool Stretchy { get { return true; } }

        public abstract int Priority { get; }

        int lastwidth = 0;

        public Bitmap Draw(SheetMusicRenderSettings settings, int width) {
            if (lastwidth != width) {
                bmp =
                    new Bitmap(
                            width,
                            (int)settings.Height
                        );

                using (var gfx = Graphics.FromImage(bmp)) {
                    gfx.CompositingQuality = CompositingQuality.HighQuality;
                    gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gfx.SmoothingMode = SmoothingMode.HighQuality;
                    gfx.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    Render(gfx, settings, width);
                }

                lastwidth = width;
            }

            return bmp;
        }

        protected abstract void Render(Graphics gfx, SheetMusicRenderSettings settings, int width);

        public abstract float MinWidth(SheetMusicRenderSettings settings);

        public void Dispose() {
            if (bmp != null)
                bmp.Dispose();
        }
    }
}
