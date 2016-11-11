using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public static class Extensions {
        public static PitchClass GetPitchClass(this KeyClass key) {
            switch (key) {
                case KeyClass.C:
                    return PitchClass.C;

                case KeyClass.D:
                    return PitchClass.D;

                case KeyClass.E:
                    return PitchClass.E;

                case KeyClass.F:
                    return PitchClass.F;

                case KeyClass.G:
                    return PitchClass.G;

                case KeyClass.A:
                    return PitchClass.A;

                case KeyClass.B:
                    return PitchClass.B;

                default:
                    return (PitchClass)key;
            }
        }

        public static KeyClass GetNaturalKeyClass(this PitchClass pitch) {
            switch (pitch) {
                case PitchClass.C:
                case PitchClass.Dflat:
                    return KeyClass.C;

                case PitchClass.D:
                    return KeyClass.D;

                case PitchClass.Eflat:
                case PitchClass.E:
                    return KeyClass.E;

                case PitchClass.F:
                    return KeyClass.F;

                case PitchClass.Gflat:
                case PitchClass.G:
                    return KeyClass.G;

                case PitchClass.Aflat:
                case PitchClass.A:
                    return KeyClass.A;

                case PitchClass.Bflat:
                case PitchClass.B:
                    return KeyClass.B;

                default:
                    return (KeyClass)pitch;
            }
        }

        public static Key ToKey(this KeyClass keyclass, int octave) =>
            new Key(
                keyclass,
                octave
            );

        public static KeyClass ToRight(this KeyClass key) =>
            (KeyClass)(((int)key + 1) % 7);

        public static KeyClass ToLeft(this KeyClass key) =>
            (KeyClass)(((int)key - 1) % 7);

        public static IEnumerable<IDuratedItem<T>> Intersecting_children<T>(
                this IDurationField<IDurationField<T>> field,
                Time point
            ) =>
            field
                .Intersecting(point)
                .SelectMany(
                        child =>
                            child
                                .Value
                                .Intersecting(point - child.Duration.Start)
                                .Select(
                                        item =>
                                            new DuratedItem<T> {
                                                Value = item.Value,
                                                Duration = item.Duration + child.Duration.Start
                                            }
                                    )
                    );

        public static IEnumerable<IDuratedItem<T>> Intersecting_children<T>(
                this IDurationField<IDurationField<T>> field,
                Duration duration
            ) =>
            field
                .Intersecting(duration)
                .SelectMany(
                        child =>
                            child
                                .Value
                                .Intersecting(duration - child.Duration.Start)
                                .Select(
                                        item =>
                                            new DuratedItem<T> {
                                                Value = item.Value,
                                                Duration = item.Duration + child.Duration.Start
                                            }
                                    )
                    );
    }
}
