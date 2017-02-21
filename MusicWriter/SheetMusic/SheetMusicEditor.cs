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

        public int ActiveTrackIndex { get; set; } = 0;

        public MusicTrack ActiveTrack {
            get { return Tracks[ActiveTrackIndex] as MusicTrack; }
        }

        public Cursor Cursor { get; } =
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

            CommandCenter.WhenSelectAll += CommandCenter_WhenSelectAll;
            CommandCenter.WhenDeselectAll += CommandCenter_WhenDeselectAll;
            CommandCenter.WhenToggleSelectAll += CommandCenter_WhenToggleSelectAll;

            CommandCenter.WhenNotePlacementStart += CommandCenter_NotePlacementStart;
            CommandCenter.WhenNotePlacementFinish += CommandCenter_NotePlacementFinish;

            CommandCenter.WhenDeleteSelection += CommandCenter_WhenDeleteSelection;
            CommandCenter.WhenEraseSelection += CommandCenter_WhenEraseSelection;

            Pin.Time.ActualTime.AfterChange += Pin_ActualTime_AfterChange;
        }

        private void Tracks_ItemAdded(ITrack track) {
            var musictrack =
                track as MusicTrack;

            NoteSelections.Add(musictrack, new NoteSelection());

            if (ActiveTrackIndex == -1)
                ActiveTrackIndex = Tracks.Count - 1;

            InvalidateTime(new Duration { End = Tracks.MaxOrDefault(_ => _.Length.Value) });
        }

        private void Tracks_ItemRemoved(ITrack track) {
            var musictrack =
                track as MusicTrack;

            NoteSelections.Remove(musictrack);

            if (ActiveTrackIndex >= Tracks.Count)
                ActiveTrackIndex = Tracks.Count - 1;

            InvalidateTime(new Duration { End = Tracks.MaxOrDefault(_ => _.Length.Value) });
        }

        public static IFactory<ITrackController> CreateFactory() =>
            new CtorFactory<ITrackController, SheetMusicEditor>(ItemName);

        private class OverrideTrackControllerHints : TrackControllerHints
        {
            public SheetMusicEditor Editor { get; set; }

            public OverrideTrackControllerHints(SheetMusicEditor editor = null) {
                Editor = editor;
            }

            public override Time UnitSize(Time here) =>
                Editor
                    .ActiveTrack
                    .Rhythm
                    .Intersecting(here)
                    .First()
                    .Duration
                    .Length;

            public override Time WordSize(Time here) =>
                Editor
                    .ActiveTrack
                    .Rhythm
                    .TimeSignatures
                    .Intersecting_children(here)
                    .First()
                    .Duration
                    .Length;
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

        private void CommandCenter_NotePlacementFinish() {
        }

        private void CommandCenter_NotePlacementStart() {
            var tone =
                Cursor.Tone;

            var duration =
                Cursor.Caret.Duration;

            var noteID =
                ActiveTrack.Melody.AddNote(tone, duration);

            NoteSelections[ActiveTrack].Selected_End.Add(noteID);
            NoteSelections[ActiveTrack].Selected_Tone.Add(noteID);

            Refresh();
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
