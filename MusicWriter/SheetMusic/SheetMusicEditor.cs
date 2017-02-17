using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class SheetMusicEditor : TrackController
    {
        public const string ItemName = "musicwriter.track-controller.controller.music.sheet";

        readonly TrackControllerHints hints;

        public int ActiveTrackIndex { get; set; } = 0;

        public MusicTrack ActiveTrack {
            get { return Tracks[ActiveTrackIndex] as MusicTrack; }
        }

        public Cursor Cursor { get; } =
            new Cursor();

        public override TrackControllerHints Hints {
            get { return hints; }
        }

        public SheetMusicEditor(
                StorageObjectID storageobjectID, 
                EditorFile file,
                IFactory<ITrackController> factory
            ) :
            base(
                    storageobjectID, 
                    file,
                    factory
                ) {
            hints = new OverrideTrackControllerHints(this);
        }

        public static IFactory<ITrackController> CreateFactory() =>
            new CtorFactory<ITrackController, SheetMusicEditor>(ItemName);

        private class OverrideTrackControllerHints : TrackControllerHints
        {
            public SheetMusicEditor Editor { get; set; }

            public OverrideTrackControllerHints(SheetMusicEditor editor = null) {
                Editor = editor;
            }

            public override Time UnitSize(Time here) =>
                Editor
                    .ActiveTrack
                    .Rhythm
                    .Intersecting(here)
                    .First()
                    .Duration
                    .Length;

            public override Time WordSize(Time here) =>
                Editor
                    .ActiveTrack
                    .Rhythm
                    .TimeSignatures
                    .Intersecting_children(here)
                    .First()
                    .Duration
                    .Length;
        }
    }
}
