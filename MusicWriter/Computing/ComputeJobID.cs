using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public struct ComputeJobID : IEquatable<ComputeJobID>
    {
        public readonly ulong ID;

        public ComputeJobID(ulong id) {
            ID = id;
        }

        static readonly Random rnd = new Random(Environment.TickCount);

        public static ComputeJobID NewComputeJobID() {
            byte[] bytes = new byte[8];
            ulong val =
                ((ulong)bytes[0] << (8 * 0)) |
                ((ulong)bytes[1] << (8 * 1)) |
                ((ulong)bytes[2] << (8 * 2)) |
                ((ulong)bytes[3] << (8 * 3)) |
                ((ulong)bytes[4] << (8 * 4)) |
                ((ulong)bytes[5] << (8 * 5)) |
                ((ulong)bytes[6] << (8 * 6)) |
                ((ulong)bytes[7] << (8 * 7));

            return new ComputeJobID(val);
        }

        public override string ToString() => ID.ToString();

        public override int GetHashCode() => unchecked((int)ID ^ (int)(ID >> 32));

        public override bool Equals(object obj) =>
            obj is ComputeJobID &&
            Equals((ComputeJobID)obj);

        public bool Equals(ComputeJobID that) => this == that;

        public static bool operator ==(ComputeJobID a, ComputeJobID b) =>
            a.ID == b.ID;

        public static bool operator !=(ComputeJobID a, ComputeJobID b) =>
            a.ID != b.ID;
    }
}
