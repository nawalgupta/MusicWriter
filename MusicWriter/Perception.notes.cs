using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    partial class Perception {
        IEnumerable<PerceptualNote> Decompose_Note(Note note) {
            var cells =
                track.Rhythm.CellsIn(note.Duration);

            var i = 0;

            foreach (var cell in cells) {
                var cutduration = cell.Duration.Intersection(note.Duration);

                var lengths =
                    Decompose_Time(cutduration.Length);

                foreach (var length in lengths) {
                    yield return new PerceptualNote(
                            new PerceptualNoteID(note.ID, i++),
                            cutduration,
                            length,
                            cell
                        );
                }
            }
        }

        IEnumerable<PerceptualTime> Decompose_Time(Time length) {
            var fractions_whole = length / Time.Note128th_3rd_5th_7th;

            var tuplet = TupletClass.None;

            if ((fractions_whole % 3) != 0)
                tuplet |= TupletClass.Triplet;
            else {
                fractions_whole /= 3;
            }

            if ((fractions_whole % 5) != 0)
                tuplet |= TupletClass.Pentuplet;
            else {
                fractions_whole /= 5;
            }

            if ((fractions_whole % 7) != 0)
                tuplet |= TupletClass.Septuplet;
            else {
                fractions_whole /= 7;
            }

            // fractions_base is in 
            var bits = new BitArray(new int[] { fractions_whole });

            // the pattern ...01...10... indicates a dotted note
            
            var state_dots = -1;
            var head_value = LengthClass.None;
            var place_value = LengthClass.Whole;

            // MSB (whole note) to LSB (2^-32 note)
            for (int i = 0; i < bits.Length; i++) {
                if (bits[i]) {
                    state_dots++;

                    if (state_dots == 0)
                        head_value = place_value;
                }

                place_value++;

                if (!bits[i] ||
                    !Enum.IsDefined(typeof(LengthClass), place_value)) {
                    if (state_dots >= 0) {
                        var ptime =
                            new PerceptualTime(
                                    tuplet,
                                    head_value,
                                    state_dots
                                );

                        yield return ptime;

                        state_dots = -1;
                        head_value = LengthClass.None;
                    }
                }
            }
        }
    }
}
