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
        public const string ItemCodeName = "track-controller";

        readonly TrackControllerSettings settings;
        readonly BoundList<ITrack> tracks;
        readonly BoundList<ITrackController> controllers;

        public TrackControllerSettings Settings {
            get { return settings; }
        }

        public BoundList<ITrack> Tracks {
            get { return tracks; }
        }

        public BoundList<ITrackController> Controllers {
            get { return controllers; }
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
                    ItemName,
                    ItemCodeName
                ) {
            var obj = file.Storage[storageobjectID];

            settings =
                new TrackControllerSettings(
                        obj.GetOrMake("settings")
                    );

            tracks =
                new BoundList<ITrack>(
                        obj.GetOrMake("tracks").ID,
                        file
                    );

            controllers =
                new BoundList<ITrackController>(
                        obj.GetOrMake("controllers").ID,
                        file
                    );
        }
    }
}
