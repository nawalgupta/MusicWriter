using System;
using System.Collections.Generic;
using System.IO;
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

        readonly IStorageObject storage;

        public IStorageObject Storage {
            get { return storage; }
        }

        public IDurationField<Cell> Cells {
            get { return this; }
        }

        public IDurationField<Simple> Simples {
            get { return this; }
        }

        public IDurationField<Measure> Measures {
            get { return this; }
        }
        
        public RhythmTrack(IStorageObject storage) {
            this.storage = storage;

            Setup();
        }

        void Setup() {
            var binder_timesignatures =
                new DurationFieldBinder<TimeSignature>(
                        TimeSignatures,
                        storage.GetOrMake("time-signatures")
                    );

            binder_timesignatures.Deserializer = timesig_obj => {
                using (var stream = timesig_obj.OpenRead()) {
                    using (var br = new BinaryReader(stream)) {
                        var simples = new Simple[br.ReadInt32()];

                        for (int i = 0; i < simples.Length; i++) {
                            var upper = br.ReadInt32();
                            var lower = br.ReadInt32();

                            simples[i] = new Simple(upper, lower);
                        }

                        return new TimeSignature(simples);
                    }
                }
            };

            binder_timesignatures.Serializer = (timesig_obj, timesig) => {
                using (var stream = timesig_obj.OpenWrite()) {
                    using (var bw = new BinaryWriter(stream)) {
                        bw.Write(timesig.Simples.Count);

                        for (int i = 0; i < timesig.Simples.Count; i++) {
                            var simple = timesig.Simples[i];

                            bw.Write(simple.Upper);
                            bw.Write(simple.Lower);
                        }
                    }
                }
            };

            binder_timesignatures.Start();

            var binder_metersignatures =
                new DurationFieldBinder<MeterSignature>(
                        MeterSignatures,
                        storage.GetOrMake("meter-signatures")
                    );

            binder_metersignatures.Deserializer = metersig_obj => {
                using (var stream = metersig_obj.OpenRead()) {
                    using (var br = new BinaryReader(stream)) {
                        var cells = new Cell[br.ReadInt32()];

                        for (int i = 0; i < cells.Length; i++) {
                            var length = Time.FromTicks(br.ReadInt32());
                            var stress = br.ReadSingle();

                            cells[i] = new Cell {
                                Length = length,
                                Stress = stress
                            };
                        }

                        var totallength =
                            cells.Aggregate(
                                    Time.Zero,
                                    (acc, cell) => acc + cell.Length
                                );

                        return new MeterSignature(totallength, cells);
                    }
                }
            };

            binder_metersignatures.Serializer = (metersig_obj, metersig) => {
                using (var stream = metersig_obj.OpenWrite()) {
                    using (var bw = new BinaryWriter(stream)) {
                        bw.Write(metersig.Cells.Count);

                        for (int i = 0; i < metersig.Cells.Count; i++) {
                            var cell = metersig.Cells[i];

                            bw.Write(cell.Length.Ticks);
                            bw.Write(cell.Stress);
                        }
                    }
                }
            };

            binder_metersignatures.Start();
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
