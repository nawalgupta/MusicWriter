using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms {
    public sealed class NoteCaret {
        public Time Focus {
            get {
                switch (Side) {
                    case FocusSide.Point:
                        return Selection.Start;

                    case FocusSide.Start:
                        return Selection.Start;

                    case FocusSide.End:
                        return Selection.End;

                    default:
                        return default(Time);
                }
            }
            set {
                switch (Side) {
                    case FocusSide.Point:
                        Selection.Start = value;
                        Selection.Length = Time.Zero;
                        break;

                    case FocusSide.Start:
                        Selection.Start = value;
                        break;

                    case FocusSide.End:
                        Selection.End = value;
                        break;
                }
            }
        }

        public Duration Selection { get; }
        public FocusSide Side { get; set; } = FocusSide.Point;

        public enum FocusSide {
            Point,
            Start,
            End
        }
    }
}
