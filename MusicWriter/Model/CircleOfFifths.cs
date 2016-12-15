using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public static class CircleOfFifths
    {
        public static ChromaticPitchClass Index(int i, ChromaticPitchClass basis = ChromaticPitchClass.C) {
            while (i > 0) {
                basis = (ChromaticPitchClass)(((int)basis + 7) % 12);
                i--;
            }

            while (i < 0) {
                basis = (ChromaticPitchClass)(((int)basis - 7) % 12);
                i++;
            }

            return basis;
        }
        
        public static DiatonicToneClass Index(int i, ChromaticPitchClass basis, out PitchTransform transform) {
            var color = Index(i, basis);

            DiatonicToneClass diatonicclass;

            if (i > 0)
                diatonicclass = color.GetNaturalKeyClass_PreferSharps();
            else diatonicclass = color.GetNaturalKeyClass_PreferFlats();

            transform = new PitchTransform(diatonicclass.GetPitchClass() - color);

            return diatonicclass;
        }
    }
}
