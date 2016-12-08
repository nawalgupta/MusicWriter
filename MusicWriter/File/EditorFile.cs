using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicWriter.TimeSignature;

namespace MusicWriter {
    public sealed class EditorFile<View> {
        readonly MusicBrain brain = new MusicBrain();
        readonly Dictionary<string, ITrack> trackmap =
            new Dictionary<string, ITrack>();

        public ObservableCollection<ITrack> Tracks { get; } =
            new ObservableCollection<ITrack>();

        public ObservableCollection<ITrackController<View>> Controllers { get; } =
            new ObservableCollection<ITrackController<View>>();

        public ObservableCollection<Screen<View>> Screens { get; } =
            new ObservableCollection<Screen<View>>();

        public FileCapabilities<View> Capabilities =
            new FileCapabilities<View>();

        public bool IsDirty { get; private set; } = false;

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
            
            track.Dirtied -= Track_Dirtied;

            Tracks.Remove(track);
            trackmap.Remove(track.Name.Value);
        }

        public ITrackController<View> GetController(string value) =>
            Controllers.First(controller => controller.Name.Value == value);

        void Add(ITrack track) {
            if (trackmap.ContainsKey(track.Name.Value))
                throw new InvalidOperationException("Cannot add track that already exists");

            track.Name.BeforeChange += Rename;
            
            track.Dirtied += Track_Dirtied;

            trackmap.Add(track.Name.Value, track);
        }

        private void Track_Dirtied() {
            IsDirty = true;
        }

        //public MusicTrack NewMusicTrack(string name) {
        //    var propertygraphlet =
        //        new ExplicitPropertyGraphlet<NoteID>();

        //    var perceptualmemory =
        //        new PerceptualMemory();

        //    var track =
        //        new MusicTrack(
        //                new MelodyTrack(),
        //                new RhythmTrack(),
        //                new AdornmentTrack(),
        //                perceptualmemory,
        //                propertygraphlet
        //            );

        //    track.Name.Value = name;

        //    track.Rhythm.TimeSignatures.ScootAndOverwrite(new TimeSignature(new Simple(4, 4)), Duration.Eternity);
        //    track.Rhythm.MeterSignatures.ScootAndOverwrite(MeterSignature.Default(track.Rhythm.TimeSignaturesInTime(Duration.Eternity).Single().Value.Simples[0]), Duration.Eternity);
        //    track.Adornment.Staffs.ScootAndOverwrite(Staff.Treble, Duration.Eternity);
        //    track.Adornment.KeySignatures.ScootAndOverwrite(KeySignature.Create(DiatonicToneClass.C, PitchTransform.Natural, Mode.Major), Duration.Eternity);

        //    perceptualmemory.InsertMemoryModule(new EditableMemoryModule<NoteLayout>());
        //    perceptualmemory.InsertMemoryModule(new EditableMemoryModule<ChordLayout>());
        //    perceptualmemory.InsertMemoryModule(new EditableMemoryModule<MeasureLayout>());
        //    perceptualmemory.InsertMemoryModule(new IgnorantMemoryModule<Cell>(track.Rhythm));
        //    perceptualmemory.InsertMemoryModule(new IgnorantMemoryModule<Simple>(track.Rhythm));
        //    perceptualmemory.InsertMemoryModule(new IgnorantMemoryModule<Measure>(track.Rhythm));
        //    perceptualmemory.InsertMemoryModule(new IgnorantMemoryModule<Note>(track.Melody));
        //    perceptualmemory.InsertMemoryModule(new IgnorantMemoryModule<KeySignature>(track.Adornment.KeySignatures));
        //    perceptualmemory.InsertMemoryModule(new IgnorantMemoryModule<Staff>(track.Adornment.Staffs));
        //    perceptualmemory.InsertMemoryModule(new NotePerceptualCog.MemoryModule());

        //    Add(track);

        //    return track;
        //}

        public void Load(Stream zipstream) {
            IsDirty = false;

            Tracks.Clear();
            Controllers.Clear();
            Screens.Clear();

            using (var zip = new ZipArchive(zipstream, ZipArchiveMode.Read)) {
                foreach (var track_name in CodeTools.ReadKVPs(zip.GetEntry("tracks").Open())) {
                    var entry = zip.GetEntry($"tracks/{track_name.Key}");

                    var trackfactory =
                        Capabilities
                            .TrackFactories
                            .FirstOrDefault(factory => factory.Name == track_name.Value);

                    using (var stream = entry.Open()) {
                        var track =
                            trackfactory.Load(stream);

                        track.Name.Value = track_name.Key;

                        Add(track);
                    }
                }

                foreach (var controller_name in CodeTools.ReadKVPs(zip.GetEntry("controllers").Open())) {
                    var entry = zip.GetEntry($"controllers/{controller_name.Key}");

                    var controllerfactory =
                        Capabilities
                            .ControllerFactories
                            .FirstOrDefault(factory => factory.Name == controller_name.Value);

                    using (var stream = entry.Open()) {
                        var controller =
                            controllerfactory.Load(
                                    stream,
                                    this
                                );
                        
                        controller.Name.Value = controller_name.Key;

                        Controllers.Add(controller);
                    }
                }

                foreach (var entry in zip.Entries.Where(entry => entry.FullName.StartsWith("screens/"))) {
                    var screen = new Screen<View>(this);
                    screen.Load(entry.Open());
                    Screens.Add(screen);
                }
            }
        }

        public void Save(Stream zipstream) {
            IsDirty = false;

            //TODO: consider supporting abstract directories so the
            // file can be saved to either a zip or to the filesystem.
            // If the entire composition file is saved to the filesystem,
            // then a fs watcher can observe new changes, git can be
            // used, outside tools can access data easier.

            using (var zip = new ZipArchive(zipstream, ZipArchiveMode.Create)) {
                using (var stream = zip.CreateEntry("tracks").Open()) {
                    CodeTools.WriteKVPs(stream, Tracks.Select(track => new KeyValuePair<string, string>(track.Name.Value, track.Factory.Name)));
                }

                foreach (var track in Tracks) {
                    using (var stream = zip.CreateEntry($"tracks/{track.Name}").Open()) {
                        track.Factory.Save(track, stream);
                    }
                }

                using (var stream = zip.CreateEntry("controllers").Open()) {
                    CodeTools.WriteKVPs(stream, Controllers.Select(controller => new KeyValuePair<string, string>(controller.Name.Value, controller.Factory.Name)));
                }

                foreach (var controller in Controllers) {
                    using (var stream = zip.CreateEntry($"controllers/{controller.Name}").Open()) {
                        controller.Factory.Save(stream, controller, this);
                    }
                }

                foreach (var screen in Screens) {
                    using (var stream = zip.CreateEntry($"screens/{screen.Name}").Open()) {
                        screen.Save(stream);
                    }
                }
            }
        }

        void InitializeBrain() {
            brain.InsertCog(new NotePerceptualCog());
            //brain.InsertCog(new NoteLayoutPerceptualCog());
            //brain.InsertCog(new ChordLayoutPerceptualCog());
            brain.InsertCog(new MeasureLayoutPerceptualCog());
        }
    }
}
