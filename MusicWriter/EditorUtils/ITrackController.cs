using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface ITrackController<TView> {
        ObservableProperty<string> Name { get; }

        EditorFile File { get; }

        CommandCenter CommandCenter { get; }
        
        TView View { get; }

        IList<ITrack> Tracks { get; }

        Pin Pin { get; }
    }
}
