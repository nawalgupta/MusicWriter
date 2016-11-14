using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Cell {
        public Time Length { get; set; }
        public float Stress { get; set; }
        
        public static IEnumerable<Cell> CreateCells(IEnumerable<Time> lengths, float secondary) {
            float stress = 1.0F;

            foreach (var length in lengths) {
                yield return new Cell {
                    Length = length,
                    Stress = stress
                };
                
                if (stress == 1.0F)
                    stress = secondary;
            }
        }

        public static IEnumerable<Cell> CutCell(Cell parent, float r) {
            var half =
                parent.Length / 2;

            yield return new Cell {
                Length = half,
                Stress = parent.Stress
            };

            yield return new Cell {
                Length = half,
                Stress = parent.Stress * r
            };
        }

        public static IEnumerable<Cell> Create_2x_2y(
                int x,
                int y,
                Time length,
                float r
            ) {
            // for making the cells of 2^x / 2^y time signatures

            var cells = new Cell[] {
                new Cell {
                    Length = length,
                    Stress = 1
                }
            };

            for (int i = 0; i < y; i++) {
                cells =
                    cells
                        .SelectMany(cell => CutCell(cell, r))
                        .ToArray();
            }

            return cells.Take((int)Math.Pow(2, x));
        }
    }
}
