using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicWriter.TimeSignature;

namespace MusicWriter {
    public sealed class EditorFile {
        readonly MusicBrain brain = new MusicBrain();
        readonly Dictionary<string, ITrack> trackmap =
            new Dictionary<string, ITrack>();

        public ObservableCollection<ITrack> Tracks { get; } =
            new ObservableCollection<ITrack>();

        public ITrack this[string name] {
            get { return trackmap[name]; }
        }

        public MusicBrain Brain {
            get { return brain; }
        }

        public EditorFile() {
            InitializeBrain();

            Tracks.CollectionChanged += Tracks_CollectionChanged;
        }

        private void Tracks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if (e.OldItems != null)
                foreach (ITrack removedtrack in e.OldItems)
                    Remove(removedtrack);

            if (e.NewItems != null)
                foreach (ITrack addedtrack in e.NewItems)
                    Add(addedtrack);
        }

        void Rename(string old, string @new) {
            if (trackmap.ContainsKey(@new))
                throw new InvalidOperationException("Cannot overwrite track");

            trackmap.Add(@new, trackmap[old]);
            trackmap.Remove(old);
        }

        void Remove(ITrack track) {
            if (!trackmap.ContainsKey(track.Name.Value))
                throw new InvalidOperationException("Cannot delete track - doesn't exist");
            
            track.Name.BeforeChange -= Rename;

            Tracks.Remove(track);
            trackmap.Remove(track.Name.Value);
        }
        
        void Add(ITrack track) {
            if (trackmap.ContainsKey(track.Name.Value))
                throw new InvalidOperationException("Cannot add track that already exists");

            track.Name.BeforeChange += Rename;

            trackmap.Add(track.Name.Value, track);
        }

        public MusicTrack NewMusicTrack(string name) {
            var propertygraphlet =
                new ExplicitPropertyGraphlet<NoteID>();

            var perceptualmemory =
                new PerceptualMemory();

            var track =
                new MusicTrack(
                        new MelodyTrack(),
                        new RhythmTrack(),
                        new AdornmentTrack(),
                        perceptualmemory,
                        propertygraphlet
                    );

            track.Name.Value = name;

            track.Rhythm.TimeSignatures.ScootAndOverwrite(new TimeSignature(new Simple(4, 4)), Duration.Eternity);
            track.Rhythm.MeterSignatures.ScootAndOverwrite(MeterSignature.Default(track.Rhythm.TimeSignaturesInTime(Duration.Eternity).Single().Value.Simples[0]), Duration.Eternity);
            track.Adornment.Staffs.ScootAndOverwrite(Staff.Treble, Duration.Eternity);
            track.Adornment.KeySignatures.ScootAndOverwrite(KeySignature.Create(DiatonicToneClass.C, PitchTransform.Natural, Mode.Major), Duration.Eternity);
            
            perceptualmemory.InsertMemoryModule(new EditableMemoryModule<NoteLayout>());
            perceptualmemory.InsertMemoryModule(new EditableMemoryModule<ChordLayout>());
            perceptualmemory.InsertMemoryModule(new EditableMemoryModule<MeasureLayout>());
            perceptualmemory.InsertMemoryModule(new IgnorantMemoryModule<Cell>(track.Rhythm));
            perceptualmemory.InsertMemoryModule(new IgnorantMemoryModule<Simple>(track.Rhythm));
            perceptualmemory.InsertMemoryModule(new IgnorantMemoryModule<Measure>(track.Rhythm));
            perceptualmemory.InsertMemoryModule(new IgnorantMemoryModule<Note>(track.Melody));
            perceptualmemory.InsertMemoryModule(new IgnorantMemoryModule<KeySignature>(track.Adornment.KeySignatures));
            perceptualmemory.InsertMemoryModule(new IgnorantMemoryModule<Staff>(track.Adornment.Staffs));
            perceptualmemory.InsertMemoryModule(new NotePerceptualCog.MemoryModule());

            Add(track);

            return track;
        }
        
        public void Load(string uri) {

        }

        public void Save(string uri) {

        }

        void InitializeBrain() {
            brain.InsertCog(new NotePerceptualCog());
            //brain.InsertCog(new NoteLayoutPerceptualCog());
            //brain.InsertCog(new NoteLayoutPerceptualCog());
            //brain.InsertCog(new ChordLayoutPerceptualCog());
            brain.InsertCog(new MeasureLayoutPerceptualCog());
        }
    }
}
