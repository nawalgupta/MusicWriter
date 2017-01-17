using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicWriter.TimeSignature;

namespace MusicWriter {
    public sealed class MusicTrack : ITrack {
        readonly IStorageObject storage;
        readonly TrackSettings settings;

        readonly MelodyTrack melody;
        readonly RhythmTrack rhythm;
        readonly AdornmentTrack adornment;
        readonly PerceptualMemory memory;
        readonly IPropertyGraphlet<NoteID> propertygraphlet;
        readonly PropertyManager propertymanager;
        readonly PolylineFunction tempo;

        public event FieldChangedDelegate Dirtied;

        public StorageObjectID StorageObjectID {
            get { return storage.ID; }
        }

        public TrackSettings Settings {
            get { return settings; }
        }

        public MelodyTrack Melody {
            get { return melody; }
        }

        public RhythmTrack Rhythm {
            get { return rhythm; }
        }

        public AdornmentTrack Adornment {
            get { return adornment; }
        }

        public PerceptualMemory Memory {
            get { return memory; }
        }

        public IPropertyGraphlet<NoteID> PropertyGraphlet {
            get { return propertygraphlet; }
        }

        public PropertyManager PropertyManager {
            get { return propertymanager; }
        }

        public PolylineFunction Tempo {
            get { return tempo; }
        }

        public ITrackFactory Factory {
            get { return MusicTrackFactory.Instance; }
        }

        public ObservableProperty<string> Name { get; } =
            new ObservableProperty<string>("");

        public ObservableProperty<Time> Length {
            get { return melody.Length; }
        }

        public MusicTrack(
                IStorageObject storage,
                TrackSettings settings
            ) {
            melody = new MelodyTrack(storage.GetOrMake("melody"));
            rhythm = new RhythmTrack(storage.GetOrMake("rhythm"));
            adornment = new AdornmentTrack(storage.GetOrMake("adornment"));
            memory = new PerceptualMemory();
            propertygraphlet = new StoragePropertyGraphlet<NoteID>(storage, propertymanager);
            propertymanager = settings.PropertyManager;
            tempo = new PolylineFunction(storage.GetOrMake("tempo"), 120f);

            this.storage = storage;
            this.settings = settings;

            melody.FieldChanged += Melody_FieldChanged;
            rhythm.MeterSignatures.FieldChanged += MeterSignatures_FieldChanged;
            rhythm.TimeSignatures.FieldChanged += TimeSignatures_FieldChanged;
            adornment.KeySignatures.FieldChanged += KeySignatures_FieldChanged;
            adornment.Staffs.FieldChanged += Staffs_FieldChanged;

            if (!storage.HasChild("state") || storage.Get("state").ReadAllString() != "inited") {
                Init();
                storage.GetOrMake("state").WriteAllString("inited");
            }

            Init_temp();
        }

        private void Melody_FieldChanged(Duration delta) =>
            Dirtied?.Invoke(delta);

        private void MeterSignatures_FieldChanged(Duration delta) =>
            Dirtied?.Invoke(delta);

        private void TimeSignatures_FieldChanged(Duration delta) =>
            Dirtied?.Invoke(delta);

        private void KeySignatures_FieldChanged(Duration delta) =>
            Dirtied?.Invoke(delta);

        private void Staffs_FieldChanged(Duration delta) =>
            Dirtied?.Invoke(delta);

        void Init() {
            Rhythm.TimeSignatures.ScootAndOverwrite(new TimeSignature(new Simple(4, 4)), Duration.Eternity);
            Rhythm.MeterSignatures.ScootAndOverwrite(MeterSignature.Default(Rhythm.TimeSignaturesInTime(Duration.Eternity).Single().Value.Simples[0]), Duration.Eternity);
            Adornment.Staffs.ScootAndOverwrite(Staff.Treble, Duration.Eternity);
            Adornment.KeySignatures.ScootAndOverwrite(KeySignature.Create(DiatonicToneClass.C, PitchTransform.Natural, Mode.Major), Duration.Eternity);
        }

        void Init_temp() { 
            Memory.InsertMemoryModule(new EditableMemoryModule<NoteLayout>());
            Memory.InsertMemoryModule(new EditableMemoryModule<ChordLayout>());
            Memory.InsertMemoryModule(new EditableMemoryModule<MeasureLayout>());
            Memory.InsertMemoryModule(new IgnorantMemoryModule<Cell>(Rhythm));
            Memory.InsertMemoryModule(new IgnorantMemoryModule<Simple>(Rhythm));
            Memory.InsertMemoryModule(new IgnorantMemoryModule<Measure>(Rhythm));
            Memory.InsertMemoryModule(new IgnorantMemoryModule<Note>(Melody));
            Memory.InsertMemoryModule(new IgnorantMemoryModule<KeySignature>(Adornment.KeySignatures));
            Memory.InsertMemoryModule(new IgnorantMemoryModule<Staff>(Adornment.Staffs));
            Memory.InsertMemoryModule(new NotePerceptualCog.MemoryModule());
        }

        private class ClipboardData {
            public Time Length;
            public Note[] Notes;
            public IDuratedItem<TimeSignature>[] TimeSignatures;
            public IDuratedItem<KeySignature>[] KeySignatures;
            public IDuratedItem<MeterSignature>[] MeterSignatures;
            public IDuratedItem<Staff>[] Staffs;
            public KeyValuePair<NoteID, KeyValuePair<Property, object>[]>[] PropertyGraphletData;
        }

        public void Erase(Duration window) {
            // Erasing just applies to notes - discreet events

            foreach (Note note in melody.Intersecting(window).ToArray()) {
                var subtractedtime =
                    note
                        .Duration
                        .Subtract(window)
                        .Select(
                                duration =>
                                    duration.Start > window.Start ?
                                        duration - window.Start :
                                        duration
                            )
                        .ToArray();

                if (subtractedtime.Length == 0)
                    melody.DeleteNote(note.ID);
                else {
                    melody.UpdateNote(note.ID, subtractedtime[0], note.Tone);

                    for (int i = 1; i < subtractedtime.Length; i++)
                        melody.AddNote(note.Tone, subtractedtime[i]);
                }
            }
        }

        public void Delete(Duration window) {
            // Deleting applies to everything
            var beyond =
                new Duration {
                    Start = window.End,
                    End = Time.Eternity
                };

            var throughandbeyond =
                new Duration {
                    Start = window.Start,
                    End = Time.Eternity
                };

            foreach (Note note in melody.Intersecting(throughandbeyond).ToArray()) {
                if (note.Duration.IsInside(window))
                    melody.DeleteNote(note.ID);
                else {
                    var subtractedtime =
                        note.Duration - window;
                    
                    melody.UpdateNote(note.ID, subtractedtime, note.Tone);
                }
            }

            rhythm.TimeSignatures.DeleteTime(window);
            rhythm.MeterSignatures.DeleteTime(window);

            adornment.Staffs.DeleteTime(window);
            adornment.KeySignatures.DeleteTime(window);
        }

        public object Copy(Duration window) =>
            new ClipboardData {
                Length = window.Length,
                Notes =
                    melody
                        .Intersecting(window)
                        .Select(item => {
                            var note = item.Value;

                            var cloned =
                                new Note(
                                        note.ID,
                                        note.Duration.Intersection(window) - window.Start,
                                        note.Tone
                                    );

                            return cloned;
                        })
                        .ToArray(),
                KeySignatures =
                    Adornment
                        .KeySignatures
                        .Intersecting(window)
                        .Select(
                                item =>
                                    new DuratedItem<KeySignature> {
                                        Value = item.Value,
                                        Duration = item.Duration.Intersection(window) - window.Start
                                    }
                            )
                        .ToArray(),
                TimeSignatures =
                    Rhythm
                        .TimeSignatures
                        .Intersecting(window)
                        .Select(
                                item =>
                                    new DuratedItem<TimeSignature> {
                                        Value = item.Value,
                                        Duration = item.Duration.Intersection(window) - window.Start
                                    }
                            )
                        .ToArray(),
                MeterSignatures =
                    Rhythm
                        .MeterSignatures
                        .Intersecting(window)
                        .Select(
                                item =>
                                    new DuratedItem<MeterSignature> {
                                        Value = item.Value,
                                        Duration = item.Duration.Intersection(window) - window.Start
                                    }
                            )
                        .ToArray(),
                Staffs =
                    Adornment
                        .Staffs
                        .Intersecting(window)
                        .Select(
                                item =>
                                    new DuratedItem<Staff> {
                                        Value = item.Value,
                                        Duration = item.Duration.Intersection(window) - window.Start
                                    }
                            )
                        .ToArray(),
                PropertyGraphletData =
                    PropertyGraphlet
                        .Extract(
                                melody
                                    .Intersecting(window)
                                    .Select(item => item.Value.ID)
                                    .ToArray()
                            )
                        .ToArray()
            };

        public void Paste(object data, Time insert) {
            var noteID_translations =
                new Dictionary<NoteID, NoteID>();

            var clipboard =
                data as ClipboardData;

            foreach (var note in clipboard.Notes)
                noteID_translations.Add(
                        note.ID,
                        melody
                            .AddNote(
                                    note.Tone,
                                    note.Duration + insert
                                )
                    );

            foreach (var signature in clipboard.TimeSignatures)
                rhythm.TimeSignatures.ScootAndOverwrite(signature.Value, signature.Duration + insert);

            foreach (var signature in clipboard.MeterSignatures)
                rhythm.MeterSignatures.ScootAndOverwrite(signature.Value, signature.Duration + insert);

            foreach (var signature in clipboard.KeySignatures)
                adornment.KeySignatures.ScootAndOverwrite(signature.Value, signature.Duration + insert);

            foreach (var staff in clipboard.Staffs)
                adornment.Staffs.ScootAndOverwrite(staff.Value, staff.Duration + insert);

            var translated_propertygraphletdata =
                clipboard
                    .PropertyGraphletData
                    .Select(
                            kvp =>
                                new KeyValuePair<NoteID, KeyValuePair<Property, object>[]>(
                                        noteID_translations[kvp.Key],
                                        kvp.Value
                                    )
                        );

            propertygraphlet.Inject(translated_propertygraphletdata);
        }
    }
}
