using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class SheetMusicEditor : TrackController
    {
        public const string ItemName = "musicwriter.track-controller.controller.music.sheet";

        readonly TrackControllerHints hints;
        readonly TrackControllerContainer container;
        readonly Selector<ITrack> trackselector;

        public Selector<ITrack> TrackSelector {
            get { return trackselector; }
        }

        public Cursor Cursor { get; } =
            new Cursor();

        public Cursor CursorBackup { get; } =
            new Cursor();

        public override TrackControllerHints Hints {
            get { return hints; }
        }

        public Dictionary<MusicTrack, NoteSelection> NoteSelections { get; } =
            new Dictionary<MusicTrack, NoteSelection>();

        public event Action Redrawn;
        public event Action<Duration> Invalidated;

        public SheetMusicEditor(
                StorageObjectID storageobjectID, 
                EditorFile file,
                IFactory<ITrackController> factory
            ) :
            base(
                    storageobjectID, 
                    file,
                    factory
                ) {
            hints = new OverrideTrackControllerHints(this);

            container = file[TrackControllerContainer.ItemName] as TrackControllerContainer;

            Tracks.ItemAdded += Tracks_ItemAdded;
            Tracks.ItemRemoved += Tracks_ItemRemoved;

            CommandCenter.WhenCursor_Divide += CommandCenter_WhenCursor_Divide;
            CommandCenter.WhenCursor_Multiply += CommandCenter_WhenCursor_Multiply;
            CommandCenter.WhenCursor_ResetOne += CommandCenter_WhenCursor_ResetOne;

            CommandCenter.WhenSelectAll += CommandCenter_WhenSelectAll;
            CommandCenter.WhenDeselectAll += CommandCenter_WhenDeselectAll;
            CommandCenter.WhenToggleSelectAll += CommandCenter_WhenToggleSelectAll;

            CommandCenter.WhenToneChanged += CommandCenter_WhenToneChanged;
            CommandCenter.WhenTimeChanged += CommandCenter_WhenTimeChanged;
            CommandCenter.WhenPreviewToneChanged += CommandCenter_WhenPreviewToneChanged;
            CommandCenter.WhenPreviewTimeChanged += CommandCenter_WhenPreviewTimeChanged;
            CommandCenter.WhenToneStart += CommandCenter_WhenToneStart;
            CommandCenter.WhenTimeStart += CommandCenter_WhenTimeStart;
            CommandCenter.WhenToneReset += CommandCenter_WhenToneReset;
            CommandCenter.WhenTimeReset += CommandCenter_WhenTimeReset;

            CommandCenter.WhenSelectionStart += CommandCenter_WhenSelectionStart;
            CommandCenter.WhenSelectionFinish += CommandCenter_WhenSelectionFinish;

            CommandCenter.WhenNotePlacementStart += CommandCenter_WhenNotePlacementStart;
            CommandCenter.WhenNotePlacementFinish += CommandCenter_WhenNotePlacementFinish;

            CommandCenter.WhenDeleteSelection += CommandCenter_WhenDeleteSelection;
            CommandCenter.WhenEraseSelection += CommandCenter_WhenEraseSelection;

            CommandCenter.WhenUnitPicking += CommandCenter_WhenUnitPicking;

            Pin.Time.ActualTime.AfterChange += Pin_ActualTime_AfterChange;

            trackselector =
                new Selector<ITrack>(
                        file.Storage[storageobjectID].GetOrMake("selector").ID,
                        file,
                        Tracks,
                        allownull: false
                    );
        }

        public void Invalidate() {
            InvalidateTime(new Duration {
                Start = Time.Zero,
                Length = Tracks.MaxOrDefault(track => track.Length.Value)
            });
        }

        private void Tracks_ItemAdded(ITrack track) {
            var musictrack =
                track as MusicTrack;

            NoteSelections.Add(musictrack, new NoteSelection());

            if (TrackSelector.ActiveIndex.Value == -1)
                TrackSelector.ActiveIndex.Value = Tracks.Count - 1;

            InvalidateTime(new Duration { End = Tracks.MaxOrDefault(_ => _.Length.Value) });
        }

        private void Tracks_ItemRemoved(ITrack track) {
            var musictrack =
                track as MusicTrack;

            NoteSelections.Remove(musictrack);

            if (TrackSelector.ActiveIndex.Value >= Tracks.Count)
                TrackSelector.ActiveIndex.Value = Tracks.Count - 1;

            InvalidateTime(new Duration { End = Tracks.MaxOrDefault(_ => _.Length.Value) });
        }

        public static IFactory<ITrackController> CreateFactory() =>
            new CtorFactory<ITrackController, SheetMusicEditor>(
                    ItemName,
                    true
                );

        private class OverrideTrackControllerHints : TrackControllerHints
        {
            public SheetMusicEditor Editor { get; set; }

            public OverrideTrackControllerHints(SheetMusicEditor editor = null) {
                Editor = editor;
            }

            public override Time UnitSize(Time here) =>
                Editor
                    .TrackSelector
                    .Active
                    .Value
                    .As<ITrack, MusicTrack>()
                    .Rhythm
                    .Intersecting(here)
                    .First()
                    .Duration
                    .Length;

            public override Time WordSize(Time here) =>
                Editor
                    .TrackSelector
                    .Active
                    .Value
                    .As<ITrack, MusicTrack>()
                    .Rhythm
                    .TimeSignatures
                    .Intersecting_children(here)
                    .First()
                    .Duration
                    .Length;
        }

        private void CommandCenter_WhenCursor_Divide(int divisor) {
            if (!Cursor.Caret.Unit.Value.CanDivideInto(divisor))
                return;

            Cursor.Caret.Unit.Value /= divisor;

            Refresh();
        }

        private void CommandCenter_WhenCursor_Multiply(int factor) {
            Cursor.Caret.Unit.Value *= factor;

            Refresh();
        }

        private void CommandCenter_WhenCursor_ResetOne() {
            Cursor.Caret.Unit.Value = Time.Note;

            Refresh();
        }

        private void CommandCenter_WhenToggleSelectAll() {
            var any =
                NoteSelections
                    .Values
                    .Any(
                            selection =>
                                selection.Selected_Start.Any() ||
                                selection.Selected_End.Any() ||
                                selection.Selected_Tone.Any()
                        );

            if (any)
                CommandCenter_WhenDeselectAll();
            else CommandCenter_WhenSelectAll();
        }

        private void CommandCenter_WhenDeselectAll() {
            foreach (var selection in NoteSelections.Values) {
                selection.Selected_Start.Clear();
                selection.Selected_End.Clear();
                selection.Selected_Tone.Clear();
            }

            Refresh();
        }

        private void CommandCenter_WhenSelectAll() {
            foreach (var selectionkvp in NoteSelections) {
                var noteIDs =
                    selectionkvp
                        .Key
                        .Melody
                        .AllNotes()
                        .Select(note => note.ID);

                var selection =
                    selectionkvp.Value;

                foreach (var noteID in noteIDs) {
                    if (!selection.Selected_Start.Contains(noteID))
                        selection.Selected_Start.Add(noteID);
                    if (!selection.Selected_End.Contains(noteID))
                        selection.Selected_End.Add(noteID);
                    if (!selection.Selected_Tone.Contains(noteID))
                        selection.Selected_Tone.Add(noteID);
                }
            }

            Refresh();
        }
        
        private void CommandCenter_WhenToneChanged(int tone, CaretMode mode) {
            // action was already handled by preview
        }

        private void CommandCenter_WhenTimeChanged(Time time, CaretMode mode) {
            // action was already handled by preview
        }

        private void CommandCenter_WhenPreviewToneChanged(int tone, CaretMode mode) =>
            Effect_ToneChanged(tone, mode);

        private void CommandCenter_WhenPreviewTimeChanged(Time time, CaretMode mode) =>
            Effect_TimeChanged(time, mode);

        private void CommandCenter_WhenTimeStart() {
            foreach (var selection in NoteSelections)
                selection.Value.Save_Time(selection.Key);

            CursorBackup.Caret.Duration.Start = Cursor.Caret.Duration.Start;
            CursorBackup.Caret.Duration.Length = Cursor.Caret.Duration.Length;
            CursorBackup.Caret.Side.Value = Cursor.Caret.Side.Value;
        }

        private void CommandCenter_WhenToneStart() {
            foreach (var selection in NoteSelections)
                selection.Value.Save_Tone(selection.Key);

            CursorBackup.Tone.Value = Cursor.Tone.Value;
        }

        private void CommandCenter_WhenTimeReset() {
            foreach (var selection in NoteSelections)
                selection.Value.Restore_Time(selection.Key);

            Cursor.Caret.Duration.Offset = CursorBackup.Caret.Duration.Offset;
            Cursor.Caret.Duration.Length = CursorBackup.Caret.Duration.Length;
            Cursor.Caret.Side.Value = CursorBackup.Caret.Side.Value;

            Refresh();
        }

        private void CommandCenter_WhenToneReset() {
            foreach (var selection in NoteSelections)
                selection.Value.Restore_Tone(selection.Key);

            Cursor.Tone.Value = CursorBackup.Tone.Value;

            Refresh();
        }

        private void CommandCenter_WhenSelectionFinish() {
            Cursor.Caret.Side.Value = Caret.FocusSide.Both;

            Refresh();
        }

        private void CommandCenter_WhenSelectionStart() {
            Cursor.Caret.Side.Value = Caret.FocusSide.Right;

            Refresh();
        }

        void Effect_TimeChanged(Time time, CaretMode mode) {
            var effectedarea = default(Duration);

            foreach (var trackkvp in NoteSelections) {
                var track = trackkvp.Key;

                foreach (var noteID in trackkvp.Value.Selected_Start.Union(trackkvp.Value.Selected_End)) {
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

                    effectedarea =
                        oldduration
                            .Union(newduration)
                            .Union(effectedarea);

                    track.Melody.UpdateNote(note.ID, newduration, note.Tone);
                }
            }

            if (mode == CaretMode.Absolute)
                Cursor.Caret.Focus = time;
            else if (mode == CaretMode.Delta)
                Cursor.Caret.Focus += time;

            //TODO: update only the affected duration

            if (effectedarea != null)
                InvalidateTime(effectedarea);

            Refresh();
        }

        void Effect_ToneChanged(int tone, CaretMode mode) {
            var effectedarea = default(Duration);

            foreach (var trackkvp in NoteSelections) {
                var track = trackkvp.Key;

                foreach (var noteID in trackkvp.Value.Selected_Tone) {
                    var note = track.Melody[noteID];

                    effectedarea = note.Duration.Union(effectedarea);

                    var newtone =
                        Effect_ToneChanged_affect(
                                note.Tone,
                                note.Duration.Start, 
                                tone, 
                                mode, 
                                track
                            );

                    track.Melody.UpdateNote(note.ID, note.Duration, newtone);
                }
            }

            Cursor
                .Tone
                .Value =
                Effect_ToneChanged_affect(
                        Cursor.Tone.Value,
                        Cursor.Caret.Focus,
                        tone,
                        mode,
                        TrackSelector
                            .Active
                            .Value
                            as MusicTrack
                    );

            if (effectedarea != null)
                InvalidateTime(effectedarea);

            Refresh();
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

        private void CommandCenter_WhenNotePlacementStart() {
            var tone =
                Cursor.Tone.Value;

            var duration =
                Cursor.Caret.Duration;

            var noteID =
                TrackSelector.Active.Value.As<ITrack, MusicTrack>().Melody.AddNote(tone, duration);

            NoteSelections[TrackSelector.Active.Value.As<ITrack, MusicTrack>()].Selected_End.Add(noteID);
            NoteSelections[TrackSelector.Active.Value.As<ITrack, MusicTrack>()].Selected_Tone.Add(noteID);

            Refresh();
        }

        private void CommandCenter_WhenNotePlacementFinish() {
        }

        private void CommandCenter_WhenDeleteSelection() {
            CommandCenter_WhenDeleteSelectedNotes();

            foreach (MusicTrack track in Tracks)
                track.Delete(Cursor.Caret.Duration);
        }

        private void CommandCenter_WhenEraseSelection() {
            CommandCenter_WhenDeleteSelectedNotes();

            foreach (MusicTrack track in Tracks)
                track.Erase(Cursor.Caret.Duration);
        }

        public void CommandCenter_WhenDeleteSelectedNotes() {
            var effectedarea = default(Duration);

            foreach (var selectionkvp in NoteSelections) {
                var track =
                    selectionkvp.Key;

                var selection =
                    new NoteID[0]
                        .Concat(selectionkvp.Value.Selected_End)
                        .Concat(selectionkvp.Value.Selected_Start)
                        .Concat(selectionkvp.Value.Selected_Tone)
                        .Distinct()
                        .ToArray();

                foreach (var noteID in selection) {
                    var note = track.Melody[noteID];

                    track.Melody.DeleteNote(noteID);

                    if (selectionkvp.Value.Selected_Start.Contains(noteID))
                        selectionkvp.Value.Selected_Start.Remove(noteID);
                    if (selectionkvp.Value.Selected_End.Contains(noteID))
                        selectionkvp.Value.Selected_End.Remove(noteID);
                    if (selectionkvp.Value.Selected_Tone.Contains(noteID))
                        selectionkvp.Value.Selected_Tone.Remove(noteID);

                    effectedarea = note.Duration.Union(effectedarea);
                }
            }

            if (effectedarea != null)
                InvalidateTime(effectedarea);

            Refresh();
        }

        private void CommandCenter_WhenUnitPicking(CaretUnitPickerEventArgs args) {
            args.Length = Cursor.Caret.Unit.Value;
            args.Handled = true;
        }

        private void Pin_ActualTime_AfterChange(Time old, Time @new) {
            Refresh();
        }

        public void InvalidateTime(Duration duration) {
            foreach (MusicTrack track in Tracks)
                container.Settings.MusicBrain.Invalidate(track.Memory, duration);

            Invalidated?.Invoke(duration);

            Refresh();
        }
        
        public void Refresh() {
            Redrawn?.Invoke();
        }
    }
}
