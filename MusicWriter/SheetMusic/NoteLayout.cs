using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class NoteLayout {
        readonly PerceptualNote core;
        readonly int halfline;
        readonly float x;
        readonly float width;
        readonly DiatonicTone key;
        readonly PitchTransform transform;

        public PerceptualNote Core {
            get { return core; }
        }

        public int HalfLine {
            get { return halfline; }
        }

        public float X {
            get { return x; }
        }

        public float Width {
            get { return width; }
        }

        public DiatonicTone Key {
            get { return key; }
        }

        public PitchTransform Transform {
            get { return transform; }
        }

        public NoteLayout(
                PerceptualNote core,
                int halfline,
                float x,
                float width,
                DiatonicTone key,
                PitchTransform transform
            ) {
            this.core = core;
            this.halfline = halfline;
            this.x = x;
            this.width = width;
            this.key = key;
            this.transform = transform;
        }
    }
}
