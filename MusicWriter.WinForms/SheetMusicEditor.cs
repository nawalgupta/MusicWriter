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
            const float pixelsperstaffline = 20F;
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
                var offset = note.Duration.Start - measure.Duration.Start;
                var x_width = note.Duration.Length.Notes / notefractions * pixelspernotefraction;
                var x_left = (note.Duration.Start - measure.Duration.Start).Notes / notefractions * pixelspernotefraction;


                PitchTransform transform;
                var keyclass = keysignature.KeyOfPitchClass(note.Tone.PitchClass, out transform);
                var key =
                    new Key(
                            keyclass,
                            note.Tone.Octave
                        );

                var line = Staff.GetHalfLine(key);

                var y = pixelsperstaffline * line;

                var rect =
                    new RectangleF(
                            x_left,
                            y - noteheight / 2F,
                            x_width,
                            noteheight
                        );

                if (notes_boxes.ContainsKey(note.ID))
                    notes_boxes[note.ID] = rect;
                else {
                    notes_boxes.Add(note.ID, rect);
                }


            }

            graphics.TranslateTransform(left, 0);
        }
    }
}
