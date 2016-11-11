using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Duration : IEquatable<Duration> {
        Time offset, length;

        public Time Start {
            get { return offset; }
            set { offset = value; }
        }

        public Time End {
            get { return offset + length; }
            set { length = value - offset; }
        }

        public Time Length {
            get { return length; }
            set { length = value; }
        }

        public Duration Intersection(Duration duration) {
            if (Start > duration.End ||
                End < duration.Start)
                return null;

            return new Duration {
                Start = Time.Max(Start, duration.Start),
                End = Time.Min(End, duration.End)
            };
        }

        public bool Equals(Duration that) =>
            length == that.length &&
            offset == that.offset;

        public override bool Equals(object obj) =>
            obj is Duration &&
            Equals((Duration)obj);

        public override int GetHashCode() =>
            offset.GetHashCode() ^ 
            length.GetHashCode();

        public static readonly Duration Eternity = new Duration {
            Start = Time.Zero,
            Length = Time.Eternity
        };

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

        public static bool operator ==(Duration a, Duration b) =>
            a.Equals(b);

        public static bool operator !=(Duration a, Duration b) =>
            !a.Equals(b);
    }
}
