using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class MeasureLayout {
        readonly Duration duration;
        readonly PerceptualNote[] notes;
        readonly List<ChordLayout> chords = new List<ChordLayout>();
        readonly Staff staff;
        readonly KeySignature keysignature;
        
        public Duration Duration {
            get { return duration; }
        }

        public PerceptualNote[] Notes {
            get { return notes; }
        }

        public Staff Staff {
            get { return staff; }
        }
        
        public KeySignature KeySignature {
            get { return keysignature; }
        }

        public float ScaleX { get; private set; } = 1.0F;

        public IEnumerable<ChordLayout> Chords {
            get { return chords; }
        }

        public MeasureLayout(
                Duration duration,
                PerceptualNote[] notes,
                Staff staff,
                KeySignature keysignature
            ) {
            this.duration = duration;
            this.notes = notes;
            this.staff = staff;
            this.keysignature = keysignature;

            BreakFractions();
            LayoutChords();
            LayoutBeams();
        }

        void BreakFractions() {
            var fractionalpieces =
                notes.Select(note => ((Time.Note * 8) / note.Duration.Length) / 8F);

            var avgmin_fractionalpieces =
                Statistics.AvgMin(fractionalpieces);

            ScaleX *= avgmin_fractionalpieces;
        }

        NoteLayout LayoutNote(PerceptualNote note) {
            PitchTransform transform;

            var key =
                keysignature.Key(note.Note.Tone, out transform);
            
            var halfline =
                staff.GetHalfLine(key);

            var x =
                ToVirtualPX(note.Duration.Start - duration.Start);

            var width =
                ToVirtualPX(note.Note.Duration.Length);

            return
                new NoteLayout(
                        note,
                        halfline,
                        x,
                        width,
                        key,
                        transform
                    );
        }

        void LayoutChords() {
            var buckets =
                notes
                    .Select(LayoutNote)
                    .GroupBy(note => note.Core.Duration);

            foreach (var bucket in buckets) {
                var direction =
                    staff
                        .GetStemDirection(
                                bucket
                                    .OrderBy(note => note.HalfLine)
                                    .Median()
                                    .Key
                            );
                
                var chordlayout =
                    new ChordLayout(
                            bucket.ToArray()
                        );

                if (chordlayout.Length.Length < LengthClass.Half)
                    direction = NoteStemDirection.None;

                // ask the property graph what direction the stem should be
                chordlayout.StemDirection = direction;

                if (direction == NoteStemDirection.Down) {
                    chordlayout.StemSide = NoteStemSide.Left;
                    chordlayout.StemStartHalfLines = chordlayout.Notes.Min(note => note.HalfLine) - 5F;
                }
                else {
                    chordlayout.StemSide = NoteStemSide.Right;
                    chordlayout.StemStartHalfLines = chordlayout.Notes.Max(note => note.HalfLine) + 5F;
                }

                chords.Add(chordlayout);
            }
        }

        void LayoutBeams() {
            var beamsequences =
                new List<ChordLayout[]>();

            foreach (var chord in chords.OrderBy(chord => chord.Duration.Start).OrderByDescending(chord => chord.Duration.Length.Ticks)) {
                if (chord.Flags != 0)
                    continue; // already hit

                if (chord.Length.Length > LengthClass.Quarter) {
                    var cell =
                        chord.Notes[0].Core.Cell;

                    if (chord.Duration.End < cell.Duration.End) {
                        var train =
                            new List<ChordLayout>();

                        var last = chord;

                        while (last.Duration.End < cell.Duration.End) {
                            train.Add(last);

                            var candidates =
                                chords
                                    .Where(candidate => candidate.Duration.Start == last.Duration.End)
                                    .Where(candidate => candidate.Duration.Length < Time.Note_4th)
                                    .ToArray();

                            var bestcandidate =
                                candidates
                                    .OrderBy(
                                            candidate =>
                                                candidate.Notes.Min(note => Math.Abs(note.HalfLine - last.Notes.Min(note2 => note2.HalfLine)))
                                        )
                                    .FirstOrDefault();

                            if (bestcandidate == null)
                                break;

                            last = bestcandidate;
                        }

                        if (last.Duration.End == cell.Duration.End)
                            train.Add(last);

                        if (train.Count > 1) {
                            var Xs =
                                train.Select(item => item.X).ToArray();

                            var Ys =
                                train.Select(item => item.Notes.Average(note => (float)note.HalfLine)).ToArray();

                            float m, b;
                            if (!Statistics.LinearRegression(Xs, Ys, out m, out b)) {
                                b = 0;
                                m = 0;
                            }

                            var stemdirection = NoteStemDirection.None;
                            var stemside = NoteStemSide.None;
                            if (train.Average(chordi => chordi.Notes.Average(notei => (float)notei.HalfLine)) >= staff.MiddleHalfLine) {
                                stemdirection = NoteStemDirection.Down;
                                stemside = NoteStemSide.Left;
                            }
                            else {
                                stemdirection = NoteStemDirection.Up;
                                stemside = NoteStemSide.Right;
                            }

                            float stemoffset = 3;
                            if (stemdirection == NoteStemDirection.Down)
                                stemoffset *= -1;

                            LengthClass lastlengthclass = LengthClass.Invalid;
                            for (var i = 0; i < train.Count; i++) {
                                var is1 = i == 0;
                                var is2 = i == 1;
                                var item = train[i];
                                
                                item.StemDirection = stemdirection;
                                item.StemSide = stemside;
                                item.FlagSlope = m;
                                item.LastLengthClass = lastlengthclass;
                                lastlengthclass = item.Length.Length;

                                if (is1) {
                                    item.FlagDirection = FlagDirection.Right;
                                    item.FlagLength = ToVirtualPX(item.Duration.Length);
                                }
                                else {
                                    item.FlagDirection = FlagDirection.Left;
                                    item.FlagLength = ToVirtualPX(train[i - 1].Duration.Length);
                                }

                                if (is1 || is2)
                                    item.FlagLength /= 2F;
                                else item.Past2nd = true;

                                item.TiedFlags = item.Length.Length - LengthClass.Quarter;
                                //TODO: also identify which are free flags

                                item.StemStartHalfLines = stemoffset + item.X * m + b;
                            }
                        }
                        else {
                            chord.FreeFlags = chord.Length.Length - LengthClass.Quarter;

                            if (chord.StemDirection == NoteStemDirection.Down)
                                chord.FlagDirection = FlagDirection.Left;
                            else {
                                chord.FlagDirection = FlagDirection.Right;
                            }
                        }
                    }
                    else {
                        chord.FreeFlags = chord.Length.Length - LengthClass.Quarter;
                        
                        if (chord.StemDirection == NoteStemDirection.Down)
                            chord.FlagDirection = FlagDirection.Left;
                        else {
                            chord.FlagDirection = FlagDirection.Right;
                        }
                    }
                }
            }
        }

        float ToVirtualPX(Time time) =>
            (time / (duration.Length / 1000)) / 1000F;
    }
}
