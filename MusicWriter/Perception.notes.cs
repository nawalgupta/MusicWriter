using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    partial class Perception {
        IEnumerable<PerceptualNote> Decompose_Note(Note note) {
            var cell =
                track.Rhythm.CellsIn(note.Duration);


        }
    }
}
