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
    }
}
