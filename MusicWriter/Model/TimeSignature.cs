using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicWriter.TimeSignature;

namespace MusicWriter {
    public sealed class TimeSignature :
        IDurationField<Simple>,
        IEquatable<TimeSignature> {
        public sealed class Simple : IEquatable<Simple> {
            public int Upper, Lower;

            public Time Length {
                get { return Time.Fraction(Upper, Lower); }
            }

            public Simple(int upper = 4, int lower = 4) {
                Upper = upper;
                Lower = lower;
            }

            public override int GetHashCode() =>
                Upper << 16 + Lower;

            public override bool Equals(object obj) =>
                ((obj as Simple)?.Equals(this)).GetValueOrDefault();

            public bool Equals(Simple that) =>
                Upper == that.Upper &&
                Lower == that.Lower;
        }

        readonly DurationCircle<Simple> simplescircle =
            new DurationCircle<Simple>();

        public List<Simple> Simples { get; } = new List<Simple>();
        
        public TimeSignature(params Simple[] simples) {
            Simples.AddRange(simples);

            SetupSimples();
        }

        public void Update() {
            SetupSimples();
        }

        void SetupSimples() {
            var length =
                Simples
                    .Aggregate(
                            Time.Zero,
                            (acc, simple) =>
                                acc + simple.Length
                        );

            simplescircle.Length = length;

            simplescircle.Clear();
            var offset = Time.Zero;
            foreach (var simple in Simples)
                simplescircle.Add(
                        simple,
                        new Duration {
                            Start = offset,
                            End = offset += simple.Length
                        }
                    );
        }

        public Simple GetSimple(Time offset) =>
            simplescircle
                .Intersecting(offset)
                .Select(cycledsimple => cycledsimple.Value)
                .SingleOrDefault();

        public IEnumerable<Simple> GetSimples(Duration duration) =>
            simplescircle
                .Intersecting(duration)
                .Select(cycledsimple => cycledsimple.Value);

        public IEnumerable<IDuratedItem<Simple>> Intersecting(Time point) =>
            simplescircle
                .Intersecting(point)
                .Cast<IDuratedItem<Simple>>();

        public IEnumerable<IDuratedItem<Simple>> Intersecting(Duration duration) =>
            simplescircle
                .Intersecting(duration)
                .Cast<IDuratedItem<Simple>>();

        public override int GetHashCode() =>
            Simples.GetHashCode();

        public override bool Equals(object obj) =>
            ((obj as TimeSignature)?.Equals(this)).GetValueOrDefault();

        public bool Equals(TimeSignature other) =>
            Simples.SequenceEqual(other.Simples);

        //public Simple GetSimple(Time offset, out Time simple_start) {
        //    var mod_offset = offset % totalsimpleslength;
        //    var skipped_offset = offset - mod_offset;

        //    for (int i = 0; i < Simples.Length; i++) {
        //        if ((mod_offset -= Simples[i].Length) < Time.Zero) {
        //            simple_start = skipped_offset;
        //            return Simples[i];
        //        }

        //        skipped_offset += Simples[i].Length;
        //    }

        //    simple_start = default(Time);
        //    return default(Simple);
        //}
    }
}
