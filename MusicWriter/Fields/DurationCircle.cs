using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class DurationCircle<T> : IDurationField<T> {
        readonly DurationField<T> field1 = new DurationField<T>();
        readonly DurationField<T> field2 = new DurationField<T>();

        Time _length = Time.Zero;

        public Time Length {
            get { return _length; }
            set {
                field1.Translate(_length - value);

                _length = value;
            }
        }

        public IEnumerable<T> All {
            get { return field1.All; }
        }

        //public Duration this[T item] {
        //    get { return field1[item]; }
        //    set {
        //        field1[item] = value;
        //        field2[item] = new Duration {
        //            Start = value.Start - Length,
        //            Length = value.Length
        //        };
        //    }
        //}
        
        public void Add(T item, Duration duration) {
            var start = duration.Start;
            start %= Length;

            field1.Add(item, new Duration {
                Start = start,
                Length = duration.Length
            });

            field2.Add(item, new Duration {
                Start = start - Length,
                Length = duration.Length
            });
        }

        public void Remove(IDuratedItem<T> item) {
            field1.Remove(item);
            field2.Remove(item);
        }

        public void Remove(T value, Duration duration) {
            field1.Remove(value, duration);
            field2.Remove(value, duration);
        }

        public IEnumerable<IDuratedItem<T>> Intersecting(Time point) =>
            IntersectingCycled(point).Cast<IDuratedItem<T>>();

        public IEnumerable<CycledDuratedItem<T>> IntersectingCycled(Time point) =>
            Intersecting_compter(point).Distinct();

        public void Clear() {
            field1.Clear();
            field2.Clear();
        }

        IEnumerable<CycledDuratedItem<T>> Intersecting_compter(Time point) {
            var cycles = point / Length;
            var offset = Length * cycles;
            point %= Length;

            foreach (var item in field1.Intersecting(point))
                yield return new CycledDuratedItem<T>(
                        item.Value,
                        new Duration {
                            Start = offset + item.Duration.Start,
                            Length = item.Duration.Length
                        },
                        cycles  
                    );

            foreach (var item in field2.Intersecting(point))
                yield return new CycledDuratedItem<T>(
                        item.Value,
                        new Duration {
                            Start = offset + Length + item.Duration.Start,
                            Length = item.Duration.Length
                        },
                        cycles - 1
                    );
        }

        public IEnumerable<IDuratedItem<T>> Intersecting(Duration duration) =>
            IntersectingCycled(duration).Cast<IDuratedItem<T>>();

        public IEnumerable<CycledDuratedItem<T>> IntersectingCycled(Duration duration) =>
            Intersecting_compter(duration).Distinct();

        IEnumerable<CycledDuratedItem<T>> Intersecting_compter(Duration duration) {
            var cycles = duration.Start / Length;
            var offset = cycles * Length;

            while (duration.Length > Length) {
                foreach (var item in field1.AllItems)
                    if (item.Duration.Start >= duration.Start - Length * cycles)
                        yield return
                            new CycledDuratedItem<T>(
                                    item.Value,
                                    item.Duration + offset,
                                    cycles
                                );

                duration.Start += Length;

                cycles++;
                offset += Length;
            }

            duration = new Duration {
                Start = duration.Start % Length,
                Length = duration.Length
            };

            if (duration.End > Length) {
                if (duration.Length > Length) {
                    foreach (var item in field1.AllItems)
                        yield return new CycledDuratedItem<T>(
                                item.Value,
                                new Duration {
                                    Start = offset + item.Duration.Start,
                                    Length = item.Duration.Length
                                },
                                cycles
                            );

                    yield break;
                }
                else {
                    var newduration =
                        new Duration {
                            Start = duration.Start - Length,
                            Length = duration.Length
                        };

                    foreach (var item in field2.Intersecting(newduration))
                        yield return new CycledDuratedItem<T>(
                                item.Value,
                                new Duration {
                                    Start = offset + Length + item.Duration.Start,
                                    Length = item.Duration.Length
                                },
                                cycles - 1
                            );
                }
            }

            foreach (var item in field1.Intersecting(duration))
                yield return new CycledDuratedItem<T>(
                        item.Value,
                        new Duration {
                            Start = offset + item.Duration.Start,
                            Length = item.Duration.Length
                        },
                        cycles
                    );
        }
    }
}
