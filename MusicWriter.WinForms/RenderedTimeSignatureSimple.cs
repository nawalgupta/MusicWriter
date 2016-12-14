using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicWriter.TimeSignature;

namespace MusicWriter.WinForms {
    public sealed class RenderedTimeSignatureSimple : RenderedSheetMusicItem {
        readonly Simple simple;

        public override bool Stretchy {
            get { return false; }
        }

        public override int Priority {
            get { return 3; }
        }

        public RenderedTimeSignatureSimple(Simple simple) {
            this.simple = simple;
        }
        
        protected override void Render(Graphics gfx, SheetMusicRenderSettings settings, int width) {
            var upper_str = simple.Upper.ToString();
            var lower_str = simple.Lower.ToString();

            var upper_sz = gfx.MeasureString(upper_str, settings.TimeSignatureFont);
            var lower_sz = gfx.MeasureString(lower_str, settings.TimeSignatureFont);

            gfx.DrawString(upper_str, settings.TimeSignatureFont, Brushes.Black, 0, settings.YVal(4) - upper_sz.Height);
            gfx.DrawString(lower_str, settings.TimeSignatureFont, Brushes.Black, 0, settings.YVal(0) - lower_sz.Height);
        }

        public override float MinWidth(SheetMusicRenderSettings settings) =>
            (float)Math.Max(
                    Math.Ceiling(Math.Log10(simple.Upper)),
                    Math.Ceiling(Math.Log10(simple.Lower))
                ) * settings.PixelsScale * 10F;
    }
}
