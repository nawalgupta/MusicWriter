using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface ITrackController<TView> {
        ObservableProperty<string> Name { get; }

        StorageObjectID StorageObjectID { get; }

        EditorFile<TView> File { get; }

        CommandCenter CommandCenter { get; }

        ITrackControllerFactory<TView> Factory { get; }
        
        TView View { get; }

        IList<ITrack> Tracks { get; }

        Pin Pin { get; }

        TrackControllerHints Hints { get; }
    }
}
