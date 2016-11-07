using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Duration {
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
    }
}
