using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class NoteSelection
    {
        public List<NoteID> Selected_Start { get; } = new List<NoteID>();
        public List<NoteID> Selected_End { get; } = new List<NoteID>();
        public List<NoteID> Selected_Tone { get; } = new List<NoteID>();

        public IEnumerable<NoteID> Selected_NoteIDs {
            get { return Selected_End.Concat(Selected_Start).Concat(Selected_Tone); }
        }

        readonly Dictionary<NoteID, Time> backups_start = new Dictionary<NoteID, Time>();
        readonly Dictionary<NoteID, Time> backups_end = new Dictionary<NoteID, Time>();
        readonly Dictionary<NoteID, SemiTone> backups_tone = new Dictionary<NoteID, SemiTone>();

        public void Save_Time(MusicTrack track) {
            backups_start.Clear();
            backups_end.Clear();

            foreach (var noteID in Selected_Start)
                backups_start.Add(noteID, track.Melody[noteID].Duration.Start);

            foreach (var noteID in Selected_End)
                backups_end.Add(noteID, track.Melody[noteID].Duration.End);
        }

        public void Save_Tone(MusicTrack track) {
            backups_tone.Clear();
            
            foreach (var noteID in Selected_Tone)
                backups_tone.Add(noteID, track.Melody[noteID].Tone);
        }

        public void Restore_Time(MusicTrack track) {
            foreach (var kvp in backups_start) {
                var oldnote =
                    track.Melody[kvp.Key];

                track
                    .Melody
                    .UpdateNote(
                            kvp.Key,
                            new Duration {
                                Start = kvp.Value,
                                End = oldnote.Duration.End
                            },
                            oldnote.Tone
                        );
            }

            foreach (var kvp in backups_end) {
                var oldnote =
                    track.Melody[kvp.Key];

                track
                    .Melody
                    .UpdateNote(
                            kvp.Key,
                            new Duration {
                                Start = oldnote.Duration.Start,
                                End = kvp.Value
                            },
                            oldnote.Tone
                        );
            }

            backups_start.Clear();
            backups_end.Clear();
        }

        public void Restore_Tone(MusicTrack track) {
            foreach (var kvp in backups_tone) {
                var oldnote =
                    track.Melody[kvp.Key];

                track
                    .Melody
                    .UpdateNote(
                            kvp.Key,
                            oldnote.Duration,
                            kvp.Value
                        );
            }
            
            backups_tone.Clear();
        }
    }
}
