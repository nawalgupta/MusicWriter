using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class TrackControllerScreenFactory : IFactory<IScreen>
    {
        public string Name {
            get { return TrackControllerScreen.ItemName; }
        }

        private TrackControllerScreenFactory() {
        }

        public void Init(
                StorageObjectID storageobjectID,
                EditorFile file
            ) {
        }

        public IScreen Load(
                StorageObjectID storageobjectID,
                EditorFile file
            ) =>
            new TrackControllerScreen(
                    storageobjectID,
                    file
                );

        public static readonly IFactory<IScreen> Instance =
            new TrackControllerScreenFactory();
    }
}
