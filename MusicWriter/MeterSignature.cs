using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class MeterSignature {
        public Cell[] Cells { get; set; }

        public MeterSignature() {
        }

        public MeterSignature(params Cell[] cells) {
            Cells = cells;

            Array.Sort(Cells);
        }
        
        public Cell CellAt(Time offset) {
            var cells_length =
                Cells.Aggregate(Time.Zero, (length, cell) => length + cell.Duration.Length);

            offset %= cells_length;

            for (int i = 0; i < Cells.Length; i++) {
                var cell = Cells[i];

                if (offset < cell.Duration.Length)
                    return cell;

                offset -= cell.Duration.Length;
            }

            // code shouldn't reach here
            return Cells[0];
        }

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
