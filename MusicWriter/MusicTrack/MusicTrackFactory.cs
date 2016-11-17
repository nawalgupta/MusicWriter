using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class MusicTrackFactory : ITrackFactory {
        public string Name {
            get { return "Music Track"; }
        }

        public ITrack Create() =>
            new MusicTrack(
                    new MelodyTrack(),
                    new RhythmTrack(),
                    new AdornmentTrack(),
                    new PerceptualMemory(),
                    new ExplicitPropertyGraphlet<NoteID>()
                );
    }
}
