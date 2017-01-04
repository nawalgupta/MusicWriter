using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static MusicWriter.TimeSignature;

namespace MusicWriter {
    public sealed class MusicTrackFactory : ITrackFactory
    {
        public static ITrackFactory Instance { get; } =
            new MusicTrackFactory();

        public string Name {
            get { return "Music Track"; }
        }

        private MusicTrackFactory() {
        }

        public void Init(
                IStorageObject storage,
                TrackSettings settings
            ) {
            storage.WriteAllString("This is a music track.");
        }

        public ITrack Load(
                IStorageObject storage,
                TrackSettings settings
            ) =>
            new MusicTrack(
                    storage,
                    settings
                );
    }
}
