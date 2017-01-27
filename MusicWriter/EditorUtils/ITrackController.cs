using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface ITrackController {
        ObservableProperty<string> Name { get; }

        StorageObjectID StorageObjectID { get; }

        EditorFile File { get; }

        CommandCenter CommandCenter { get; }

        ITrackControllerFactory Factory { get; }
        
        IObservableList<ITrack> Tracks { get; }

        Pin Pin { get; }

        TrackControllerHints Hints { get; }
    }
}
