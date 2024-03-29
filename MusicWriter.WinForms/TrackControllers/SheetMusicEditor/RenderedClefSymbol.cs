﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms {
    public sealed class RenderedClefSymbol : RenderedSheetMusicItem {
        public override bool Stretchy {
            get { return false; }
        }

        public override int Priority {
            get { return 0; }
        }

        public override float MinWidth(SheetMusicRenderSettings settings) =>
            settings.PixelsScale * settings.PixelsPerLine * 2;

        protected override void Render(Graphics gfx, SheetMusicRenderSettings settings, int width) {
            //TODO: render
        }
    }
}
