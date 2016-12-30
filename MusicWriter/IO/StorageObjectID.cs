using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public struct StorageObjectID : IEquatable<StorageObjectID>
    {
        public readonly Guid ID;

        public StorageObjectID(Guid id) {
            ID = id;
        }

        public override string ToString() =>
            ID.ToString();

        public static StorageObjectID Parse(string source) =>
            new StorageObjectID(Guid.Parse(source));

        public bool Equals(StorageObjectID that) =>
            this == that;

        public override bool Equals(object obj) =>
            obj is StorageObjectID &&
            Equals((StorageObjectID)obj);

        public override int GetHashCode() => ID.GetHashCode();

        public static bool operator ==(StorageObjectID a, StorageObjectID b) =>
            a.ID == b.ID;

        public static bool operator !=(StorageObjectID a, StorageObjectID b) =>
            a.ID != b.ID;

        public static readonly StorageObjectID Zero = new StorageObjectID(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
    }
}
