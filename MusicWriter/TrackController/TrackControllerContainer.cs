using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class TrackControllerContainer : Container
    {
        public const string ItemName = "musicwriter.track-controller.container";
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

        public TrackControllerContainer(
                StorageObjectID storageobjectID, 
                EditorFile file,
                IFactory<IContainer> factory,

                FactorySet<ITrack> tracks_factoryset,
                ViewerSet<ITrack> tracks_viewerset,

                FactorySet<ITrackController> controllers_factoryset,
                ViewerSet<ITrackController> controllers_viewerset
            ) :
            base(
                    storageobjectID, 
                    file,
                    factory,
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
                        file,
                        tracks_factoryset,
                        tracks_viewerset
                    );

            controllers =
                new BoundList<ITrackController>(
                        obj.GetOrMake("controllers").ID,
                        file,
                        controllers_factoryset,
                        controllers_viewerset
                    );
        }

        public static IFactory<IContainer> CreateFactory(
                FactorySet<ITrack> tracks_factoryset,
                ViewerSet<ITrack> tracks_viewerset,

                FactorySet<ITrackController> controllers_factoryset,
                ViewerSet<ITrackController> controllers_viewerset
            ) =>
            new CtorFactory<IContainer, TrackControllerContainer>(
                    ItemName,
                    tracks_factoryset,
                    tracks_viewerset,
                    controllers_factoryset,
                    controllers_viewerset
                );
    }
}
