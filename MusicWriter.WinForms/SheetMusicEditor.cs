using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms {
    public partial class SheetMusicEditor :
        Control,
        ITrackController<Control> {
        MusicBrain brain;

        public float ScrollX { get; set; } = 0;
        public MusicBrain Brain {
            get { return brain; }
            set {
                if(brain != null) {
                    brain.RemoveCog<RenderedSheetMusicItemPerceptualCog>();
                }

                brain = value;

                brain.InsertCog(new RenderedSheetMusicItemPerceptualCog());
            }
        }

        public SheetMusicRenderSettings Settings { get; set; } = new SheetMusicRenderSettings();
        ObservableProperty<string> ITrackController<Control>.Name { get; } =
            new ObservableProperty<string>("abc");

        public CaretController CaretController {
            get { return caretcontroller; }
            set {
                if (caretcontroller != null) {
                    caretcontroller.PreviewTimeChanged -= Caretcontroller_PreviewTimeChanged;
                    caretcontroller.PreviewToneChanged -= Caretcontroller_PreviewToneChanged;
                    caretcontroller.TimeChanged -= Caretcontroller_TimeChanged;
                    caretcontroller.ToneChanged -= Caretcontroller_ToneChanged;
                    caretcontroller.TimeReset -= Caretcontroller_TimeReset;
                    caretcontroller.ToneReset -= Caretcontroller_ToneReset;
                    caretcontroller.TimeStart -= Caretcontroller_TimeStart;
                    caretcontroller.ToneStart -= Caretcontroller_ToneStart;
                }

                caretcontroller = value;

                caretcontroller.PreviewTimeChanged += Caretcontroller_PreviewTimeChanged;
                caretcontroller.PreviewToneChanged += Caretcontroller_PreviewToneChanged;
                caretcontroller.TimeChanged += Caretcontroller_TimeChanged;
                caretcontroller.ToneChanged += Caretcontroller_ToneChanged;
                caretcontroller.TimeReset += Caretcontroller_TimeReset;
                caretcontroller.ToneReset += Caretcontroller_ToneReset;
                caretcontroller.TimeStart += Caretcontroller_TimeStart;
                caretcontroller.ToneStart += Caretcontroller_ToneStart;
            }
        }

        CaretController caretcontroller;

        readonly ConverterList<ITrack, MusicTrack> tracks =
            new ConverterList<ITrack, MusicTrack>();

        readonly Dictionary<KeyValuePair<NoteID, MusicTrack>, Time> selectednotes_start = new Dictionary<KeyValuePair<NoteID, MusicTrack>, Time>();
        readonly Dictionary<KeyValuePair<NoteID, MusicTrack>, Time> selectednotes_end = new Dictionary<KeyValuePair<NoteID, MusicTrack>, Time>();
        readonly Dictionary<KeyValuePair<NoteID, MusicTrack>, SemiTone> selectednotes_tone = new Dictionary<KeyValuePair<NoteID, MusicTrack>, SemiTone>();

        public IList<ITrack> Tracks {
            get { return tracks.RegularCollection; }
        }

        public Control View {
            get { return this; }
        }

        public void LoadFrom(EditorFile file, string track) {
            Settings = new SheetMusicRenderSettings();
            
            Invalidate();
        }
        
        public SheetMusicEditor() {
            InitializeComponent();

            tracks.SpecialCollection.CollectionChanged += Tracks_CollectionChanged;
        }
        
        private void Caretcontroller_ToneChanged(int tone, CaretMode mode) {
            foreach (var selected in selectednotes_tone) {
                var noteID = selected.Key.Key;
                var track = selected.Key.Value;
                var note = track.Melody[noteID];

                var newtone =
                    note.Tone;

                if (mode == CaretMode.Absolute)
                    newtone = new SemiTone(tone);
                else if (mode == CaretMode.Delta)
                    newtone += new SemiTone(tone);

                track.Melody.UpdateNote(note.ID, note.Duration, newtone);
            }
        }

        private void Caretcontroller_TimeChanged(Time time, CaretMode mode) {
            foreach (var selected in selectednotes_start.Concat(selectednotes_end)) {
                var is_start = selectednotes_start.ContainsKey(selected.Key);
                var is_end = selectednotes_end.ContainsKey(selected.Key);

                var noteID = selected.Key.Key;
                var track = selected.Key.Value;
                var note = track.Melody[noteID];
                
                var newduration =
                    note.Duration;

                if (is_start) {
                    if (mode == CaretMode.Absolute)
                        newduration.Start = time;
                    else if (mode == CaretMode.Delta)
                        newduration.Start += time;
                }

                if (is_end) {
                    if (mode == CaretMode.Absolute)
                        newduration.End = time;
                    else if (mode == CaretMode.Delta)
                        newduration.End += time;
                }

                track.Melody.UpdateNote(note.ID, newduration, note.Tone);
            }
        }

        private void Caretcontroller_PreviewToneChanged(int tone, CaretMode mode) =>
            Caretcontroller_PreviewToneChanged(tone, mode);

        private void Caretcontroller_PreviewTimeChanged(Time time, CaretMode mode) =>
            Caretcontroller_PreviewTimeChanged(time, mode);

        private void Caretcontroller_TimeStart() {
            foreach (var note_ref in selectednotes_start.Keys)
                selectednotes_start[note_ref] = note_ref.Value.Melody[note_ref.Key].Duration.Start;
            
            foreach (var note_ref in selectednotes_end.Keys)
                selectednotes_end[note_ref] = note_ref.Value.Melody[note_ref.Key].Duration.End;
        }

        private void Caretcontroller_ToneStart() {
            foreach (var note_ref in selectednotes_tone.Keys)
                selectednotes_tone[note_ref] = note_ref.Value.Melody[note_ref.Key].Tone;
        }

        private void Caretcontroller_TimeReset() {
            foreach (var note_ref in selectednotes_start.Keys.Concat(selectednotes_end.Keys)) {
                var is_start = selectednotes_start.ContainsKey(note_ref);
                var is_end = selectednotes_end.ContainsKey(note_ref);

                var note =
                    note_ref.Value.Melody[note_ref.Key];

                var new_start =
                    is_start ?
                        selectednotes_start[note_ref] :
                        note.Duration.Start;

                var new_end =
                    is_start ?
                        selectednotes_start[note_ref] :
                        note.Duration.Start;

                var newduration =
                    new Duration {
                        Start = new_start,
                        End = new_end
                    };

                note_ref.Value.Melody.UpdateNote(note_ref.Key, newduration, note.Tone);
            }
        }

        private void Caretcontroller_ToneReset() {
            foreach (var note_ref in selectednotes_tone.Keys) {
                var note =
                    note_ref.Value.Melody[note_ref.Key];

                var newtone =
                    selectednotes_tone[note_ref];

                note_ref.Value.Melody.UpdateNote(note_ref.Key, note.Duration, newtone);
            }
        }

        private void Tracks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.OldItems != null)
                foreach (MusicTrack track in e.OldItems)
                    track.Memory.RemoveMemoryModule<RenderedSheetMusicItem>();

            if (e.NewItems != null)
                foreach (MusicTrack track in e.NewItems)
                    track.Memory.InsertMemoryModule(new RenderedSheetMusicItemPerceptualCog.MemoryModule());
            
            Invalidate();
        }

        protected override void OnInvalidated(InvalidateEventArgs e) {
            Height = (int)(Settings.Height * tracks.Count);

            foreach (var track in tracks.SpecialCollection)
                brain.Invalidate(track.Memory, Duration.Eternity);

            base.OnInvalidated(e);
        }

        protected override void OnPaint(PaintEventArgs pe) {
            foreach (var track in tracks.SpecialCollection) {
                // draw staff
                if (Settings?.Staff != null)
                    for (int line = 0; line < Settings.Staff.Lines; line++)
                        pe.Graphics.DrawLine(
                                Pens.Black,
                                0,
                                Settings.YVal(line * 2),
                                Width,
                                Settings.YVal(line * 2)
                            );

                // draw sheet items
                var rendered = new List<RenderedSheetMusicItem>();

                var starttimes =
                    track
                        .Memory
                        .Analyses<RenderedSheetMusicItem>(Duration.Eternity)
                        .Select(item => item.Duration)
                        .Select(duration => duration.Start)
                        .Distinct()
                        .ToList();

                starttimes.Sort();

                if (starttimes.Count != 0) {
                    var focusstarttime =
                        starttimes[0];

                    var focusitems = new List<RenderedSheetMusicItem>();

                    var x = -ScrollX;

                    while (x < Width) {
                        if (focusitems.Count == 0) {
                            if (starttimes.Count == 0)
                                break;

                            focusstarttime = starttimes[0];
                            starttimes.RemoveAt(0);

                            focusitems
                                .AddRange(
                                        track
                                            .Memory
                                            .Analyses<RenderedSheetMusicItem>(focusstarttime)
                                            .Select(focusitem => focusitem.Value)
                                    );

                            focusitems.Sort((a, b) => a.Priority.CompareTo(b.Priority));
                        }

                        var item = focusitems[0];
                        focusitems.RemoveAt(0);

                        var width = item.Width(Settings);
                        if (x + width > 0)
                            pe.Graphics.DrawImageUnscaled(item.Draw(Settings), (int)x, 0);

                        x += width;
                    }
                }

                pe.Graphics.TranslateTransform(0, Settings.Height);
            }

            base.OnPaint(pe);
        }

    }
}
