using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public struct PerceptualNoteID : IEquatable<PerceptualNoteID> {
        public readonly NoteID NoteID;
        public readonly int Instance;

        public PerceptualNoteID(
                NoteID noteID,
                int instance
            ) {
            NoteID = noteID;
            Instance = instance;
        }

        public override int GetHashCode() =>
            NoteID.GetHashCode() ^ Instance;

        public override bool Equals(object obj) =>
            obj is PerceptualNoteID &&
            Equals((PerceptualNoteID)obj);

        public bool Equals(PerceptualNoteID that) =>
            this == that;

        public static bool operator ==(PerceptualNoteID a, PerceptualNoteID b) =>
            a.NoteID == b.NoteID &&
            a.Instance == b.Instance;

        public static bool operator !=(PerceptualNoteID a, PerceptualNoteID b) =>
            a.NoteID != b.NoteID ||
            a.Instance != b.Instance;
    }
}
