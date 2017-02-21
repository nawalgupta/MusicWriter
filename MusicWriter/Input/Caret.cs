using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Caret {
        public ObservableDuration Duration { get; } = new ObservableDuration();

        public ObservableProperty<FocusSide> Side { get; } = new ObservableProperty<FocusSide>(FocusSide.Both);

        public ObservableProperty<Time> Unit { get; } = new ObservableProperty<Time>(Time.Note_4th);

        public Time Focus {
            get {
                if ((Side.Value & FocusSide.Left) == FocusSide.Left)
                    return Duration.Start;

                if ((Side.Value & FocusSide.Right) == FocusSide.Right)
                    return Duration.End;

                return Time.Zero;
            }
            set {
                if (value < Time.Zero)
                    value = Time.Zero;

                if ((Side.Value & FocusSide.Left) == FocusSide.Left)
                    Duration.Start = value;

                if ((Side.Value & FocusSide.Right) == FocusSide.Right)
                    Duration.End = value;
            }
        }

        public Time DeltaFocus {
            set {
                if ((Side.Value & FocusSide.Left) == FocusSide.Left)
                    Duration.Start += value;

                if ((Side.Value & FocusSide.Right) == FocusSide.Right)
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
