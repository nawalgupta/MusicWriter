using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicWriter {
    public sealed class Note {
        public Time Offset { get; set; }
        public Time Length { get; set; }
        public float Velocity { get; set; } = 0.5f;
    }
}
