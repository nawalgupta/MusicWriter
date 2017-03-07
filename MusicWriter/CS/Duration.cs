using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public class Duration : IEquatable<Duration>
    {
        Time offset, length;

        public Time Start {
            get { return Offset; }
            set {
                Length += Offset - value;
                Offset = value;
            }
        }

        public Time End {
            get { return Offset + Length; }
            set { Length = value - Offset; }
        }

        public virtual Time Length {
            get { return length; }
            set { length = value; }
        }

        public virtual Time Offset {
            get { return offset; }
            set { offset = value; }
        }

        public Duration Union(Duration other) =>
            other == null ?
                this :
                new Duration {
                    Start = Time.Min(Start, other.Start),
                    End = Time.Max(End, other.End)
                };

        public bool Contains(Time time) =>
            time >= Offset && time < Offset + Length;

        public Duration Intersection(Duration duration) {
            if (Start > duration.End ||
                End < duration.Start)
                return null;

            return new Duration {
                Start = Time.Max(Start, duration.Start),
                End = Time.Min(End, duration.End)
            };
        }

        public Duration Subtract_Time(Duration cut) {
            if (Start < cut.Start) {
                if (End > cut.End) {
                    // cut some time out of inside of note
                    return new Duration {
                        Start = Start,
                        Length = Length - cut.Length
                    };
                }
                else if (End > cut.Start) {
                    return
                        new Duration {
                            Start = Start,
                            End = cut.Start
                        };
                }
                else return this; // End <= cut.start
            }
            else if (Start < cut.End) {
                if (End > cut.End) {
                    return
                        new Duration {
                            Start = cut.End,
                            End = End
                        };
                }
                else return null;
            }
            // Start >= cut.End
            return this - cut.Length;
        }

        public Duration[] Subtract(Duration cut) {
            if (Start < cut.Start) {
                if (End > cut.End) {
                    return new Duration[] {
                        new Duration {
                            Start = Start,
                            Length = cut.Start
                        },
                        new Duration {
                            Start = cut.End,
                            End = End
                        }
                    };
                }
                else if (End > cut.Start) {
                    return
                        new Duration[] {
                            new Duration {
                                Start = Start,
                                End = cut.Start
                            }
                        };
                }
            }
            else if (Start < cut.End) {
                if (End > cut.End) {
                    return
                        new Duration[] {
                            new Duration {
                                Start = cut.End,
                                End = End
                            }
                        };
                }
                else return new Duration[0];
            }

            return new Duration[] { this };
        }

        public override string ToString() =>
            $"{Offset}+{Length}";

        public bool Equals(Duration that) =>
            Length == that.Length &&
            Offset == that.Offset;

        public override bool Equals(object obj) =>
            obj is Duration &&
            Equals((Duration)obj);

        public override int GetHashCode() =>
            Offset.GetHashCode() ^
            Length.GetHashCode();

        public static Duration Eternity { get; } = new Duration {
            Start = Time.Zero,
            Length = Time.Eternity
        };

        public static Duration Empty { get; } = new Duration {
            Start = Time.Zero,
            Length = Time.Zero
        };

        public static Duration operator |(Duration a, Duration b) =>
            a != null ?
                a.Union(b) :
                b;

        public static Duration operator &(Duration a, Duration b) =>
            a?.Intersection(b);

        public static Duration operator +(Duration duration, Time offset) =>
            new Duration {
                Start = duration.Start + offset,
                Length = duration.Length
            };

        public static Duration operator -(Duration duration, Time offset) =>
            new Duration {
                Start = duration.Start - offset,
                Length = duration.Length
            };

        public static Duration operator -(Duration duration, Duration cut) =>
            duration.Subtract_Time(cut);

        public bool IsInside(Duration container) =>
            container.Start <= Start &&
            container.End >= End;

        public static bool operator ==(Duration a, Duration b) {
            var anull = ReferenceEquals(a, null);
            var bnull = ReferenceEquals(b, null);

            if (anull && bnull)
                return true;
            else if (anull ^ bnull)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Duration a, Duration b) =>
            !(a == b);
    }
}
