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
            track.Adornment.SetStaff(Staff.Treble, Duration.Eternity);
            track.Adornment.SetKeySignature(KeySignature.Create(KeyClass.C, PitchTransform.Natural, Mode.Major), Duration.Eternity);

            var propertygraphlet =
                new ExplicitPropertyGraphlet<NoteID>();

            var brain =
                new MusicBrain(
                        propertygraphlet,
                        track
                    );

            brain.InsertCog(new IgnorantPerceptualCog<Cell>(track.Rhythm));
            brain.InsertCog(new IgnorantPerceptualCog<Simple>(track.Rhythm));
            brain.InsertCog(new IgnorantPerceptualCog<Measure>(track.Rhythm));
            brain.InsertCog(new IgnorantPerceptualCog<Note>(track.Melody));
            brain.InsertCog(new IgnorantPerceptualCog<KeySignature>(track.Adornment.KeySignatures));
            brain.InsertCog(new IgnorantPerceptualCog<Staff>(track.Adornment.Staffs));
            brain.InsertCog(new NotePerceptualCog());
            //brain.InsertCog(new NoteLayoutPerceptualCog());
            //brain.InsertCog(new NoteLayoutPerceptualCog());
            //brain.InsertCog(new ChordLayoutPerceptualCog());
            brain.InsertCog(new MeasureLayoutPerceptualCog());

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
