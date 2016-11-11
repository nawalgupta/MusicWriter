using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicWriter.TimeSignature;

namespace MusicWriter.WinForms {
    public sealed class MusicEditorFile {
        readonly Dictionary<string, Track> tracks =
            new Dictionary<string, Track>();
        readonly Dictionary<string, MusicBrain> brains = 
            new Dictionary<string, MusicBrain>();
        readonly Dictionary<string, IPropertyGraphlet<NoteID>> notepropertygraphlets =
            new Dictionary<string, IPropertyGraphlet<NoteID>>();

        public Track this[string name] {
            get { return tracks[name]; }
        }

        public void Rename(string old, string @new) {
            if (tracks.ContainsKey(@new))
                throw new InvalidOperationException("Cannot overwrite track");

            tracks.Add(@new, tracks[old]);
            tracks.Remove(old);
        }

        public void Delete(string track) {
            if (!tracks.ContainsKey(track))
                throw new InvalidOperationException("Cannot delete track - doesn't exist");

            tracks.Remove(track);
        }

        public Track Add(string name) {
            if (tracks.ContainsKey(name))
                throw new InvalidOperationException("Cannot create track that already exists");

            var track =
                new Track(
                        new MelodyTrack(),
                        new RhythmTrack(),
                        new AdornmentTrack()
                    );

            track.Rhythm.SetTimeSignature(new TimeSignature(new TimeSignature.Simple(4, 4)), Duration.Eternity);
            track.Rhythm.SetMeter(MeterSignature.Default(track.Rhythm.TimeSignaturesInTime(Duration.Eternity).Single().Value.Simples[0]), Duration.Eternity);

            var propertygraphlet =
                new ExplicitPropertyGraphlet<NoteID>();

            var brain =
                new MusicBrain(
                        propertygraphlet
                    );

            brain.Cogs.Add(new IgnorantPerceptualCog<Cell>(track.Rhythm));
            brain.Cogs.Add(new IgnorantPerceptualCog<Simple>(track.Rhythm));
            brain.Cogs.Add(new IgnorantPerceptualCog<Note>(track.Melody));
            brain.Cogs.Add(new NotePerceptualCog());

            tracks.Add(name, track);
            notepropertygraphlets.Add(name, propertygraphlet);
            brains.Add(name, brain);

            return track;
        }

        public MusicBrain BrainFor(string name) =>
            brains[name];
        
        public void Load(string uri) {

        }

        public void Save(string uri) {

        }
    }
}
