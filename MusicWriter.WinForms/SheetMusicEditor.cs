using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms {
    public partial class SheetMusicEditor :
        Control,
        ITrackController<Control> {
        EditorFile file;

        public Pin Pin { get; } = new Pin();

        public ToneCaret Caret { get; } = new ToneCaret();

        public EditorFile File {
            get { return file; }
            set {
                if (file != null) {
                    file.Brain.RemoveCog<RenderedSheetMusicItemPerceptualCog>();
                }

                file = value;
                file.Brain.InsertCog(new RenderedSheetMusicItemPerceptualCog());
            }
        }

        public SheetMusicRenderSettings Settings { get; set; } = new SheetMusicRenderSettings();
        ObservableProperty<string> ITrackController<Control>.Name { get; } =
            new ObservableProperty<string>("abc");

        public InputController InputController {
            get { return inputcontroller; }
            set {
                if (inputcontroller != null) {
                    inputcontroller.PreviewTimeChanged -= InputController_PreviewTimeChanged;
                    inputcontroller.PreviewToneChanged -= InputController_PreviewToneChanged;
                    inputcontroller.TimeChanged -= InputController_TimeChanged;
                    inputcontroller.ToneChanged -= InputController_ToneChanged;
                    inputcontroller.TimeReset -= InputController_TimeReset;
                    inputcontroller.ToneReset -= InputController_ToneReset;
                    inputcontroller.TimeStart -= InputController_TimeStart;
                    inputcontroller.ToneStart -= InputController_ToneStart;
                }

                inputcontroller = value;

                inputcontroller.PreviewTimeChanged += InputController_PreviewTimeChanged;
                inputcontroller.PreviewToneChanged += InputController_PreviewToneChanged;
                inputcontroller.TimeChanged += InputController_TimeChanged;
                inputcontroller.ToneChanged += InputController_ToneChanged;
                inputcontroller.TimeReset += InputController_TimeReset;
                inputcontroller.ToneReset += InputController_ToneReset;
                inputcontroller.TimeStart += InputController_TimeStart;
                inputcontroller.ToneStart += InputController_ToneStart;
                inputcontroller.NotePlacementStart += InputController_NotePlacementStart;
                inputcontroller.NotePlacementFinish += InputController_NotePlacementFinish;
                inputcontroller.SelectionStart += InputController_SelectionStart;
                inputcontroller.SelectionFinish += InputController_SelectionFinish;
            }
        }

        InputController inputcontroller;

        readonly ConverterList<ITrack, MusicTrack> tracks =
            new ConverterList<ITrack, MusicTrack>();

        readonly Dictionary<KeyValuePair<NoteID, MusicTrack>, Time> selectednotes_start = new Dictionary<KeyValuePair<NoteID, MusicTrack>, Time>();
        readonly Dictionary<KeyValuePair<NoteID, MusicTrack>, Time> selectednotes_end = new Dictionary<KeyValuePair<NoteID, MusicTrack>, Time>();
        readonly Dictionary<KeyValuePair<NoteID, MusicTrack>, SemiTone> selectednotes_tone = new Dictionary<KeyValuePair<NoteID, MusicTrack>, SemiTone>();

        int activetrack_index = 0;

        public MusicTrack ActiveTrack {
            get { return tracks.SpecialCollection[activetrack_index]; }
        }

        public IList<ITrack> Tracks {
            get { return tracks.RegularCollection; }
        }

        public Control View {
            get { return this; }
        }
        
        public SheetMusicEditor() {
            InitializeComponent();

            tracks.SpecialCollection.CollectionChanged += Tracks_CollectionChanged;
        }

        private void InputController_ToneChanged(int tone, CaretMode mode) {
            // action was already handled by preview
        }

        private void InputController_TimeChanged(Time time, CaretMode mode) {
            // action was already handled by preview
        }

        private void InputController_PreviewToneChanged(int tone, CaretMode mode) =>
            Effect_ToneChanged(tone, mode);

        private void InputController_PreviewTimeChanged(Time time, CaretMode mode) =>
            Effect_TimeChanged(time, mode);

        private void InputController_TimeStart() {
            foreach (var note_ref in selectednotes_start.Keys.ToArray())
                selectednotes_start[note_ref] = note_ref.Value.Melody[note_ref.Key].Duration.Start;
            
            foreach (var note_ref in selectednotes_end.Keys.ToArray())
                selectednotes_end[note_ref] = note_ref.Value.Melody[note_ref.Key].Duration.End;
        }

        private void InputController_ToneStart() {
            foreach (var note_ref in selectednotes_tone.Keys.ToArray())
                selectednotes_tone[note_ref] = note_ref.Value.Melody[note_ref.Key].Tone;
        }

        private void InputController_TimeReset() {
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

        private void InputController_ToneReset() {
            foreach (var note_ref in selectednotes_tone.Keys) {
                var note =
                    note_ref.Value.Melody[note_ref.Key];

                var newtone =
                    selectednotes_tone[note_ref];

                note_ref.Value.Melody.UpdateNote(note_ref.Key, note.Duration, newtone);
            }
        }

        private void InputController_SelectionFinish() {
            Caret.Caret.Side = MusicWriter.Caret.FocusSide.Both;
        }

        private void InputController_SelectionStart() {
            Caret.Caret.Side = MusicWriter.Caret.FocusSide.Right;
        }

        private void InputController_NotePlacementFinish() {
        }

        private void InputController_NotePlacementStart() {
            var tone =
                //SemiTone.C4;
                Caret.Tone;

            var duration =
                //new Duration {
                //    Start = Time.Note_8th,
                //    Length = Time.Note_4th
                //};
                Caret.Caret.Duration;

            var note =
                ActiveTrack.Melody.AddNote(tone, duration);

            var kvp =
                new KeyValuePair<NoteID, MusicTrack>(note.ID, ActiveTrack);

            selectednotes_end.Add(kvp, note.Duration.End);
            selectednotes_tone.Add(kvp, note.Tone);
            
            Invalidate();
        }

        void Effect_TimeChanged(Time time, CaretMode mode) {
            foreach (var selected in selectednotes_start.Concat(selectednotes_end)) {
                var is_start = selectednotes_start.ContainsKey(selected.Key);
                var is_end = selectednotes_end.ContainsKey(selected.Key);

                var noteID = selected.Key.Key;
                var track = selected.Key.Value;
                var note = track.Melody[noteID];

                var oldduration =
                    note.Duration;

                var newduration =
                    new Duration {
                        Start = oldduration.Start,
                        Length = oldduration.Length
                    };

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

            if (mode == CaretMode.Absolute)
                Caret.Caret.Focus = time;
            else if (mode == CaretMode.Delta)
                Caret.Caret.Focus += time;

            Invalidate();
        }

        void Effect_ToneChanged(int tone, CaretMode mode) {
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

            if (mode == CaretMode.Absolute)
                Caret.Tone = new SemiTone(tone);
            else if (mode == CaretMode.Delta)
                Caret.Tone += new SemiTone(tone);
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
                file.Brain.Invalidate(track.Memory, Duration.Eternity);

            base.OnInvalidated(e);
        }

        int timesRedrawn = 0;
        protected override void OnPaint(PaintEventArgs pe) {
            timesRedrawn++;
            pe.Graphics.DrawString(timesRedrawn.ToString(), Font, Brushes.Red, PointF.Empty);

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

                    float x = 0; //TODO: make it scroll with the pin

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
