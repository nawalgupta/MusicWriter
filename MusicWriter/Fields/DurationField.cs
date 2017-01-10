using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class DurationField<T> : IDurationField<T> {
        readonly TimeTree<DuratedItem<T>> elements_start = new TimeTree<DuratedItem<T>>();
        readonly TimeTree<DuratedItem<T>> elements_end = new TimeTree<DuratedItem<T>>();
        readonly List<DuratedItem<T>> items = new List<DuratedItem<T>>();

        public ObservableProperty<Duration> GeneralDuration { get; } =
            new ObservableProperty<Duration>();

        public event FieldChangedDelegate FieldChanged;

        public event DurationFieldItemDelegate ItemAdded;
        public event DurationFieldItemMovedDelegate ItemMoved;
        public event DurationFieldItemChangedDelegate ItemChanged;
        public event DurationFieldItemDelegate ItemRemoved;

        public delegate void DurationFieldItemDelegate(Duration duration, T value);
        public delegate void DurationFieldItemMovedDelegate(Duration oldduration, Duration newduration, T value);
        public delegate void DurationFieldItemChangedDelegate(Duration duration, T oldvalue, T newvalue);

        public void Translate(Time delta) {
            if (GeneralDuration.Value == null)
                return;

            if (delta == Time.Zero)
                return;

            foreach (var item in items) {
                var newduration = item.Duration + delta;

                ItemMoved?.Invoke(item.Duration, newduration, item.Value);

                item.Duration = newduration;
            }

            elements_start.Translate(delta);
            elements_end.Translate(delta);

            FieldChanged?.Invoke(GeneralDuration.Value | (GeneralDuration.Value + delta));
            GeneralDuration.Value += delta;
        }

        public void Update(IDuratedItem<T> old, T newvalue) {
            var durateditem =
                old as DuratedItem<T> ??
                items.FirstOrDefault(
                        item =>
                            item.Duration == old.Duration && 
                            item.Value.Equals(old.Value)
                    );

            if (!old.Value.Equals(newvalue)) {
                ItemChanged?.Invoke(old.Duration, old.Value, newvalue);
                durateditem.Value = newvalue;
            }
        }

        public void Update(Duration duration, T oldvalue, T newvalue) =>
            Update(items.FirstOrDefault(item => item.Duration == duration && item.Value.Equals(oldvalue)), newvalue);

        public void UpdateUnique(Duration duration, T newvalue) =>
            Update(items.FirstOrDefault(item => item.Duration == duration), newvalue);

        //public void Translate(Duration startduration, Time delta) {
        //    var items =
        //        elements_start
        //            .AfterOrAt(startduration.Start)
        //            .Select(item => item.Value)
        //            .Intersect(
        //                    elements_start
        //                        .Before(startduration.End)
        //                        .Select(item => item.Value)
        //                )
        //            .ToArray();

        //    foreach (var item in items) {
        //        Remove(item);

        //        var newduration =
        //            new Duration {
        //                Start = item.Duration.Start + delta,
        //                Length = item.Duration.Length
        //            };

        //        Add(item.Value, newduration);
        //    }
        //    //TODO: implement updating the GeneralDuration variable and invoke FieldChanged event
        //}

        public void Clear(Duration duration) {
            if (duration == null || duration.Length == Time.Zero)
                return;

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

        public bool HasItem(Duration duration) =>
            items.Any(item => item.Duration == duration);

        public bool HasItem(Duration duration, T value) =>
            items.Any(item => item.Duration == duration && item.Value.Equals(value));

        void GeneralDuration_recalc_start() =>
            GeneralDuration.Value =
                new Duration {
                    Start = items.Max(item => item.Duration.Start),
                    End = GeneralDuration.Value.End
                };

        void GeneralDuration_recalc_end() =>
            GeneralDuration.Value =
                new Duration {
                    Start = GeneralDuration.Value.Start,
                    End = items.Min(item => item.Duration.End)
                };

        void GeneralDuration_recalc() =>
            GeneralDuration.Value =
                new Duration {
                    Start = items.Max(item => item.Duration.Start),
                    End = items.Min(item => item.Duration.End)
                };

        public void Add(T item, Duration duration) {
            var durateditem =
                new DuratedItem<T> {
                    Value = item,
                    Duration = duration
                };

            elements_start.Add(durateditem, duration.Start);
            elements_end.Add(durateditem, duration.End);
            items.Add(durateditem);

            FieldChanged?.Invoke(duration);
            GeneralDuration.Value |= duration;

            ItemAdded?.Invoke(duration, item);
        }

        public void Move(T value, Duration oldduration, Duration newduration) =>
            Move(
                    items.FirstOrDefault(item => item.Duration == oldduration && item.Value.Equals(value)),
                    newduration
                );

        public void Move(IDuratedItem<T> item, Duration newduration) {
            var durateditem =
                item as DuratedItem<T> ??
                items.FirstOrDefault(_ => _.Duration == item.Duration && _.Value.Equals(item.Value));

            var oldduration =
                item.Duration;

            if (oldduration == newduration)
                return;

            if (oldduration.Start != newduration.Start) {
                elements_start.Remove(durateditem, oldduration.Start);
                elements_start.Add(durateditem, newduration.Start);
            }

            if (oldduration.End != newduration.End) {
                elements_end.Remove(durateditem, oldduration.End);
                elements_end.Add(durateditem, newduration.End);
            }
            
            durateditem.Duration = newduration;

            ItemMoved?.Invoke(oldduration, newduration, item.Value);
        }

        public bool TryMoveUnique(Duration oldduration, Duration newduration) {
            var durateditem = items.FirstOrDefault(item => item.Duration == oldduration);

            if (durateditem == null)
                return false;

            if (oldduration.Start != newduration.Start) {
                elements_start.Remove(durateditem, oldduration.Start);
                elements_start.Add(durateditem, newduration.Start);
            }

            if (oldduration.End != newduration.End) {
                elements_end.Remove(durateditem, oldduration.End);
                elements_end.Add(durateditem, newduration.End);
            }

            durateditem.Duration = newduration;

            ItemMoved?.Invoke(oldduration, newduration, durateditem.Value);

            return true;
        }

        public void Remove(IDuratedItem<T> durateditem) {
            var duration = durateditem.Duration;
            var durateditem_real =
                durateditem as DuratedItem<T> ??
                items.FirstOrDefault(item => item.Value.Equals(durateditem.Value) && item.Duration == duration);

            elements_start.Remove(durateditem_real, duration.Start);
            elements_end.Remove(durateditem_real, duration.End);

            items.Remove(durateditem_real);

            FieldChanged?.Invoke(duration);

            if (GeneralDuration.Value == duration)
                GeneralDuration_recalc();
            else if (duration.Contains(GeneralDuration.Value.End))
                GeneralDuration_recalc_end();
            else if (duration.Contains(GeneralDuration.Value.Start))
                GeneralDuration_recalc_start();

            ItemRemoved?.Invoke(duration, durateditem.Value);
        }

        public void Remove(T value, Duration duration) {
            var durateditem = items.Single(item => item.Value.Equals(value) && item.Duration == duration);

            elements_start.Remove(durateditem, duration.Start);
            elements_end.Remove(durateditem, duration.End);

            items.Remove(durateditem);

            FieldChanged?.Invoke(duration);

            if (GeneralDuration.Value == duration)
                GeneralDuration_recalc();
            else if (duration.Contains(GeneralDuration.Value.End))
                GeneralDuration_recalc_end();
            else if (duration.Contains(GeneralDuration.Value.Start))
                GeneralDuration_recalc_start();

            ItemRemoved?.Invoke(duration, value);
        }

        public void RemoveUnique(Duration duration) {
            var durateditem = items.Single(item => item.Duration == duration);

            elements_start.Remove(durateditem, duration.Start);
            elements_end.Remove(durateditem, duration.End);

            items.Remove(durateditem);

            FieldChanged?.Invoke(duration);

            if (GeneralDuration.Value == duration)
                GeneralDuration_recalc();
            else if (duration.Contains(GeneralDuration.Value.End))
                GeneralDuration_recalc_end();
            else if (duration.Contains(GeneralDuration.Value.Start))
                GeneralDuration_recalc_start();

            ItemRemoved?.Invoke(duration, durateditem.Value);
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
            duration.Length == Time.Zero ?
                Intersecting(duration.Start) :
                Enumerable.Intersect(
                        elements_start
                            .Before(duration.End)
                            .Select(kvp => kvp.Value),
                        elements_end
                            .After(duration.Start)
                            .Select(kvp => kvp.Value)
                    );
    }
}
