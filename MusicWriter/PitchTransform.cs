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

        public PitchClass Transform(PitchClass natural) =>
            (PitchClass)(((int)natural + steps) % 12);

        public static PitchTransform operator +(PitchTransform a, PitchTransform b) =>
            new PitchTransform(a.steps + b.steps);

        public static PitchTransform operator -(PitchTransform a, PitchTransform b) =>
            new PitchTransform(a.steps - b.steps);

        public static PitchClass operator *(PitchTransform a, PitchClass b) =>
            a.Transform(b);

        public static readonly PitchTransform Natural = new PitchTransform(0);
        public static readonly PitchTransform Sharp = new PitchTransform(1);
        public static readonly PitchTransform DoubleSharp = new PitchTransform(2);
        public static readonly PitchTransform Flat = new PitchTransform(-1);
        public static readonly PitchTransform DoubleFlat = new PitchTransform(-2);
    }
}
