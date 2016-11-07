using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class MeterSignature {
        public DurationCircle<Cell> Cells { get; }

        public Time Length {
            get { return Cells.Length; }
        }

        public MeterSignature(Time length, Cell[] cells = null) {
            Cells = new DurationCircle<Cell>(length);

            foreach (var cell in cells)
                Cells.Add(cell, cell.Duration);
        }

        public Cell CellAt(Time offset, Time meter_start) =>
            Cells.ItemsAt(offset)
                .Select(
                        cycledcell =>
                            new Cell {
                                Duration = new Duration {
                                    Start = cycledcell.Offset + meter_start + cycledcell.Value.Duration.Start,
                                    Length = cycledcell.Value.Duration.Length
                                },
                                Stress = cycledcell.Value.Stress
                            }
                    )
                .Single();

        public IEnumerable<Cell> CellsIn(Duration duration, Time meter_start) =>
            Cells.ItemsIn(duration)
                .Select(
                        cycledcell =>
                            new Cell {
                                Duration = new Duration {
                                    Start = cycledcell.Offset + meter_start + cycledcell.Value.Duration.Start,
                                    Length = cycledcell.Value.Duration.Length
                                },
                                Stress = cycledcell.Value.Stress
                            }
                    );

        public static MeterSignature Default(TimeSignature.Simple simple) =>
            Default_specialcase_2powX_2powY(simple) ??
            Default_specialcase_3X_2powY(simple) ??
            new MeterSignature(
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
                        Cell.CreateCells(Enumerable.Repeat(length, (int)x), 0.8F).ToArray()
                    );
        }
    }
}
