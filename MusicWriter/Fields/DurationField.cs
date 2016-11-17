using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class DurationField<T> : IDurationField<T> {
        readonly TimeTree<DuratedItem<T>> elements_start = new TimeTree<DuratedItem<T>>();
        readonly TimeTree<DuratedItem<T>> elements_end = new TimeTree<DuratedItem<T>>();
        //readonly Dictionary<T, DuratedItem<T>> durations = new Dictionary<T, DuratedItem<T>>();
        readonly List<DuratedItem<T>> items = new List<DuratedItem<T>>();

        public void Translate(Time delta) {
            foreach (var item in items)
                item.Duration.Start += delta;
        }
        
        //public Duration this[T item] {
        //    get { return durations[item].Duration; }
        //    set {
        //        var oldduration = durations[item];

        //        elements_start[oldduration, oldduration.Duration.Start] = value.Start;
        //        elements_end[oldduration, oldduration.Duration.End] = value.End;
        //        oldduration.Duration = value;
        //    }
        //}

        public void Clear(Duration duration) {
            foreach (var item in Intersecting(duration).ToArray())
                Remove(item);
        }

        public void Clear() {
            elements_end.Clear();
            elements_start.Clear();
            items.Clear();
        }

        public IEnumerable<T> All {
            get { return items.Select(item => item.Value).Distinct(); }
        }

        public IEnumerable<Duration> AllDurations {
            get { return items.Select(item => item.Duration); }
        }

        public IEnumerable<IDuratedItem<T>> AllItems {
            get { return items; }
        }
        
        public void Add(T item, Duration duration) {
            var durateditem =
                new DuratedItem<T> {
                    Value = item,
                    Duration = duration
                };

            elements_start.Add(durateditem, duration.Start);
            elements_end.Add(durateditem, duration.End);
            items.Add(durateditem);
        }

        public void Remove(IDuratedItem<T> durateditem) =>
            Remove(durateditem.Value, durateditem.Duration);

        public void Remove(T value, Duration duration) {
            var durateditem = items.Single(item => item.Value.Equals(value) && item.Duration == duration);
            
            elements_start.Remove(durateditem, duration.Start);
            elements_end.Remove(durateditem, duration.End);

            items.Remove(durateditem);
        }

        public IEnumerable<IDuratedItem<T>> Intersecting(Time point) =>
            elements_start
                .BeforeOrAt(point)
                .Select(kvp => kvp.Value)
                .Intersect(
                        elements_end
                            .After(point)
                            .Select(kvp => kvp.Value)
                    );

        public IEnumerable<IDuratedItem<T>> Intersecting(Duration duration) =>
            elements_start
                .BeforeOrAt(duration.End)
                .Select(kvp => kvp.Value)
                .Intersect(
                        elements_end
                            .After(duration.Start)
                            .Select(kvp => kvp.Value)
                    );
    }
}
