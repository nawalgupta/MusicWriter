using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class RhythmTrack {
        readonly DurationField<TimeSignature> signatures =
            new DurationField<TimeSignature>();
        readonly DurationField<MeterSignature> meters =
            new DurationField<MeterSignature>();
        
        public Measure MeasureAt(Time point) {
            // if the point is on the edge between measures, the measure that starts at point is returned

            var signature =
                signatures.Intersecting(point).Single();

            var signature_duration =
                signatures[signature];

            Time simple_start;

            var signature_simple =
                signature.GetSimple(point - signature_duration.Start, out simple_start);

            return new Measure {
                Duration = new Duration {
                    Start = signature_duration.Start + simple_start,
                    Length = signature_simple.Length
                }
            };
        }

        public Cell CellAt(Time point) {
            var meter =
                meters.Intersecting(point).Single();

            var meter_start =
                meters[meter].Start;

            var cell =
                meter.CellAt(point - meter_start, meter_start);

            return cell;
        }

        public IEnumerable<Cell> CellsIn(Duration duration) => (
                from meter in meters.Intersecting(duration)
                let meter_start = meters[meter].Start
                let duration_mod = new Duration {
                    Start = duration.Start - meter_start,
                    Length = duration.Length
                }
                from cell in meter.CellsIn(duration_mod, meter_start)
                select cell
            );
    }
}
