using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public struct NoteID : IEquatable<NoteID> {
        public readonly int ID;

        public NoteID(int id) {
            ID = id;
        }

        public override string ToString() =>
            ID.ToString();

        public bool Equals(NoteID that) =>
            this == that;

        public override bool Equals(object obj) =>
            obj is NoteID &&
            Equals((NoteID)obj);

        public override int GetHashCode() => ID;

        public static bool operator ==(NoteID a, NoteID b) =>
            a.ID == b.ID;

        public static bool operator !=(NoteID a, NoteID b) =>
            a.ID != b.ID;
    }
}
