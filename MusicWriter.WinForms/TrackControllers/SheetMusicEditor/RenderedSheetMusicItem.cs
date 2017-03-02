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

        public virtual float PixelAtTime(Time offset, float width, SheetMusicRenderSettings settings) => float.NaN;

        public virtual bool Stretchy { get { return true; } }

        public abstract int Priority { get; }

        int lastwidth = 0;

        public virtual void Select(
                NoteSelection selection,
                RectangleF rectangle,
                SheetMusicRenderSettings settings,
                float width
            ) {
        }

        public void Draw(
                Graphics graphics,
                SheetMusicRenderSettings settings,
                int width,
                int x
            ) {
            graphics.TranslateTransform(x, 0);
            Render(graphics, settings, width);
            graphics.TranslateTransform(-x, 0);

            //if (lastwidth != width) {
            //    lastwidth = width;

            //    bmp =
            //        new Bitmap(
            //                width,
            //                (int)settings.Height
            //            );

            //    using (var gfx = Graphics.FromImage(bmp)) {
            //        gfx.CompositingQuality = CompositingQuality.HighQuality;
            //        gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //        gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //        gfx.SmoothingMode = SmoothingMode.HighQuality;
            //        gfx.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            //        Render(gfx, settings, lastwidth);
            //    }
            //}

            //return bmp;
        }

        protected abstract void Render(Graphics gfx, SheetMusicRenderSettings settings, int width);

        public abstract float MinWidth(SheetMusicRenderSettings settings);

        public void Dispose() {
            if (bmp != null)
                bmp.Dispose();
        }
    }
}
