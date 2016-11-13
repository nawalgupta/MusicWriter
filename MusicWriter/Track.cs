using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Track {
        readonly MelodyTrack melody;
        readonly RhythmTrack rhythm;
        readonly AdornmentTrack adornment;

        public MelodyTrack Melody {
            get { return melody; }
        }

        public RhythmTrack Rhythm {
            get { return rhythm; }
        }

        public AdornmentTrack Adornment {
            get { return adornment; }
        }

        public Track(
                MelodyTrack melody,
                RhythmTrack rhythm,
                AdornmentTrack adornment
            ) {
            this.melody = melody;
            this.rhythm = rhythm;
            this.adornment = adornment;
        }
    }
}
