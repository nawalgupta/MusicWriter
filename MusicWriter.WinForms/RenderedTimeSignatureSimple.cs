using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms {
    public sealed class RenderedTimeSignatureSimple : RenderedSheetMusicItem {
        readonly TimeSignatureSimpleLayout timesignaturesimple;

        public override bool Stretchy {
            get { return false; }
        }

        public override int Priority {
            get { return 3; }
        }

        public RenderedTimeSignatureSimple(TimeSignatureSimpleLayout timesignaturesimple) {
            this.timesignaturesimple = timesignaturesimple;
        }
        
        protected override void Render(Graphics gfx, SheetMusicRenderSettings settings, int width) {
            var upper_str = timesignaturesimple.Simple.Upper.ToString();
            var lower_str = timesignaturesimple.Simple.Lower.ToString();

            var upper_sz = gfx.MeasureString(upper_str, settings.TimeSignatureFont);
            var lower_sz = gfx.MeasureString(lower_str, settings.TimeSignatureFont);

            gfx.DrawString(upper_str, settings.TimeSignatureFont, Brushes.Black, 0, settings.YVal(4) - upper_sz.Height);
            gfx.DrawString(lower_str, settings.TimeSignatureFont, Brushes.Black, 0, settings.YVal(0) - lower_sz.Height);
        }

        public override float MinWidth(SheetMusicRenderSettings settings) =>
            (float)Math.Max(
                    Math.Ceiling(Math.Log10(timesignaturesimple.Simple.Upper)),
                    Math.Ceiling(Math.Log10(timesignaturesimple.Simple.Lower))
                ) * settings.PixelsScale * 10F;
    }
}
