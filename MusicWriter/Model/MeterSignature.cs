using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class MeterSignature :
        IDurationField<Cell> {
        public List<Cell> Cells { get; } = new List<Cell>();

        readonly DurationCircle<Cell> cellscircle =
            new DurationCircle<Cell>();

        public Time Length {
            get { return cellscircle.Length; }
        }

        public MeterSignature(Time length, Cell[] cells = null) {
            if (cells != null)
                Cells.AddRange(cells);

            Update();
        }

        public void Update() {
            SetupCells();
        }

        void SetupCells() {
            var offset = Time.Zero;

            cellscircle.Length = Cells.Aggregate(Time.Zero, (acc, cell) => acc + cell.Length);

            foreach (var cell in Cells)
                cellscircle
                    .Add(
                            cell,
                            new Duration {
                                Start = offset,
                                End = offset += cell.Length
                            }
                        );
        }

        public IEnumerable<IDuratedItem<Cell>> Intersecting(Time point) =>
            cellscircle
                .Intersecting(point)
                .Cast<IDuratedItem<Cell>>();

        public IEnumerable<IDuratedItem<Cell>> Intersecting(Duration duration) =>
            cellscircle
                .Intersecting(duration)
                .Cast<IDuratedItem<Cell>>();

        //public Cell CellAt(Time offset, Time meter_start) =>
        //    cellscircle.ItemsAt(offset)
        //        .Select(
        //                cycledcell =>
        //                    new Cell {
        //                        Duration = new Duration {
        //                            Start = cycledcell.Offset + meter_start + cycledcell.Value.Duration.Start,
        //                            Length = cycledcell.Value.Duration.Length
        //                        },
        //                        Stress = cycledcell.Value.Stress
        //                    }
        //            )
        //        .Single();

        //public IEnumerable<Cell> CellsIn(Duration duration, Time meter_start) =>
        //    cellscircle.ItemsIn(duration)
        //        .Select(
        //                cycledcell =>
        //                    new Cell {
        //                        Duration = new Duration {
        //                            Start = cycledcell.Offset + meter_start + cycledcell.Value.Duration.Start,
        //                            Length = cycledcell.Value.Duration.Length
        //                        },
        //                        Stress = cycledcell.Value.Stress
        //                    }
        //            );

        public static MeterSignature Default(TimeSignature.Simple simple) =>
            Default_specialcase_2powX_2powY(simple) ??
            Default_specialcase_3X_2powY(simple) ??
            new MeterSignature(
                    simple.Length,
                    Cell.CreateCells(Enumerable.Repeat(Time.Note / simple.Lower, simple.Upper), 0.8F)
                        .ToArray()
                );

        private static MeterSignature Default_specialcase_2powX_2powY(TimeSignature.Simple simple) {
            var x = Math.Log(simple.Upper, 2);
            var y = Math.Log(simple.Lower, 2);

            if (Math.Round(x) != x ||
                Math.Round(y) != y)
                return null;

            return
                new MeterSignature(
                        simple.Length,
                        Cell.Create_2x_2y(
                                (int)x,
                                (int)y,
                                simple.Length,
                                0.8F
                            )
                            .ToArray()
                    );
        }

        private static MeterSignature Default_specialcase_3X_2powY(TimeSignature.Simple simple) {
            var x = simple.Upper / 3F;
            var y = Math.Log(simple.Lower, 2);

            if (Math.Round(x) != x ||
                Math.Round(y) != y ||
                y < 3)
                return null;

            // where y >= 3, x >= 1
            var length =
                Time.Fraction(simple.Upper, simple.Lower);

            return
                new MeterSignature(
                        simple.Length,
                        Cell.CreateCells(Enumerable.Repeat(length, (int)x), 0.8F).ToArray()
                    );
        }
    }
}
