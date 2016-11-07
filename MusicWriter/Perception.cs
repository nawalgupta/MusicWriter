using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed partial class Perception {
        readonly Track track;

        public Track Track {
            get { return track; }
        }

        public Perception(Track track) {
            this.track = track;
        }
    }
}
