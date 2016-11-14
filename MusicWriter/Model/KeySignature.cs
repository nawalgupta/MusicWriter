using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class KeySignature {
        readonly Dictionary<DiatonicToneClass, PitchTransform> keytransforms =
            new Dictionary<DiatonicToneClass, PitchTransform>() {
                { DiatonicToneClass.C, PitchTransform.Natural },
                { DiatonicToneClass.D, PitchTransform.Natural },
                { DiatonicToneClass.E, PitchTransform.Natural },
                { DiatonicToneClass.F, PitchTransform.Natural },
                { DiatonicToneClass.G, PitchTransform.Natural },
                { DiatonicToneClass.A, PitchTransform.Natural },
                { DiatonicToneClass.B, PitchTransform.Natural },
            };

        public PitchTransform this[DiatonicToneClass key] {
            get { return keytransforms[key]; }
            set { keytransforms[key] = value; }
        }

        public DiatonicToneClass KeyOfPitchClass(
                ChromaticPitchClass pitch,
                out PitchTransform transform
            ) {
            foreach (var keytransform in keytransforms) {
                if (keytransform.Value * keytransform.Key.GetPitchClass() == pitch) {
                    transform = keytransform.Value;
                    return keytransform.Key;
                }
            }

            var naturalkey =
                pitch.GetNaturalKeyClass();

            var naturalpitch =
                naturalkey.GetPitchClass();

            transform = new PitchTransform((int)pitch - (int)naturalpitch);
            return naturalkey;
        }

        public static KeySignature Create(
                DiatonicToneClass key,
                PitchTransform transform,
                Mode mode
            ) {
            var modesteps = new PitchTransform[] {
                PitchTransform.DoubleSharp,
                PitchTransform.DoubleSharp,
                PitchTransform.Sharp,
                PitchTransform.DoubleSharp,
                PitchTransform.DoubleSharp,
                PitchTransform.DoubleSharp,
                PitchTransform.Sharp
            };

            var keysteps = new PitchTransform[] {
                PitchTransform.DoubleSharp,
                PitchTransform.DoubleSharp,
                PitchTransform.Sharp,
                PitchTransform.DoubleSharp,
                PitchTransform.DoubleSharp,
                PitchTransform.DoubleSharp,
                PitchTransform.Sharp
            };

            var mode_copy = mode;
            while (mode_copy != Mode.Major) {
                // rotate L one step
                var step0 = modesteps[0];
                for (int i = 1; i < modesteps.Length; i++)
                    modesteps[i - 1] = modesteps[i];
                modesteps[modesteps.Length] = step0;

                mode_copy = (Mode)((int)mode_copy - 1);
            }

            var key_copy = key;
            while (key_copy != DiatonicToneClass.C) {
                // rotate L one step
                var step0 = keysteps[0];
                for (int i = 1; i < keysteps.Length; i++)
                    keysteps[i - 1] = keysteps[i];
                keysteps[keysteps.Length] = step0;

                key_copy = (DiatonicToneClass)((int)key_copy - 1);
            }

            var signature =
                new KeySignature();

            var pitch = key.GetPitchClass();
            
            for (int i = 0; i < 7; i++) {
                signature[key] = transform;

                transform += modesteps[i];
                transform -= keysteps[i];

                pitch = modesteps[i] * pitch;
                key = key.ToRight();
            }

            return signature;
        }
    }
}
