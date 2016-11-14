using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class DuratedItem<T> : IDuratedItem<T> {
        public Duration Duration { get; set; }
        public T Value { get; set; }
    }
}
