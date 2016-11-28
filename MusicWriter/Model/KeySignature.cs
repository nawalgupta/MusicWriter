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

        public SemiTone SemiToneOfKey(DiatonicTone diatone, PitchTransform transform) =>
            transform * (this[diatone.KeyClass] * new SemiTone(diatone.KeyClass.GetPitchClass(), diatone.Octave));

        public SemiTone SemiToneOfKey(DiatonicTone diatone) =>
            this[diatone.KeyClass] * new SemiTone(diatone.KeyClass.GetPitchClass(), diatone.Octave);

        public DiatonicToneClass KeyOfPitchClass(
                ChromaticPitchClass pitch,
                out PitchTransform displaytransform
            ) {
            foreach (var keytransformkvp in keytransforms) {
                if (SemiToneOfKey(new DiatonicTone(keytransformkvp.Key, 0)).PitchClass == pitch) {
                    displaytransform = PitchTransform.Natural;
                    return keytransformkvp.Key;
                }
            }
            
            var naturalkey =
                keytransforms
                    .Sum(keytransformkvp => keytransformkvp.Value.Steps) > 0 ?
                        pitch.GetNaturalKeyClass_PreferSharps() :
                        pitch.GetNaturalKeyClass_PreferFlats();

            var naturalpitch =
                naturalkey.GetPitchClass();

            var keytransform =
                this[naturalkey];

            displaytransform = new PitchTransform((int)pitch - (int)naturalpitch);
            return naturalkey;
        }

        static readonly PitchTransform[] wholesteps = new PitchTransform[] {
                PitchTransform.DoubleSharp,
                PitchTransform.DoubleSharp,
                PitchTransform.Sharp,
                PitchTransform.DoubleSharp,
                PitchTransform.DoubleSharp,
                PitchTransform.DoubleSharp,
                PitchTransform.Sharp
            };

        public SemiTone Left(SemiTone wholetone) {
            PitchTransform transform;

            var diatone =
                this.Key(wholetone, out transform);

            //if (transform.Steps != 0)
            //    return this[diatone.KeyClass] * new SemiTone(diatone.KeyClass.GetPitchClass(), wholetone.Octave);

            var wholestep =
                -wholesteps[((int)diatone.KeyClass - 1 + 7) % 7];
            
            return
                wholestep * SemiToneOfKey(diatone, transform);
        }

        public SemiTone Right(SemiTone wholetone) {
            PitchTransform transform;

            var diatone =
                this.Key(wholetone, out transform);

            //if (transform.Steps != 0)
            //    return this[diatone.KeyClass] * new SemiTone(diatone.KeyClass.GetPitchClass(), wholetone.Octave);

            var wholestep =
                wholesteps[(int)diatone.KeyClass];

            return
                wholestep * SemiToneOfKey(diatone, transform);
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

                //TODO: should modesteps[i] or transform be used here
                pitch = modesteps[i].Transform(pitch);
                key = key.ToRight();
            }

            return signature;
        }
    }
}
