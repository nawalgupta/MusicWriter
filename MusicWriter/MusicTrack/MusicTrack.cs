using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class MusicTrack : ITrack {
        readonly MelodyTrack melody;
        readonly RhythmTrack rhythm;
        readonly AdornmentTrack adornment;
        readonly PerceptualMemory memory;
        readonly IPropertyGraphlet<NoteID> propertygraphlet;

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

        public ObservableProperty<string> Name { get; } =
            new ObservableProperty<string>("");

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
            // Key signatures, time signatures, and meters aren't removed
            // becasue they aren't events - they are completely fields.
            // Only notes are erased.

            foreach (var note in melody.Intersecting(window))
                melody.DeleteNote(note.Value);
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
                rhythm.SetTimeSignature(signature.Value, signature.Duration + insert);

            foreach (var signature in clipboard.MeterSignatures)
                rhythm.SetMeter(signature.Value, signature.Duration + insert);

            foreach (var signature in clipboard.KeySignatures)
                adornment.SetKeySignature(signature.Value, signature.Duration + insert);

            foreach (var staff in clipboard.Staffs)
                adornment.SetStaff(staff.Value, staff.Duration + insert);

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
