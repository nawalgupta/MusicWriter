using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    //NOTE: this method assumes that the end handle of the note is selected so it is in the middle of being dragged
    public delegate void NotePlacedDelegate(Duration duration, SemiTone tone);
}
