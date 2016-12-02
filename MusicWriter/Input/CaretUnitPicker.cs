using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public delegate void CaretUnitPickerDelegate(CaretUnitPickerEventArgs args);

    public sealed class CaretUnitPickerEventArgs
    {
        public Time Length { get; set; }
        public bool Handled { get; set; } = false;
    }
}
