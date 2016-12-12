using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicWriter.TimeSignature;

namespace MusicWriter {
    public sealed class MusicTrack : ITrack {
        readonly MelodyTrack melody;
        readonly RhythmTrack rhythm;
        readonly AdornmentTrack adornment;
        readonly PerceptualMemory memory;
        readonly IPropertyGraphlet<NoteID> propertygraphlet;

        public event Action Dirtied;

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

        public ITrackFactory Factory {
            get { return MusicTrackFactory.Instance; }
        }

        public ObservableProperty<string> Name { get; } =
            new ObservableProperty<string>("");

        public ObservableProperty<Time> Length { get; } =
            new ObservableProperty<Time>(Time.Zero);

        public MusicTrack(
                MelodyTrack melody,
                RhythmTrack rhythm,
                AdornmentTrack adornment,
                PerceptualMemory memory,
                IPropertyGraphlet<NoteID> propertygraphlet
            ) {
            this.melody = melody;
            this.rhythm = rhythm;
            this.adornment = adornment;
            this.memory = memory;
            this.propertygraphlet = propertygraphlet;

            melody.FieldChanged += Update;
		}

		public static MusicTrack Create() {
			var track =
				new MusicTrack(
						new MelodyTrack(),
						new RhythmTrack(),
						new AdornmentTrack(),
						new PerceptualMemory(),
						new ExplicitPropertyGraphlet<NoteID>()
					);

			track.Rhythm.TimeSignatures.ScootAndOverwrite(new TimeSignature(new Simple(4, 4)), Duration.Eternity);
			track.Rhythm.MeterSignatures.ScootAndOverwrite(MeterSignature.Default(track.Rhythm.TimeSignaturesInTime(Duration.Eternity).Single().Value.Simples[0]), Duration.Eternity);
			track.Adornment.Staffs.ScootAndOverwrite(Staff.Treble, Duration.Eternity);
			track.Adornment.KeySignatures.ScootAndOverwrite(KeySignature.Create(DiatonicToneClass.C, PitchTransform.Natural, Mode.Major), Duration.Eternity);

			track.Memory.InsertMemoryModule(new EditableMemoryModule<NoteLayout>());
			track.Memory.InsertMemoryModule(new EditableMemoryModule<ChordLayout>());
			track.Memory.InsertMemoryModule(new EditableMemoryModule<MeasureLayout>());
			track.Memory.InsertMemoryModule(new IgnorantMemoryModule<Cell>(track.Rhythm));
			track.Memory.InsertMemoryModule(new IgnorantMemoryModule<Simple>(track.Rhythm));
			track.Memory.InsertMemoryModule(new IgnorantMemoryModule<Measure>(track.Rhythm));
			track.Memory.InsertMemoryModule(new IgnorantMemoryModule<Note>(track.Melody));
			track.Memory.InsertMemoryModule(new IgnorantMemoryModule<KeySignature>(track.Adornment.KeySignatures));
			track.Memory.InsertMemoryModule(new IgnorantMemoryModule<Staff>(track.Adornment.Staffs));
			track.Memory.InsertMemoryModule(new NotePerceptualCog.MemoryModule());

			return track;
		}

		void Update() {
            var end =
                melody
                    .AllNotes()
                    .Select(note => note.Duration.End)
                    .Aggregate(
                            Time.Zero,
                            Time.Max
                        );

            end =
                rhythm
                    .TimeSignatures
                    .Intersecting_children(end)
                    .First()
                    .Duration
                    .End;

            if (end != Length.Value) {
                Length.Value = end;
                Dirtied?.Invoke();
            }
        }

        private class ClipboardData {
            public Time Length;
            public Note[] Notes;
            public IDuratedItem<TimeSignature>[] TimeSignatures;
            public IDuratedItem<KeySignature>[] KeySignatures;
            public IDuratedItem<MeterSignature>[] MeterSignatures;
            public IDuratedItem<Staff>[] Staffs;
            public KeyValuePair<NoteID, KeyValuePair<Type, object>[]>[] PropertyGraphletData;
        }

        public void Erase(Duration window) {
            // Erasing just applies to notes - discreet events

            foreach (Note note in melody.Intersecting(window).ToArray()) {
                var subtractedtime =
                    note.Duration.Subtract_Time(window);

                if (subtractedtime == null)
                    melody.DeleteNote(note.ID);
                else {
                    melody.UpdateNote(note.ID, subtractedtime, note.Tone);
                }
            }
        }

        public void Delete(Duration window) {
            // Deleting applies to everything
            Erase(window);

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

            foreach (Note note in melody.Intersecting(beyond).ToArray()) {
                var subtractedtime =
                    note.Duration - window.Length;

                melody.UpdateNote(note.ID, subtractedtime, note.Tone);
            }

            foreach (var timesig in rhythm.TimeSignatures.Intersecting(throughandbeyond).ToArray()) {
                var subtractedtime =
                    timesig.Duration.Subtract_Time(window);

                rhythm.TimeSignatures.Remove(timesig);

                if (subtractedtime != null)
                    rhythm.TimeSignatures.Add(timesig.Value, subtractedtime);
            }

            foreach (var metersig in rhythm.MeterSignatures.Intersecting(throughandbeyond).ToArray()) {
                var subtractedtime =
                    metersig.Duration.Subtract_Time(window);

                rhythm.MeterSignatures.Remove(metersig);

                if (subtractedtime != null)
                    rhythm.MeterSignatures.Add(metersig.Value, subtractedtime);
            }

            foreach (var staff in adornment.Staffs.Intersecting(throughandbeyond).ToArray()) {
                var subtractedtime =
                    staff.Duration.Subtract_Time(window);

                adornment.Staffs.Remove(staff);

                if (subtractedtime != null)
                    adornment.Staffs.Add(staff.Value, subtractedtime);
            }

            foreach (var keysig in adornment.KeySignatures.Intersecting(throughandbeyond).ToArray()) {
                var subtractedtime =
                    keysig.Duration.Subtract_Time(window);

                adornment.KeySignatures.Remove(keysig);

                if (subtractedtime != null)
                    adornment.KeySignatures.Add(keysig.Value, subtractedtime);
            }
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
                            .ID
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
                                new KeyValuePair<NoteID, KeyValuePair<Type, object>[]>(
                                        noteID_translations[kvp.Key],
                                        kvp.Value
                                    )
                        );

            propertygraphlet.Inject(translated_propertygraphletdata);
        }
    }
}
