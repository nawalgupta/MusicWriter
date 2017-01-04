using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Caret {
        public Duration Duration { get; } = new Duration();

        public FocusSide Side { get; set; } = FocusSide.Both;

        public Time Unit { get; set; } = Time.Note_8th;

        public Time Focus {
            get {
                if ((Side & FocusSide.Left) == FocusSide.Left)
                    return Duration.Start;

                if ((Side & FocusSide.Right) == FocusSide.Right)
                    return Duration.End;

                return Time.Zero;
            }
            set {
                if (value < Time.Zero)
                    value = Time.Zero;

                if ((Side & FocusSide.Left) == FocusSide.Left)
                    Duration.Start = value;

                if ((Side & FocusSide.Right) == FocusSide.Right)
                    Duration.End = value;
            }
        }

        public Time DeltaFocus {
            set {
                if ((Side & FocusSide.Left) == FocusSide.Left)
                    Duration.Start += value;

                if ((Side & FocusSide.Right) == FocusSide.Right)
                    Duration.End += value;
            }
        }

        [Flags]
        public enum FocusSide {
            Left = 1,
            Right = 2,
            Both = Left | Right
        }
    }
}
