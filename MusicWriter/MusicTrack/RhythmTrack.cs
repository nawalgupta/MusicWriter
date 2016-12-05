using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicWriter.TimeSignature;

namespace MusicWriter {
    public sealed class RhythmTrack :
        IDurationField<Cell>,
        IDurationField<Simple>,
        IDurationField<Measure> {
        public DurationField<TimeSignature> TimeSignatures { get; } =
            new DurationField<TimeSignature>();

        public DurationField<MeterSignature> MeterSignatures { get; } =
            new DurationField<MeterSignature>();

        public IDurationField<Cell> Cells {
            get { return this; }
        }

        public IDurationField<Simple> Simples {
            get { return this; }
        }

        public IDurationField<Measure> Measures {
            get { return this; }
        }
        
        public IEnumerable<IDuratedItem<Cell>> Intersecting(Time point) =>
            MeterSignatures.Intersecting_children(point);

        public IEnumerable<IDuratedItem<Cell>> Intersecting(Duration duration) =>
            MeterSignatures.Intersecting_children(duration);

        public IEnumerable<IDuratedItem<TimeSignature>> TimeSignaturesInTime(Duration duration) =>
            TimeSignatures.Intersecting(duration);

        IEnumerable<IDuratedItem<Simple>> IDurationField<Simple>.Intersecting(Time point) =>
            TimeSignatures.Intersecting_children(point);

        IEnumerable<IDuratedItem<Simple>> IDurationField<Simple>.Intersecting(Duration duration) =>
            TimeSignatures.Intersecting_children(duration);

        IEnumerable<IDuratedItem<Measure>> IDurationField<Measure>.Intersecting(Time point) =>
            TimeSignatures
                .Intersecting_children(point)
                .Select(
                        simple_item =>
                            new DuratedItem<Measure> {
                                Duration = simple_item.Duration,
                                Value = new Measure()
                            }
                    );

        IEnumerable<IDuratedItem<Measure>> IDurationField<Measure>.Intersecting(Duration duration) =>
            TimeSignatures
                .Intersecting_children(duration)
                .Select(
                        simple_item =>
                            new DuratedItem<Measure> {
                                Duration = simple_item.Duration,
                                Value = new Measure()
                            }
                    );
    }
}
