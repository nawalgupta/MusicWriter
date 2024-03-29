﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public struct PerceptualTime {
        public readonly TupletClass Tuplet;
        public readonly LengthClass Length;
        public readonly int Dots;
        
        public PerceptualTime(
                TupletClass tuplet,
                LengthClass length,
                int dots
            ) {
            Tuplet = tuplet;
            Length = length;
            Dots = dots;
        }

        public Time TimeLength() {
            var basis = Time.Note;

            if (Tuplet.HasFlag(TupletClass.Half))
                basis /= 2;
            if (Tuplet.HasFlag(TupletClass.Triplet))
                basis /= 3;
            if (Tuplet.HasFlag(TupletClass.Pentuplet))
                basis /= 5;
            if (Tuplet.HasFlag(TupletClass.Septuplet))
                basis /= 7;

            switch (Length) {
                case LengthClass.Whole:
                    basis /= 1;
                    break;
                case LengthClass.Half:
                    basis /= 2;
                    break;
                case LengthClass.Quarter:
                    basis /= 4;
                    break;
                case LengthClass.Eighth:
                    basis /= 8;
                    break;
                case LengthClass.Sixteenth:
                    basis /= 16;
                    break;
                case LengthClass.ThirtySecond:
                    basis /= 32;
                    break;
                case LengthClass.SixtyFourth:
                    basis /= 64;
                    break;
                case LengthClass.OneHundredTwentyEighth:
                    basis /= 128;
                    break;
            }

            var dotsize = basis;
            for (int i = 0; i < Dots; i++) {
                dotsize /= 2;

                basis += dotsize;
            }

            return basis;
        }

        public override int GetHashCode() =>
            Dots ^ Length.GetHashCode() ^ Tuplet.GetHashCode();

        public override bool Equals(object obj) =>
            obj is PerceptualTime &&
            (PerceptualTime)obj == this;

        public static bool operator ==(PerceptualTime a, PerceptualTime b) =>
            a.Dots == b.Dots &&
            a.Length == b.Length &&
            a.Tuplet == b.Tuplet;

        public static bool operator !=(PerceptualTime a, PerceptualTime b) =>
            !(a == b);

        public static IEnumerable<KeyValuePair<PerceptualTime, Time>> Decompose(Time length) {
            // I'm sorry in advance for anyone who has to read this method.

            var fractions_whole = length / Time.Note_128th_3rd_5th_7th;

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
            var offset = Time.Zero;
            
            // bit 0 (128th note) to bit 7 (whole note)
            for (
                    var place_value =
                        tuplet != TupletClass.None ?
                            LengthClass.Half :
                            LengthClass.Whole;
                    place_value < LengthClass.Invalid;
                    place_value++
                ) {
                var i =
                    (int)LengthClass.Invalid -
                    (int)place_value -
                    1;

                if (bits[i]) {
                    state_dots++;

                    if (state_dots == 0)
                        head_value = place_value;
                }
                else {
                    if (state_dots >= 0) {
                        var ptime =
                            new PerceptualTime(
                                    tuplet,
                                    head_value,
                                    state_dots
                                );

                        yield return
                            new KeyValuePair<PerceptualTime, Time>(
                                    ptime,
                                    offset
                                );

                        state_dots = -1;
                        head_value = LengthClass.None;
                        offset += ptime.TimeLength();
                    }
                }
            }

            if (state_dots >= 0) {
                var ptime =
                    new PerceptualTime(
                            tuplet,
                            head_value,
                            state_dots
                        );

                yield return
                    new KeyValuePair<PerceptualTime, Time>(
                            ptime,
                            offset
                        );

                state_dots = -1;
                head_value = LengthClass.None;
                offset += ptime.TimeLength();
            }
        }
    }
}
