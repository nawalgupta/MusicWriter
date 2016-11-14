using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface IDuratedItem<out T> {
        Duration Duration { get; }

        T Value { get; }
    }
}
