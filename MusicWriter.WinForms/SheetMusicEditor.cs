using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms {
    public partial class SheetMusicEditor : Control {
        public MelodyTrack Track { get; set; }
        public Time ScrollStart { get; set; } = Time.Zero;
        public NoteCaret Caret { get; } = new NoteCaret();
        public Staff Staff { get; set; } = Staff.Treble;

        readonly Dictionary<NoteID, RectangleF> notes_boxes = new Dictionary<NoteID, RectangleF>();

        public SheetMusicEditor() {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe) {
            var measure =
                Track.Rhythm.MeasureAt(ScrollStart);

            base.OnPaint(pe);
        }

        bool DrawMeasure(
                Measure measure,
                Graphics graphics,
                int left,
                out int right
            ) {
            graphics.TranslateTransform(left, 0);

            var notes =
                Track.NotesInTime(measure.Duration).ToList();
            
            notes.Sort(new NoteComparerByOffset());

            var notefractions =
                (int)Statistics.AvgMin(notes.Select(note => (float)Time.Note.Ticks / note.Duration.Length.Ticks));

            const float pixelspernotefraction = 40F;
            const float pixelsperstaffhalfline = 20F;
            const float noteheight = 20f;

            var tuplets_info =
                new Dictionary<int, List<Duration>>();

            var keysignature =
                Track
                    .KeySignatures
                    .Intersecting(measure.Duration)
                    .Single();

            for (int i = 0; i < notes.Count; i++) {
                var note = notes[i];

                var rect_x =
                    (note.Duration.Start - measure.Duration.Start).Notes
                    / notefractions 
                    * pixelspernotefraction;
                var rect_w =
                    note.Duration.Length.Notes
                    / notefractions
                    * pixelspernotefraction;

                PitchTransform transform;
                var keyclass = keysignature.KeyOfPitchClass(note.Tone.PitchClass, out transform);
                var key =
                    new Key(
                            keyclass,
                            note.Tone.Octave
                        );

                var line = Staff.GetHalfLine(key);

                var y = pixelsperstaffhalfline * line;
                var y_dots = pixelsperstaffhalfline * 2 * (float)Math.Ceiling(line / 2.0);

                var stemdirection =
                    Staff.GetStemDirection(key);

                var rect =
                    new RectangleF(
                            rect_x,
                            y - noteheight / 2F,
                            rect_w,
                            noteheight
                        );

                if (notes_boxes.ContainsKey(note.ID))
                    notes_boxes[note.ID] = rect;
                else {
                    notes_boxes.Add(note.ID, rect);
                }

                var lastcut_x = -1F;

                foreach (var cutduration in NoteDuration.Decompose(note.Duration)) {
                    var offset = cutduration.Duration.Start - measure.Duration.Start;
                    var x_width = cutduration.Duration.Length.Notes / notefractions * pixelspernotefraction;
                    var x_left = offset.Notes / notefractions * pixelspernotefraction;

                    bool note_filled = false;
                    bool note_stem = false;
                    int note_flags = 0;

                    if (cutduration.Length > NoteDuration.LengthClass.Half)
                        note_filled = true;

                    if (cutduration.Length > NoteDuration.LengthClass.Whole)
                        note_stem = true;

                    if (cutduration.Length > NoteDuration.LengthClass.Quarter)
                        note_flags = cutduration.Length - NoteDuration.LengthClass.Quarter;

                    DrawNoteHead(graphics, x_left, y, y_dots, note_filled, cutduration.Dots);

                    if (note_stem) {
                        DrawNoteStem(graphics, x_left, y, stemdirection);

                        if (cutduration.Duration.Start > cutduration.Cell.Duration.Start) {
                            var cell_lastnote =

                        }
                    }
                    
                    if (lastcut_x != -1F) {
                        // tie this note to last
                        DrawTie(graphics, lastcut_x, x_left, y);
                    }

                    lastcut_x = x_left;
                }
            }

            graphics.TranslateTransform(left, 0);
        }

        struct NoteDuration {
            internal enum LengthClass : int {
                Whole = 1,
                Half,
                Quarter,
                Eighth,
                Sixteenth,
                ThirtySecond,
                SixtyFourth,
                OneHundredTwentyEighth
            }

            internal enum TupletClass : int {
                Half = 0x01,
                Triplet = 0x02,
                Pentuplet = 0x04,
                Septuplet = 0x08,
            }

            public LengthClass Length;
            public TupletClass Tuplet;
            public Duration Duration;
            public int Dots;
            public Cell Cell;

            private NoteDuration(
                    LengthClass length,
                    TupletClass tuplet,
                    Duration duration,
                    int dots,
                    Cell cell
                ) {
                Length = length;
                Tuplet = tuplet;
                Duration = duration;
                Dots = dots;
                Cell = cell;
            }

            public static IEnumerable<NoteDuration> Decompose(Duration duration) {
                var length = duration.Length;
                int notes_128th = length / Time.Note128th_3rd_5th_7th;

                //NOTE: this code is highly dependent on the Time.cs class structure

                TupletClass tuplet = TupletClass.Half;

                // an Nth note can't be divided further into N pieces

                if (!length.Third.HasValue)
                    tuplet |= TupletClass.Triplet;
                else {
                    notes_128th /= 3;
                }

                if (!length.Fifth.HasValue)
                    tuplet |= TupletClass.Pentuplet;
                else {
                    notes_128th /= 5;
                }

                if (!length.Seventh.HasValue)
                    tuplet |= TupletClass.Septuplet;
                else {
                    notes_128th /= 7;
                }

                bool length_128th = (notes_128th & 0x01) != 0;
                bool length_64th = (notes_128th & 0x02) != 0;
                bool length_32nd = (notes_128th & 0x04) != 0;
                bool length_16th = (notes_128th & 0x08) != 0;
                bool length_8th = (notes_128th & 0x10) != 0;
                bool length_4th = (notes_128th & 0x20) != 0;
                bool length_2nd = (notes_128th & 0x40) != 0;
                bool length_1st = (notes_128th & 0x80) != 0;

                // if pattern ..01..10.. is present, then we can use dotted notes

            }
        }

        void DrawNoteFlag_free(Graphics gfx, float x, float y, int count) {
            for (int i = count - 1; i >= 0; i--) {
                gfx.DrawLine(Pens.Black, x, y, x + 6, y - 10);

                y -= 8;
            }
        }

        void DrawNoteFlag_start(Graphics gfx, float x, float y, int count, float width) {
            for (int i = count - 1; i >= 0; i--) {
                gfx.DrawLine(Pens.Black, x, y, x + width - 8, y);

                y -= 8;
            }
        }
        
        void DrawNoteFlag_end(Graphics gfx, float x, float y, int count, float width) {
            for (int i = count - 1; i >= 0; i--) {
                gfx.DrawLine(Pens.Black, x, y, x - 8, y);

                y -= 8;
            }
        }

        void DrawNoteHead(Graphics gfx, float x, float y, float y_dots, bool fill, int dots) {
            gfx.DrawEllipse(Pens.Black, x - 8, y - 4, 8, 8);

            if (fill)
                gfx.FillEllipse(Brushes.Black, x - 8, y - 4, 8, 8);

            for (int i = 0; i < dots; i++)
                gfx.FillEllipse(Brushes.Black, x + i * 3.5F + 1F, y_dots - 1.2F, 2.4F, 2.4F);
        }

        void DrawNoteStem(Graphics gfx, float x, float y, NoteStemDirection direction) {
            switch (direction) {
                case NoteStemDirection.Down:
                    gfx.DrawLine(Pens.Black, x, y, x, y - 20F);
                    break;

                case NoteStemDirection.Up:
                    gfx.DrawLine(Pens.Black, x, y, x, y + 20F);
                    break;
            }
        }

        void DrawTie(Graphics gfx, float x0, float x1, float y) {

        }
    }
}
