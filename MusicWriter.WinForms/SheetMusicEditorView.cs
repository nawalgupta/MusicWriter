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

namespace MusicWriter.WinForms
{
    public partial class SheetMusicEditorView : Control
    {
        SheetMusicEditor editor;
        TrackControllerContainer container;

        public SheetMusicEditor Editor {
            get { return editor; }
            set {
                if (editor != null)
                    throw new InvalidOperationException();

                editor = value;

                container = editor.File[TrackControllerContainer.ItemName] as TrackControllerContainer;
                container.Settings.MusicBrain.InsertCog(new RenderedSheetMusicItemPerceptualCog());

                editor.Tracks.ItemAdded += Tracks_ItemAdded;
                editor.Tracks.ItemRemoved += Tracks_ItemRemoved;

                editor.Invalidated += Editor_Invalidated;
                editor.Redrawn += Editor_Redrawn;

                mouseselector.Selected += Mouseselector_Selected;
                mouseselector.Redraw += () => Invalidate();

                editor.Invalidate();
            }
        }

        public SheetMusicRenderSettings TemplateSettings { get; set; } = new SheetMusicRenderSettings();

        readonly Dictionary<Duration, float> minwidths =
            new Dictionary<Duration, float>();

        readonly Dictionary<MusicTrack, Dictionary<RenderedSheetMusicItem, float>> itemwidths =
            new Dictionary<MusicTrack, Dictionary<RenderedSheetMusicItem, float>>();

        readonly Dictionary<MusicTrack, float> trackheights =
            new Dictionary<MusicTrack, float>();

        readonly RectangleMouseSelector mouseselector =
            new RectangleMouseSelector();
        
        public Control View {
            get { return this; }
        }

        public SheetMusicEditorView() {
            InitializeComponent();
            
            DoubleBuffered = true;
        }

        private void Editor_Invalidated(Duration window) {
            MeasureAndLayoutTrackItems();

            Height = (int)trackheights.Values.Sum();
        }

        private void Editor_Redrawn() {
            Refresh();
        }
        
        protected override void OnGotFocus(EventArgs e) {
            BackColor = Color.FromArgb(255, 251, 227);

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e) {
            BackColor = Color.FromArgb(241, 234, 200);

            base.OnLostFocus(e);
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            if (!Focused)
                Focus();

            mouseselector.MouseDown(e, ModifierKeys);

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            if (!Focused)
                Focus();

            mouseselector.MouseMove(e, ModifierKeys);

            var activetrackindex_new = -1;
            var y = 0f;
            for (int i = 0; i < editor.Tracks.Count; i++) {
                var h = trackheights[editor.Tracks[i] as MusicTrack];

                if (y <= e.Y && y + h > e.Y)
                    activetrackindex_new = i;

                y += h;
            }

            if (activetrackindex_new != editor.ActiveTrackIndex) {
                editor.ActiveTrackIndex = activetrackindex_new;
                Refresh();
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            mouseselector.MouseUp(e);

            base.OnMouseUp(e);
        }

        private void Mouseselector_Selected(RectangleF rect, NoteSelectionMode mode) {
            //TODO: does this work when a track has different staves throughout its duration?

            var effectedarea = default(Duration);

            if (mode == NoteSelectionMode.Replace) {
                foreach (var selectionkvp in editor.NoteSelections) {
                    var track = selectionkvp.Key;
                    var selection = selectionkvp.Value;

                    foreach (var noteID in selection.Selected_NoteIDs)
                        effectedarea = track.Melody[noteID].Duration.Union(effectedarea);

                    selection.Selected_Start.Clear();
                    selection.Selected_End.Clear();
                    selection.Selected_Tone.Clear();
                }
            }

            for (int i = 0; i < editor.Tracks.Count; i++) {
                var track =
                    editor.Tracks[i] as MusicTrack;

                var selection = editor.NoteSelections[track];

                var fakeselection = new NoteSelection();

                foreach (var itemtuple in GetItemsWithRects(track)) {
                    if (itemtuple.Item1.IntersectsWith(rect)) {
                        rect.Offset(-itemtuple.Item1.X, -itemtuple.Item1.Y);

                        itemtuple.Item3.Select(fakeselection, rect, itemtuple.Item2, itemtuple.Item1.Width);

                        rect.Offset(+itemtuple.Item1.X, +itemtuple.Item1.Y);
                    }
                }

                switch (mode) {
                    case NoteSelectionMode.Replace:
                    case NoteSelectionMode.Add:
                        foreach (var noteID in fakeselection.Selected_Start) {
                            var note = track.Melody[noteID];
                            effectedarea = note.Duration.Union(effectedarea);

                            selection.Selected_Start.Add(noteID);
                        }
                        foreach (var noteID in fakeselection.Selected_End) {
                            var note = track.Melody[noteID];
                            effectedarea = note.Duration.Union(effectedarea);

                            selection.Selected_End.Add(noteID);
                        }
                        foreach (var noteID in fakeselection.Selected_Tone) {
                            var note = track.Melody[noteID];
                            effectedarea = note.Duration.Union(effectedarea);

                            selection.Selected_Tone.Add(noteID);
                        }
                        break;

                    case NoteSelectionMode.Intersect:
                        foreach (var noteID in selection.Selected_Start.ToArray())
                            if (!fakeselection.Selected_Start.Contains(noteID)) {
                                var note = track.Melody[noteID];
                                effectedarea = note.Duration.Union(effectedarea);

                                selection.Selected_Start.Remove(noteID);
                            }
                        foreach (var noteID in selection.Selected_End.ToArray())
                            if (!fakeselection.Selected_End.Contains(noteID)) {
                                var note = track.Melody[noteID];
                                effectedarea = note.Duration.Union(effectedarea);

                                selection.Selected_End.Remove(noteID);
                            }
                        foreach (var noteID in selection.Selected_Tone.ToArray())
                            if (!fakeselection.Selected_Tone.Contains(noteID)) {
                                var note = track.Melody[noteID];
                                effectedarea = note.Duration.Union(effectedarea);

                                selection.Selected_Tone.Remove(noteID);
                            }
                        break;

                    case NoteSelectionMode.Subtract:
                        foreach (var noteID in selection.Selected_Start.ToArray())
                            if (fakeselection.Selected_Start.Contains(noteID)) {
                                var note = track.Melody[noteID];
                                effectedarea = note.Duration.Union(effectedarea);

                                selection.Selected_Start.Remove(noteID);
                            }
                        foreach (var noteID in selection.Selected_End.ToArray())
                            if (fakeselection.Selected_End.Contains(noteID)) {
                                var note = track.Melody[noteID];
                                effectedarea = note.Duration.Union(effectedarea);

                                selection.Selected_End.Remove(noteID);
                            }
                        foreach (var noteID in selection.Selected_Tone.ToArray())
                            if (fakeselection.Selected_Tone.Contains(noteID)) {
                                var note = track.Melody[noteID];
                                effectedarea = note.Duration.Union(effectedarea);

                                selection.Selected_Tone.Remove(noteID);
                            }
                        break;

                    case NoteSelectionMode.Xor:
                        foreach (var noteID in fakeselection.Selected_Start) {
                            var note = track.Melody[noteID];
                            effectedarea = note.Duration.Union(effectedarea);

                            if (selection.Selected_Start.Contains(noteID))
                                selection.Selected_Start.Remove(noteID);
                            else selection.Selected_Start.Add(noteID);
                        }
                        foreach (var noteID in fakeselection.Selected_End) {
                            var note = track.Melody[noteID];
                            effectedarea = note.Duration.Union(effectedarea);

                            if (selection.Selected_End.Contains(noteID))
                                selection.Selected_End.Remove(noteID);
                            else selection.Selected_End.Add(noteID);
                        }
                        foreach (var noteID in fakeselection.Selected_Tone) {
                            var note = track.Melody[noteID];
                            effectedarea = note.Duration.Union(effectedarea);

                            if (selection.Selected_Tone.Contains(noteID))
                                selection.Selected_Tone.Remove(noteID);
                            else selection.Selected_Tone.Add(noteID);
                        }
                        break;
                }
            }

            if (effectedarea != null)
                editor.InvalidateTime(effectedarea);

            Invalidate();
        }

        void Tracks_ItemAdded(ITrack track) {
            var musictrack =
                track as MusicTrack;

            musictrack.Memory.InsertMemoryModule(new RenderedSheetMusicItemPerceptualCog.MemoryModule());
        }

        void Tracks_ItemRemoved(ITrack track) {
            var musictrack =
                track as MusicTrack;

            musictrack.Memory.RemoveMemoryModule<RenderedSheetMusicItem>();
        }

        IEnumerable<Tuple<RectangleF, SheetMusicRenderSettings, RenderedSheetMusicItem>> GetItemsWithRects(MusicTrack track) {
            var scrollX = GetLeft(editor.Pin.Time.ActualTime.Value);
            var yoffset = editor.Tracks.TakeWhile(t => !ReferenceEquals(t, track)).Sum(t => trackheights[t as MusicTrack]);
            
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

                float x = 0 - scrollX;

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

                        focusitems.Sort((a, b) => b.Priority.CompareTo(a.Priority));

                        focussettings = GetSettings(focusstarttime, track);
                    }

                    var item = focusitems[0];
                    focusitems.RemoveAt(0);

                    var width = itemwidths[track][item];
                    yield return
                        new Tuple<RectangleF, SheetMusicRenderSettings, RenderedSheetMusicItem>(
                                new RectangleF(
                                        x,
                                        yoffset,
                                        width,
                                        focussettings.Height
                                    ),
                                focussettings,
                                item
                            );

                    x += width;
                }
            }
        }

        void MeasureAndLayoutTrackItems() {
            // we're extracting every key point in all the tracks and looking for times
            // where they share the same time. This is used to identify how much to stretch
            // each track item when its rendered.

            //TODO: handle 0 samepoints
            if (editor.Tracks.Count == 0)
                return;

            trackheights.Clear();
            foreach (MusicTrack track in editor.Tracks)
                trackheights.Add(track, 0);

            var unjoinedsamepoints =
                editor
                    .Tracks
                    .Cast<MusicTrack>()
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
                                    .Distinct()
                        );

            var samepoints =
                unjoinedsamepoints
                    .Any() ?
                    unjoinedsamepoints.Aggregate(Enumerable.Intersect).OrderBy(t => t) :
                    Enumerable.Empty<Time>();

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

            foreach (var track in editor.Tracks.Cast<MusicTrack>()) {
                desireditemchunkwidths.Add(track, new Dictionary<Duration, float>());
                desireditemchunkwidths_stretchy.Add(track, new Dictionary<Duration, float>());
                desireditemchunkwidths_fixed.Add(track, new Dictionary<Duration, float>());
                itemwidths.Add(track, new Dictionary<RenderedSheetMusicItem, float>());
            }

            // Measure
            foreach (var duration in samedurations) {
                var minwidth = 0f;
                foreach (var track in editor.Tracks.Cast<MusicTrack>()) {
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
                foreach (var track in editor.Tracks.Cast<MusicTrack>()) {
                    var items =
                        track
                            .Memory
                            .Analyses<RenderedSheetMusicItem>(minwidthkvp.Key)
                            .ToArray();

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

            float x = 0;

            if (starttimes.Count != 0) {
                var focusstarttime = default(Time);

                var focusitems = new List<IDuratedItem<RenderedSheetMusicItem>>();

                x = GetLeft(focusstarttime);

                while (focusstarttime <= time) {
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
                                );

                        focusitems.Sort((a, b) => b.Value.Priority.CompareTo(a.Value.Priority));

                        focussettings = GetSettings(focusstarttime, track);
                    }

                    var focusitem = focusitems[0];
                    var item = focusitem.Value;
                    focusitems.RemoveAt(0);

                    var width = itemwidths[track][item];

                    if (item.Stretchy && focusitem.Duration.Contains(time))
                        return x + item.PixelAtTime(time - focusitem.Duration.Start, width, focussettings);

                    x += width;
                }
            }


            return x;
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
               Selection = editor.NoteSelections[track],
               ThumbMarginX = TemplateSettings.ThumbMarginX,
               ThumbMarginY = TemplateSettings.ThumbMarginY,
               ThumbPadding = TemplateSettings.ThumbPadding,
               ThumbWidth = TemplateSettings.ThumbWidth
           };
        
        int timesRedrawn = 0;
        void DrawToGraphics(Graphics gfx) {
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            //gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            
            gfx.DrawString(timesRedrawn.ToString(), Font, Brushes.Red, PointF.Empty);

            var scrollX = GetLeft(editor.Pin.Time.ActualTime.Value);

            foreach (var track in editor.Tracks.Cast<MusicTrack>()) {
                var active =
                    ReferenceEquals(track, editor.ActiveTrack);
                
                // draw sheet items
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
                    var focusstarttime = default(Time);

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

                            focusitems.Sort((a, b) => b.Priority.CompareTo(a.Priority));

                            focussettings = GetSettings(focusstarttime, track);
                            
                            // draw staff
                            for (int line = 0; line < focussettings.Staff.Lines; line++)
                                gfx.DrawLine(
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
                            item.Draw(gfx, focussettings, (int)width, (int)x);

                        x += width;
                    }
                }

                DrawCaret(gfx, GetSettings(editor.Cursor.Caret.Focus, track), active, track, scrollX);

                if (trackheights.ContainsKey(track))
                    gfx.TranslateTransform(0, trackheights[track]);
            }

            gfx.ResetTransform();
            mouseselector.Draw(gfx);
        }
        
        protected override void OnPaint(PaintEventArgs e) {
            DrawToGraphics(e.Graphics);

            base.OnPaint(e);
        }
        
        void DrawCaret(
                Graphics gfx,
                SheetMusicRenderSettings settings,
                bool active,
                MusicTrack track,
                float scrollX
            ) {
            var caretx =
                GetLeft(editor.Cursor.Caret.Focus, track) - scrollX;

            var caretunitx =
                GetLeft(editor.Cursor.Caret.Focus + editor.Cursor.Caret.Unit.Value, track) - scrollX;

            var caretstaff =
                track
                    .Adornment
                    .Staffs
                    .Intersecting(editor.Cursor.Caret.Focus)
                    .First()
                    .Value;

            PitchTransform transform;

            var caretkey =
                track
                    .Adornment
                    .KeySignatures
                    .Intersecting(editor.Cursor.Caret.Focus)
                    .First()
                    .Value
                    .Key(editor.Cursor.Tone.Value, out transform);

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
                    Start = editor.Cursor.Caret.Focus,
                    Length = editor.Cursor.Caret.Unit.Value
                };

            var cursor_notelayout =
                new NoteLayout(
                        new PerceptualNote(
                                default(PerceptualNoteID),
                                cursor_focusduration,
                                PerceptualTime.Decompose(editor.Cursor.Caret.Unit.Value).First().Key,
                                track.Rhythm.Intersecting(cursor_focusduration).First(),
                                new Note(
                                        default(NoteID),
                                        cursor_focusduration,
                                        editor.Cursor.Tone.Value
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
