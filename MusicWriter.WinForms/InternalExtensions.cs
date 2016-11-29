using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms
{
    public static class InternalExtensions
    {
        public static Color Alpha(this Color @base, double alpha) =>
            Color.FromArgb((int)(255 * alpha), @base);

        public static Color Lerp(this Color a, Color b, float fac) =>
            Color.FromArgb(
                    (int)(a.A + (b.A - a.A) * fac),
                    (int)(a.R + (b.R - a.R) * fac),
                    (int)(a.G + (b.G - a.G) * fac),
                    (int)(a.B + (b.B - a.B) * fac)
                );
    }
}
