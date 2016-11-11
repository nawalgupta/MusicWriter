using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicWriter.TimeSignature;

namespace MusicWriter {
    public sealed class RhythmTrack :
        IDurationField<Cell>,
        IDurationField<Simple> {
        readonly DurationField<TimeSignature> signatures =
            new DurationField<TimeSignature>();
        readonly DurationField<MeterSignature> meters =
            new DurationField<MeterSignature>();

        public void SetTimeSignature(TimeSignature signature, Duration duration) {
            foreach (var oldsingature in signatures.Intersecting(duration).ToArray()) {
                var intersection =
                    oldsingature.Duration.Intersection(duration);

                signatures.Remove(oldsingature);

                if (intersection.Length != Time.Zero)
                    signatures.Add(oldsingature.Value, intersection);
            }

            signatures.Add(signature, duration);
        }

        public void SetMeter(MeterSignature meter, Duration duration) {
            foreach (var oldmeter in meters.Intersecting(duration).ToArray()) {
                var intersection =
                    oldmeter.Duration.Intersection(duration);

                meters.Remove(oldmeter);

                if (intersection.Length > Time.Zero)
                    meters.Add(oldmeter.Value, intersection);
            }

            meters.Add(meter, duration);
        }

        //public Measure MeasureAt(Time point) {
        //    // if the point is on the edge between measures, the measure that starts at point is returned

        //    var signature =
        //        signatures.Intersecting(point).Single();

        //    var signature_duration =
        //        signatures[signature];

        //    Time simple_start;

        //    var signature_simple =
        //        signature.GetSimple(point - signature_duration.Start, out simple_start);

        //    return new Measure {
        //        Duration = new Duration {
        //            Start = signature_duration.Start + simple_start,
        //            Length = signature_simple.Length
        //        }
        //    };
        //}

        //public Cell CellAt(Time point) {
        //    var meter =
        //        meters.Intersecting(point).Single();

        //    var meter_start =
        //        meters[meter].Start;

        //    var cell =
        //        meter.CellAt(point - meter_start, meter_start);

        //    return cell;
        //}

        //public IEnumerable<Cell> CellsAt(Time point) {
        //    yield return CellAt(point);
        //}

        //public IEnumerable<Cell> CellsIn(Duration duration) => (
        //        from meter in meters.Intersecting(duration)
        //        let meter_start = meters[meter].Start
        //        let duration_mod = new Duration {
        //            Start = duration.Start - meter_start,
        //            Length = duration.Length
        //        }
        //        from cell in meter.CellsIn(duration_mod, meter_start)
        //        select cell
        //    );

        public IEnumerable<IDuratedItem<Cell>> Intersecting(Time point) =>
            meters.Intersecting_children(point);

        public IEnumerable<IDuratedItem<Cell>> Intersecting(Duration duration) =>
            meters.Intersecting_children(duration);

        public IEnumerable<IDuratedItem<TimeSignature>> TimeSignaturesInTime(Duration duration) =>
            signatures.Intersecting(duration);

        IEnumerable<IDuratedItem<Simple>> IDurationField<Simple>.Intersecting(Time point) =>
            signatures.Intersecting_children(point);

        IEnumerable<IDuratedItem<Simple>> IDurationField<Simple>.Intersecting(Duration duration) =>
            signatures.Intersecting_children(duration);
    }
}
