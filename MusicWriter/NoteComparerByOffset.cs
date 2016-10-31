using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class NoteComparerByOffset : IComparer<Note> {
        public int Compare(Note x, Note y) =>
            x.Offset.CompareTo(y.Offset);
    }
}
