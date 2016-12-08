using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface ITrackFactory {
        string Name { get; }

        ITrack Create();

        ITrack Load(Stream stream);

        void Save(ITrack track, Stream stream);
    }
}
