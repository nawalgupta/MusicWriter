using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class TimeSignature {
        public struct Simple {
            public int Upper, Lower;

            public Time Length {
                get { return Time.Fraction(Upper, Lower); }
            }

            public Simple(int upper = 4, int lower = 4) {
                Upper = upper;
                Lower = lower;
            }
        }

        public Simple[] Simples;

        public TimeSignature(params Simple[] simples) {
            Simples = simples;
        }

        public Simple GetSimple(Time offset, out Time simple_start) {
            var totalsimpleslength =
                Simples.Aggregate(Time.Zero, (acc, simplex) => acc + simplex.Length);

            var mod_offset = offset % totalsimpleslength;
            var skipped_offset = offset - mod_offset;

            for (int i = 0; i < Simples.Length; i++) {
                if ((mod_offset -= Simples[i].Length) < Time.Zero) {
                    simple_start = skipped_offset;
                    return Simples[i];
                }

                skipped_offset += Simples[i].Length;
            }

            simple_start = default(Time);
            return default(Simple);
        }
    }
}
