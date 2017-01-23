using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface ITrackFactory {
        string Name { get; }

        void Init(
                IStorageObject storage,
                GlobalSettings settings
            );

        ITrack Load(
                IStorageObject storage,
                GlobalSettings settings
            );
    }
}
