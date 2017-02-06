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
    public sealed class MusicTrackFactory : IFactory<ITrack>
    {
        public static IFactory<ITrack> Instance { get; } =
            new MusicTrackFactory();

        public string Name {
            get { return "Music Track"; }
        }

        private MusicTrackFactory() {
        }

        public void Init(
                StorageObjectID storageobjectID,
                EditorFile file
            ) {
        }

        public ITrack Load(
                StorageObjectID storageobjectID,
                EditorFile file
            ) =>
            new MusicTrack(
                    file,
                    file.Storage[storageobjectID],
                    (file[TrackControllerContainer.ItemName] as TrackControllerContainer).Settings
                );
    }
}
