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
        
        readonly Dictionary<Duration, float> minwidths =
            new Dictionary<Duration, float>();

        readonly Dictionary<MusicTrack, Dictionary<RenderedSheetMusicItem, float>> itemwidths =
            new Dictionary<MusicTrack, Dictionary<RenderedSheetMusicItem, float>>();
        
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

            MeasureAndLayoutTrackItems();

            base.OnInvalidated(e);
        }

        void MeasureAndLayoutTrackItems() {
            // we're extracting every key point in all the tracks and looking for times
            // where they share the same time. This is used to identify how much to stretch
            // each track item when its rendered.

            //TODO: handle 0 samepoints
            if (tracks.Count == 0)
                return;
            
            var samepoints =
                tracks
                    .SpecialCollection
                    .Select(track => track.Memory.Analyses<RenderedSheetMusicItem>(Duration.Eternity))
                    .Where(items => items.Any())
                    .Select(
                            items =>
                                items
                                    .Select(item => item.Duration.Start)
                                    .Concat(
                                            new Time[] {
                                                items
                                                    .Select(item => item.Duration.End)
                                                    .Max()
                                            }
                                        )
                        )
                    .Aggregate((a, b) => a.Intersect(b));

            var samedurations = new List<Duration>();

            Time oldsamepoint = Time.Zero;
            foreach (var samepoint in samepoints) {
                if (samepoint != Time.Zero)
                    samedurations.Add(new Duration { Start = oldsamepoint, End = samepoint });

                oldsamepoint = samepoint;
            }

            itemwidths.Clear();
            minwidths.Clear();

            var desireditemchunkwidths_stretchy =
                new Dictionary<MusicTrack, Dictionary<Duration, float>>();
            var desireditemchunkwidths_fixed =
                new Dictionary<MusicTrack, Dictionary<Duration, float>>();
            var desireditemchunkwidths =
                new Dictionary<MusicTrack, Dictionary<Duration, float>>();

            foreach (var track in tracks.SpecialCollection) {
                desireditemchunkwidths.Add(track, new Dictionary<Duration, float>());
                desireditemchunkwidths_stretchy.Add(track, new Dictionary<Duration, float>());
                desireditemchunkwidths_fixed.Add(track, new Dictionary<Duration, float>());
                itemwidths.Add(track, new Dictionary<RenderedSheetMusicItem, float>());
            }

            // Measure
            foreach (var duration in samedurations) {
                var minwidth = 0f;
                foreach (var track in tracks.SpecialCollection) {
                    var items =
                        track
                            .Memory
                            .Analyses<RenderedSheetMusicItem>(duration);

                    var desired = 0f;
                    var desired_stretchy = 0f;
                    var desired_fixed = 0f;

                    foreach (var item in items) {
                        var minwidth_item =
                            item.Value.MinWidth(Settings);

                        if (item.Value.Stretchy)
                            desired_stretchy += minwidth_item;
                        else desired_fixed += minwidth_item;

                        desired += minwidth_item;
                    }
                    
                    minwidth =
                        Math.Max(
                            minwidth,
                            desired
                            );

                    desireditemchunkwidths_fixed[track].Add(duration, desired_fixed);
                    desireditemchunkwidths_stretchy[track].Add(duration, desired_stretchy);
                    desireditemchunkwidths[track].Add(duration, desired);
                }

                minwidths.Add(duration, minwidth);
            }

            // Layout
            foreach (var minwidthkvp in minwidths) {
                foreach (var track in tracks.SpecialCollection) {
                    var items =
                        track
                            .Memory
                            .Analyses<RenderedSheetMusicItem>(minwidthkvp.Key);

                    var factor_stretchy_width =
                        (minwidthkvp.Value - desireditemchunkwidths_fixed[track][minwidthkvp.Key]) / desireditemchunkwidths[track][minwidthkvp.Key];

                    var expandabilitydivisor =
                        items.Count(item => item.Value.Stretchy);
                    
                    foreach (var item in items) {
                        var width =
                            item.Value.MinWidth(Settings);

                        if (item.Value.Stretchy)
                            width *= factor_stretchy_width;

                        itemwidths[track].Add(item.Value, width);
                    }
                }
            }
        }

        float GetLeft(Time time) {
            var left = 0f;

            foreach (var chunk in minwidths) {
                if (chunk.Key.End < time)
                    left += chunk.Value;
                else {
                    left += Time.FloatDiv(time - chunk.Key.Start, chunk.Key.Length) * chunk.Value;

                    break;
                }
            }

            return left;
        }

        int timesRedrawn = 0;
        protected override void OnPaint(PaintEventArgs pe) {
            timesRedrawn++;
            pe.Graphics.DrawString(timesRedrawn.ToString(), Font, Brushes.Red, PointF.Empty);

            var scrollX = GetLeft(Pin.Time.Offset.Value);

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

                    float x = GetLeft(focusstarttime) - scrollX;

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

                        var width = itemwidths[track][item];
                        if (x + width > 0)
                            pe.Graphics.DrawImageUnscaled(item.Draw(Settings, (int)width), (int)x, 0);

                        x += width;
                    }
                }

                pe.Graphics.TranslateTransform(0, Settings.Height);
            }

            pe.Graphics.ResetTransform();

            var caretx = GetLeft(Caret.Caret.Focus) - scrollX;
            pe.Graphics.DrawLine(Pens.Black,
                    caretx,
                    0,
                    caretx,
                    Height
                );

            base.OnPaint(pe);
        }

    }
}
