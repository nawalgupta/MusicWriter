using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Cell : IComparable<Cell>, IComparable {
        public Duration Duration { get; set; }
        public float Stress { get; set; }
        
        public static IEnumerable<Cell> CreateCells(IEnumerable<Time> lengths, float secondary) {
            Time offset = Time.Zero;

            float stress = 1.0F;

            foreach (var length in lengths) {
                var duration =
                    new Duration {
                        Start = offset,
                        Length = length
                    };

                yield return new Cell {
                    Duration = duration,
                    Stress = stress
                };

                offset += length;

                if (stress == 1.0F)
                    stress = secondary;
            }
        }

        public static IEnumerable<Cell> CutCell(Cell parent, float r) {
            var half =
                parent.Duration.Length / 2;

            yield return new Cell {
                Duration = new Duration {
                    Start = parent.Duration.Start,
                    Length = half
                },
                Stress = parent.Stress
            };

            yield return new Cell {
                Duration = new Duration {
                    Start = parent.Duration.Start + half,
                    Length = half
                },
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
                    Duration = new Duration {
                        Length = length
                    },
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

        public int CompareTo(Cell other) =>
            Duration.Start.CompareTo(other.Duration.Start);

        public int CompareTo(object obj) =>
            obj is Cell ?
            CompareTo((Cell)obj) :
            0;
    }
}
