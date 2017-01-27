using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class TrackControllerContainer : Container
    {
        public const string ItemName = "musicwriter.containers.track-controller";

        readonly TrackControllerSettings settings;

        public TrackControllerSettings Settings {
            get { return settings; }
        }

        public static readonly IFactory<IContainer> FactoryInstance =
            new CtorFactory<IContainer, TrackControllerContainer>(ItemName);

        public override IFactory<IContainer> Factory {
            get { return FactoryInstance; }
        }

        public TrackControllerContainer(
                StorageObjectID storageobjectID, 
                EditorFile file
            ) :
            base(
                    storageobjectID, 
                    file, 
                    ItemName
                ) {
            var obj = file.Storage[storageobjectID];

            settings =
                new TrackControllerSettings(
                        obj.GetOrMake("settings")
                    );
        }
    }
}
