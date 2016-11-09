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
        readonly Key key;
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

        public Key Key {
            get { return key; }
        }

        public PitchTransform Transform {
            get { return transform; }
        }

        public NoteLayout(
                PerceptualNote core,
                int halfline,
                float x,
                Key key,
                PitchTransform transform
            ) {
            this.core = core;
            this.halfline = halfline;
            this.x = x;
            this.key = key;
            this.transform = transform;
        }
    }
}
