﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Tone {
        public PitchClass PitchClass { get; set; } = PitchClass.C;
        public int Octave { get; set; } = 0;
    }
}