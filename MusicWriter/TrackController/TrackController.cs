using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public abstract class TrackController : NamedBoundObject<ITrackController>, ITrackController
    {
        readonly BoundList<ITrack> tracks;
        readonly Pin pin;

        public CommandCenter CommandCenter { get; } =
            new CommandCenter();

        public abstract TrackControllerHints Hints { get; }

        public Pin Pin {
            get { return pin; }
        }

        public BoundList<ITrack> Tracks {
            get { return tracks; }
        }

        public TrackController(
                StorageObjectID storageobjectID,
                EditorFile file,
                IFactory<ITrackController> factory = null
            ) :
            base(
                    storageobjectID,
                    file,
                    factory
                ) {
            var obj =
                file.Storage[storageobjectID];

            var container =
                file[TrackControllerContainer.ItemName] as TrackControllerContainer;

            pin =
                new Pin(
                        obj.GetOrMake("pin").ID,
                        file,
                        container.Settings.TimeMarkerUnit
                    );

            tracks =
                new BoundList<ITrack>(
                        obj.GetOrMake("tracks").ID,
                        file,
                        container.Tracks
                    );
        }

        public override void Bind() {
            pin.Bind();
            tracks.Bind();

            base.Bind();
        }

        public override void Unbind() {
            pin.Unbind();
            tracks.Unbind();

            base.Unbind();
        }
    }
}
