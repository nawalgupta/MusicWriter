using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms {
    public sealed class SheetMusicRenderSettings {
        public float PixelsPerX { get; set; } = 100;
        public float PixelsScale { get; set; } = 1.0F;
        public int MarginalBottomHalfLines { get; set; } = 3;
        public int MarginalTopHalfLines { get; set; } = 3;
        public float PixelsPerHalfLine { get; set; } = 10F;
        public Staff Staff { get; set; } = Staff.Treble;
        public float NoteHeadRadius { get; set; } = 4.5F;
        public Font TimeSignatureFont { get; set; } = new Font(FontFamily.GenericSerif, 18);

        public float PixelsPerLine {
            get { return PixelsPerHalfLine * 2; }
            set { PixelsPerHalfLine = value / 2; }
        }

        public float Height {
            get { return PixelsPerLine * (2 * MarginalBottomHalfLines + 2 * MarginalTopHalfLines + Staff.Lines) * PixelsScale; }
        }

        public float YVal(float halflines) =>
            -PixelsPerHalfLine * halflines +
            PixelsPerHalfLine * (Staff.Lines * 2 + MarginalTopHalfLines);
    }
}
