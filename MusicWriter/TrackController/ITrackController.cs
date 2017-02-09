using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface ITrackController : 
        IBoundObject<ITrackController>,
        INamedObject
    {
        CommandCenter CommandCenter { get; }
        
        BoundList<ITrack> Tracks { get; }

        Pin Pin { get; }

        TrackControllerHints Hints { get; }
    }
}
