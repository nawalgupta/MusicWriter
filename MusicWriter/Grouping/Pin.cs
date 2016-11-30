using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Pin {
        public NamedTime Time { get; } = new NamedTime();
        public PinMode PinMode { get; set; } = PinMode.Floating;
        public ObservableProperty<Time> ActualTime { get; } = new ObservableProperty<Time>(MusicWriter.Time.Zero);
    }
}
