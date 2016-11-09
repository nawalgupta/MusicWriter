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
        readonly Track track;
        readonly Staff staff;
        readonly KeySignature keysignature;
        
        public Duration Duration {
            get { return duration; }
        }

        public PerceptualNote[] Notes {
            get { return notes; }
        }
        
        public Track Track {
            get { return track; }
        }

        public Staff Staff {
            get { return staff; }
        }

        public float ScaleX { get; private set; } = 1.0F;

        public MeasureLayout(
                Duration duration,
                PerceptualNote[] notes,
                Track track,
                Staff staff
            ) {
            this.duration = duration;
            this.notes = notes;
            this.track = track;
            this.staff = staff;

            keysignature =
                track.Adornment.KeySignatures.Intersecting(duration).Single();

            BreakFractions();

            LayoutChords();

        }

        void BreakFractions() {
            var fractionalpieces =
                notes.Select(note => ((Time.Note * 8) / note.Duration.Length) * 8F);

            var avgmin_fractionalpieces =
                Statistics.AvgMin(fractionalpieces);

            ScaleX *= avgmin_fractionalpieces;
        }

        NoteLayout LayoutNote(PerceptualNote note) {
            PitchTransform transform;
            var keyclass =
                keysignature.KeyOfPitchClass(note.Note.Tone.PitchClass, out transform);

            var key =
                new Key(
                        keyclass,
                        note.Note.Tone.Octave
                    );

            var halfline =
                staff.GetHalfLine(key);

            var x =
                ToVirtualPX(note.Duration.Start);

            return
                new NoteLayout(
                        note,
                        halfline,
                        x,
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
                            bucket.ToArray(),
                            bucket.First().X
                        );

                // ask the property graph what direction the stem should be
                chordlayout.StemDirection = direction;
                chordlayout.StemLinesHeight = 3F;

                chords.Add(chordlayout);
            }
        }

        void LayoutBeams() {
            var beamsequences =
                new List<ChordLayout[]>();

            foreach (var chord in chords) {
                if (chord.Duration.Length < Time.Note_4th) {
                    var cell =
                        chord.Notes[0].Core.Cell;

                    if (cell.Duration.End < cell.Duration.End) {
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
                                    .First();

                            last = bestcandidate;
                        }

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
                        if (train.Average(chordi => chordi.Notes.Average(notei => (float)notei.HalfLine)) >= staff.MiddleHalfLine)
                            stemdirection = NoteStemDirection.Down;
                        else {
                            stemdirection = NoteStemDirection.Up;
                        }

                        float stemoffset = 3;
                        if (stemdirection == NoteStemDirection.Down)
                            stemoffset *= -1;

                        for (var i = 0; i < train.Count; i++) {
                            var is1 = i == 0;
                            var is2 = i == 1;
                            var item = train[i];

                            item.StemDirection = stemdirection;
                            item.FlagSlope = m;

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

                            item.Flags = item.Length.Length - LengthClass.Quarter;

                            item.StemLinesHeight = stemoffset + item.X * m + b;
                        }
                    }
                }
            }
        }

        float ToVirtualPX(Time time) =>
            (time / Time.Note_128th_3rd_5th_7th) * 0.2F;
    }
}
