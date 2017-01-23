using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class TrackControllerScreenFactory<View> : IScreenFactory<View>
    {
        public string Name {
            get { return "Track Controller Screen"; }
        }

        private TrackControllerScreenFactory() {
        }

        public void Init(
                StorageObjectID storageobjectID,
                EditorFile<View> file
            ) {
        }

        public IScreen<View> Load(
                StorageObjectID storageobjectID,
                EditorFile<View> file
            ) =>
            new TrackControllerScreen<View>(
                    storageobjectID,
                    file
                );

        public static readonly IScreenFactory<View> Instance =
            new TrackControllerScreenFactory<View>();
    }
}
