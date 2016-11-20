using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class NamedTime {
        public ObservableProperty<string> GroupName { get; } = new ObservableProperty<string>();
        public ObservableProperty<Time> Offset { get; } = new ObservableProperty<Time>(Time.Zero);
    }
}
