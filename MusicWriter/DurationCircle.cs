using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class DurationCircle<T> {
        readonly DurationField<T> field1 = new DurationField<T>();
        readonly DurationField<T> field2 = new DurationField<T>();
        readonly Time length;

        public Time Length {
            get { return length; }
        }

        public Duration this[T item] {
            get { return field1[item]; }
            set {
                field1[item] = value;
                field2[item] = new Duration {
                    Start = value.Start - length,
                    Length = value.Length
                };
            }
        }

        public DurationCircle(Time length) {
            this.length = length;
        }

        public void Add(T item, Duration duration) {
            var start = duration.Start;
            start %= length;

            field1.Add(item, new Duration {
                Start = start,
                Length = duration.Length
            });

            field2.Add(item, new Duration {
                Start = start - length,
                Length = duration.Length
            });
        }

        public void Remove(T item) {
            field1.Remove(item);
            field2.Remove(item);
        }

        public IEnumerable<CycledItem<T>> ItemsAt(Time point) =>
            ItemsAt_compter(point).Distinct();

        IEnumerable<CycledItem<T>> ItemsAt_compter(Time point) {
            var cycles = point / length;
            var offset = length * cycles;
            point %= length;

            foreach (var item in field1.Intersecting(point))
                yield return new CycledItem<T>(
                        item,
                        offset,
                        cycles  
                    );

            foreach (var item in field2.Intersecting(point))
                yield return new CycledItem<T>(
                        item,
                        offset,
                        cycles
                    );
        }

        public IEnumerable<CycledItem<T>> ItemsIn(Duration duration) =>
            ItemsIn_compter(duration).Distinct();

        IEnumerable<CycledItem<T>> ItemsIn_compter(Duration duration) {
            var cycles = duration.Start / length;
            var offset = cycles * length;

            duration = new Duration {
                Start = duration.Start % length,
                Length = duration.Length
            };

            if (duration.End > length) {
                if (duration.Length > length) {
                    foreach (var item in field1.All)
                        yield return new CycledItem<T>(
                                item,
                                offset,
                                cycles
                            );

                    yield break;
                }
                else {
                    var newduration =
                        new Duration {
                            Start = duration.Start - length,
                            Length = duration.Length
                        };

                    foreach (var item in field2.Intersecting(newduration))
                        yield return new CycledItem<T>(
                                item,
                                offset - length,
                                cycles - 1
                            );
                }
            }

            foreach (var item in field1.Intersecting(duration))
                yield return new CycledItem<T>(
                        item,
                        offset,
                        cycles
                    );
        }
    }
}
