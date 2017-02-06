using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface ITrackController : IBoundObject<ITrackController> {
        CommandCenter CommandCenter { get; }
        
        IObservableList<ITrack> Tracks { get; }

        Pin Pin { get; }

        TrackControllerHints Hints { get; }
    }
}
