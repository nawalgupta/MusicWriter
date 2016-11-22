﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Caret {
        public Duration Duration { get; } = new Duration();

        public FocusSide Side { get; set; } = FocusSide.Both;

        public Time Focus {
            get {
                if ((Side & FocusSide.Left) == FocusSide.Left)
                    return Duration.Start;

                if ((Side & FocusSide.Right) == FocusSide.Right)
                    return Duration.End;

                return Time.Zero;
            }
            set {
                if ((Side & FocusSide.Left) == FocusSide.Left)
                    Duration.Start = value;

                if ((Side & FocusSide.Right) == FocusSide.Right)
                    Duration.End = value;
            }
        }

        public Time DeltaFocus {
            get { return Time.Zero; }
            set {
                if ((Side & FocusSide.Left) == FocusSide.Left)
                    Duration.Start += value;

                if (Side == FocusSide.Right)
                    Duration.End += value;
                else if (Side == FocusSide.Both)
                    Duration.End += Time.Zero;
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