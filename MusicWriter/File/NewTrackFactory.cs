using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class NewTrackFactory<T> :
        ITrackFactory
        where T : ITrack, new() {
        public string Name { get; set; }

        public ITrack Create() =>
            new T();
    }
}
