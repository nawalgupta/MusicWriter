using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class DurationField<T> {
        readonly TimeTree<T> elements_start = new TimeTree<T>();
        readonly TimeTree<T> elements_end = new TimeTree<T>();
        readonly Dictionary<T, Duration> durations = new Dictionary<T, Duration>();
        
        public Duration this[T item] {
            get { return durations[item]; }
            set {
                var oldduration = durations[item];

                elements_start[item, oldduration.Start] = value.Start;
                elements_end[item, oldduration.End] = value.End;
                durations[item] = value;
            }
        }

        public void Add(T item, Duration duration) {
            elements_start.Add(item, duration.Start);
            elements_end.Add(item, duration.End);

            durations.Add(item, duration);
        }

        public IEnumerable<T> Intersecting(Time point) =>
            elements_start.AfterOrAt(point)
                .Intersect(
                        elements_end.BeforeOrAt(point)
                    );

        public IEnumerable<T> Intersecting(Duration duration) =>
            elements_start.AfterOrAt(duration.Start)
                .Intersect(
                        elements_end.Before(duration.End)
                    );
    }
}
