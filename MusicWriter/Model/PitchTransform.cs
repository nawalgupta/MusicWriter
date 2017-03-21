using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class PitchTransform {
        readonly int steps;

        public int Steps {
            get { return steps; }
        }

        public PitchTransform(int steps = 0) {
            this.steps = steps;
        }

        public SemiTone Transform(SemiTone natural) =>
            natural + steps;

        public override string ToString() {
            if (steps > 0)
                return new string('#', steps);
            else if (steps < 0)
                return new string('♭', -steps);
            else return "";
        }

        public static PitchTransform operator -(PitchTransform that) =>
            new PitchTransform(-that.steps);

        public static PitchTransform operator +(PitchTransform a, PitchTransform b) =>
            new PitchTransform(a.steps + b.steps);

        public static PitchTransform operator -(PitchTransform a, PitchTransform b) =>
            new PitchTransform(a.steps - b.steps);

        public static SemiTone operator *(PitchTransform a, SemiTone b) =>
            a.Transform(b);

        public static readonly PitchTransform Natural = new PitchTransform(0);
        public static readonly PitchTransform Sharp = new PitchTransform(1);
        public static readonly PitchTransform DoubleSharp = new PitchTransform(2);
        public static readonly PitchTransform Flat = new PitchTransform(-1);
        public static readonly PitchTransform DoubleFlat = new PitchTransform(-2);
    }
}
