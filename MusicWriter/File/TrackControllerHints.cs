using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public class TrackControllerHints
    {
        public virtual Time WordSize(Time here) =>
            Time.Note;

        public virtual Time UnitSize(Time here) =>
            Time.Note_8th;
    }
}
