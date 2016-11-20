using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface ITrackController<TView> {
        ObservableProperty<string> Name { get; }

        EditorFile File { get; }

        InputController InputController { get; set; }

        TView View { get; }

        IList<ITrack> Tracks { get; }

        Pin Pin { get; }
    }
}
