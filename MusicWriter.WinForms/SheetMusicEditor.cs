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

        public Cursor MusicCursor { get; } = new Cursor();
        Cursor MusicCursor_bak = new Cursor();

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

        public SheetMusicRenderSettings TemplateSettings { get; set; } = new SheetMusicRenderSettings();
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

        readonly Dictionary<MusicTrack, NoteSelection> noteselections =
            new Dictionary<MusicTrack, NoteSelection>();

        readonly Dictionary<Duration, float> minwidths =
            new Dictionary<Duration, float>();

        readonly Dictionary<MusicTrack, Dictionary<RenderedSheetMusicItem, float>> itemwidths =
            new Dictionary<MusicTrack, Dictionary<RenderedSheetMusicItem, float>>();

        readonly Dictionary<MusicTrack, float> trackheights =
            new Dictionary<MusicTrack, float>();

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
            foreach (var selection in noteselections)
                selection.Value.Save_Time(selection.Key);

            MusicCursor_bak.Caret.Duration.Start = MusicCursor.Caret.Duration.Start;
            MusicCursor_bak.Caret.Duration.Length = MusicCursor.Caret.Duration.Length;
            MusicCursor_bak.Caret.Side = MusicCursor.Caret.Side;
        }

        private void InputController_ToneStart() {
            foreach (var selection in noteselections)
                selection.Value.Save_Tone(selection.Key);

            MusicCursor_bak.Tone = MusicCursor.Tone;
        }

        private void InputController_TimeReset() {
            foreach (var selection in noteselections)
                selection.Value.Restore_Time(selection.Key);

            MusicCursor.Caret.Duration.Start = MusicCursor_bak.Caret.Duration.Start;
            MusicCursor.Caret.Duration.Length = MusicCursor_bak.Caret.Duration.Length;
            MusicCursor.Caret.Side = MusicCursor_bak.Caret.Side;

            Invalidate();
        }

        private void InputController_ToneReset() {
            foreach (var selection in noteselections)
                selection.Value.Restore_Tone(selection.Key);

            MusicCursor.Tone = MusicCursor_bak.Tone;

            Invalidate();
        }

        private void InputController_SelectionFinish() {
            MusicCursor.Caret.Side = Caret.FocusSide.Both;
        }

        private void InputController_SelectionStart() {
            MusicCursor.Caret.Side = Caret.FocusSide.Right;
        }

        private void InputController_NotePlacementFinish() {
        }

        private void InputController_NotePlacementStart() {
            var tone =
                MusicCursor.Tone;

            var duration =
                MusicCursor.Caret.Duration;

            var note =
                ActiveTrack.Melody.AddNote(tone, duration);
            
            noteselections[ActiveTrack].Selected_End.Add(note.ID);
            noteselections[ActiveTrack].Selected_Tone.Add(note.ID);
            
            Invalidate();
        }

        void Effect_TimeChanged(Time time, CaretMode mode) {
            foreach (var trackkvp in noteselections) {
                var track = trackkvp.Key;

                foreach (var noteID in trackkvp.Value.Selected_Start.Concat(trackkvp.Value.Selected_End)) {
                    var is_start = trackkvp.Value.Selected_Start.Contains(noteID);
                    var is_end = trackkvp.Value.Selected_End.Contains(noteID);

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
            }

            if (mode == CaretMode.Absolute)
                MusicCursor.Caret.Focus = time;
            else if (mode == CaretMode.Delta)
                MusicCursor.Caret.Focus += time;

            if (time != Time.Zero || mode == CaretMode.Absolute)
                Invalidate();
        }

        void Effect_ToneChanged(int tone, CaretMode mode) {
            foreach (var trackkvp in noteselections) {
                var track = trackkvp.Key;

                foreach (var noteID in trackkvp.Value.Selected_Tone) {
                    var note = track.Melody[noteID];

                    var newtone =
                        Effect_ToneChanged_affect(note.Tone, note.Duration.Start, tone, mode, track);

                    track.Melody.UpdateNote(note.ID, note.Duration, newtone);
                }
            }

            MusicCursor.Tone = Effect_ToneChanged_affect(MusicCursor.Tone, MusicCursor.Caret.Focus, tone, mode, ActiveTrack);

            if (tone != 0 || mode.HasFlag(CaretMode.Absolute))
                Invalidate();
        }

        SemiTone Effect_ToneChanged_affect(SemiTone tone, Time time, int delta, CaretMode mode, MusicTrack track) {
            if (mode.HasFlag(CaretMode.Absolute))
                return new SemiTone(delta);
            else if (!mode.HasFlag(CaretMode.Delta))
                throw new InvalidOperationException();

            if (mode.HasFlag(CaretMode.SemiTones))
                tone += delta;
            else if (mode.HasFlag(CaretMode.WholeTones)) {
                var keysig =
                    track
                        .Adornment
                        .KeySignatures
                        .Intersecting(time)
                        .First()
                        .Value;

                if (delta > 0) {
                    while (delta-- > 0)
                        tone = keysig.Right(tone);
                }
                else if (delta < 0) {
                    while (delta++ < 0)
                        tone = keysig.Left(tone);
                }
            }
            else throw new InvalidOperationException();

            return tone;
        }

        private void Tracks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.OldItems != null)
                foreach (MusicTrack track in e.OldItems) {
                    track.Memory.RemoveMemoryModule<RenderedSheetMusicItem>();
                    noteselections.Remove(track);
                }

            if (e.NewItems != null)
                foreach (MusicTrack track in e.NewItems) {
                    track.Memory.InsertMemoryModule(new RenderedSheetMusicItemPerceptualCog.MemoryModule());
                    noteselections.Add(track, new NoteSelection());
                }

            Invalidate();
        }

        protected override void OnInvalidated(InvalidateEventArgs e) {
            foreach (var track in tracks.SpecialCollection)
                file.Brain.Invalidate(track.Memory, Duration.Eternity);

            MeasureAndLayoutTrackItems();

            Height = (int)trackheights.Values.Sum();

            base.OnInvalidated(e);
        }

        void MeasureAndLayoutTrackItems() {
            // we're extracting every key point in all the tracks and looking for times
            // where they share the same time. This is used to identify how much to stretch
            // each track item when its rendered.

            //TODO: handle 0 samepoints
            if (tracks.Count == 0)
                return;

            trackheights.Clear();
            foreach (var track in tracks.SpecialCollection)
                trackheights.Add(track, 0);
            
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
                            .Analyses<RenderedSheetMusicItem>(duration)
                            .ToArray();

                    var desired = 0f;
                    var desired_stretchy = 0f;
                    var desired_fixed = 0f;

                    foreach (var item in items) {
                        var item_settings =
                            GetSettings(item.Duration.Start, track);

                        if (trackheights[track] < item_settings.Height)
                            trackheights[track] = item_settings.Height;

                        var minwidth_item =
                            item.Value.MinWidth(item_settings);

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
                            item.Value.MinWidth(GetSettings(item.Duration.Start, track));

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

        float GetLeft(Time time, MusicTrack track) {
            var left = 0f;

            foreach(var item in track.Memory.Analyses<RenderedSheetMusicItem>(new Duration { End = time })) {
                if (item.Duration.End < time)
                    left += itemwidths[track][item.Value];
                else {
                    var samepoints =
                        track
                            .Memory
                            .Analyses<RenderedSheetMusicItem>(item.Duration);

                    var localtime = time - item.Duration.Start;
                    var settings =
                        GetSettings(item.Duration.Start, track);

                    foreach (var samepoint in samepoints) {
                        //TODO: enforce all have same duration
                        var itemwidth = itemwidths[track][samepoint.Value];

                        var px = samepoint.Value.PixelAtTime(localtime, itemwidth, settings);

                        if (float.IsNaN(px))
                            left += itemwidth;
                        else {
                            left += px;
                            break;
                        }
                    }
                }
            }

            return left;
        }

        SheetMusicRenderSettings GetSettings(Time time, MusicTrack track) =>
           new SheetMusicRenderSettings {
               DotInitialSpacing = TemplateSettings.DotInitialSpacing,
               DotRadius = TemplateSettings.DotRadius,
               DotSpacing = TemplateSettings.DotSpacing,
               LinesBetweenFlags = TemplateSettings.LinesBetweenFlags,
               MarginalBottomHalfLines = TemplateSettings.MarginalBottomHalfLines,
               MarginalTopHalfLines = TemplateSettings.MarginalTopHalfLines,
               NoteHeadRadius = TemplateSettings.NoteHeadRadius,
               PixelsPerHalfLine = TemplateSettings.PixelsPerHalfLine,
               PixelsPerLine = TemplateSettings.PixelsPerLine,
               PixelsPerX = TemplateSettings.PixelsPerX,
               PixelsScale = TemplateSettings.PixelsScale,
               Staff = track.Adornment.Staffs.Intersecting(time).First().Value,
               TimeSignatureFont = TemplateSettings.TimeSignatureFont,
               StaffLinePen = TemplateSettings.StaffLinePen,
               LedgerPixelWidth = TemplateSettings.LedgerPixelWidth,
               Selection = GetSelection(track),
               ThumbMarginX = TemplateSettings.ThumbMarginX,
               ThumbMarginY = TemplateSettings.ThumbMarginY,
               ThumbPadding = TemplateSettings.ThumbPadding,
               ThumbWidth = TemplateSettings.ThumbWidth
           };

        NoteSelection GetSelection(MusicTrack track) =>
            noteselections[track];

        int timesRedrawn = 0;
        protected override void OnPaint(PaintEventArgs pe) {
            timesRedrawn++;
            pe.Graphics.DrawString(timesRedrawn.ToString(), Font, Brushes.Red, PointF.Empty);

            var scrollX = GetLeft(Pin.Time.Offset.Value);

            foreach (var track in tracks.SpecialCollection) {
                var active =
                    ReferenceEquals(track, ActiveTrack);
                
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

                SheetMusicRenderSettings focussettings = null;

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

                            focussettings = GetSettings(focusstarttime, track);
                            
                            // draw staff
                            for (int line = 0; line < focussettings.Staff.Lines; line++)
                                pe.Graphics.DrawLine(
                                        focussettings.StaffLinePen,
                                        x,
                                        focussettings.YVal(line * 2),
                                        x + focusitems.Sum(focusitem => itemwidths[track][focusitem]),
                                        focussettings.YVal(line * 2)
                                    );
                        }

                        var item = focusitems[0];
                        focusitems.RemoveAt(0);
                        
                        var width = itemwidths[track][item];
                        if (x + width > 0)
                            pe.Graphics.DrawImageUnscaled(item.Draw(focussettings, (int)width), (int)x, 0);

                        x += width;
                    }
                }

                DrawCaret(pe.Graphics, GetSettings(MusicCursor.Caret.Focus, track), active, track, scrollX);

                if (trackheights.ContainsKey(track))
                    pe.Graphics.TranslateTransform(0, trackheights[track]);
            }

            base.OnPaint(pe);
        }

        void DrawCaret(
                Graphics gfx,
                SheetMusicRenderSettings settings,
                bool active,
                MusicTrack track,
                float scrollX
            ) {
            var caretx =
                GetLeft(MusicCursor.Caret.Focus, track) - scrollX;

            var caretunitx =
                GetLeft(MusicCursor.Caret.Focus + inputcontroller.UnitTime, track) - scrollX;

            var caretstaff =
                track
                    .Adornment
                    .Staffs
                    .Intersecting(MusicCursor.Caret.Focus)
                    .First()
                    .Value;

            PitchTransform transform;

            var caretkey =
                track
                    .Adornment
                    .KeySignatures
                    .Intersecting(MusicCursor.Caret.Focus)
                    .First()
                    .Value
                    .Key(MusicCursor.Tone, out transform);

            var carety =
                settings.YVal(caretstaff.GetHalfLine(caretkey));

            var caret_pen_x =
                new Pen(Color.DarkSeaGreen, active ? 2.5f : 1.2f);

            var caret_pen_y =
                new Pen(Color.Red, active ? 3f : 1.4f);

            gfx
                .DrawLine(
                        caret_pen_x,
                        caretx,
                        0,
                        caretx,
                        settings.Height
                    );

            gfx
                .DrawLine(
                        caret_pen_y,
                        caretx,
                        carety,
                        caretunitx,
                        carety
                    );

            var cursorcolor =
                active ?
                    Color.FromArgb(200, Color.DeepSkyBlue) :
                    Color.FromArgb(100, Color.Aquamarine);

            var cursor_focusduration =
                new Duration {
                    Start = MusicCursor.Caret.Focus,
                    Length = InputController.UnitTime
                };

            var cursor_notelayout =
                new NoteLayout(
                        new PerceptualNote(
                                default(PerceptualNoteID),
                                cursor_focusduration,
                                PerceptualTime.Decompose(InputController.UnitTime).First().Key,
                                track.Rhythm.Intersecting(cursor_focusduration).First(),
                                new Note(
                                        default(NoteID),
                                        cursor_focusduration,
                                        MusicCursor.Tone
                                    )
                            ),
                        settings
                            .Staff
                            .GetHalfLine(caretkey),
                        0,
                        0,
                        caretkey,
                        transform
                    );

            var cursor_chordlayout =
                new ChordLayout(cursor_notelayout);

            if (cursor_chordlayout.Length.Length > LengthClass.Whole) {
                cursor_chordlayout.StemDirection =
                    settings.Staff.GetStemDirection(cursor_notelayout.Key);

                cursor_chordlayout.StemSide =
                    cursor_chordlayout.StemDirection == NoteStemDirection.Down ?
                        NoteStemSide.Left :
                        NoteStemSide.Right;

                cursor_chordlayout.StemStartHalfLines =
                    cursor_chordlayout.StemDirection == NoteStemDirection.Down ?
                        cursor_notelayout.HalfLine - 5 :
                        cursor_notelayout.HalfLine + 5;
            }

            if (cursor_chordlayout.Length.Length > LengthClass.Quarter) {
                cursor_chordlayout.FreeFlags = cursor_chordlayout.Length.Length - LengthClass.Quarter;
                cursor_chordlayout.FlagDirection = cursor_chordlayout.StemDirection == NoteStemDirection.Down ? FlagDirection.Left : FlagDirection.Right;
            }
            gfx.TranslateTransform(caretx, 0);

            NoteRenderer
                .DrawChord(
                        gfx,
                        settings,
                        cursorcolor,
                        Color.SeaGreen,
                        cursor_chordlayout,
                        false,
                        500
                    );

            gfx.TranslateTransform(-caretx, 0);
        }
    }
}
