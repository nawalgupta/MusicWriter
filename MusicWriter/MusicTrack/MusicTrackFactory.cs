using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicWriter.TimeSignature;

namespace MusicWriter {
    public sealed class MusicTrackFactory : ITrackFactory {
        public string Name {
            get { return "Music Track"; }
        }

        public ITrack Create() {
            var track =
                new MusicTrack(
                        new MelodyTrack(),
                        new RhythmTrack(),
                        new AdornmentTrack(),
                        new PerceptualMemory(),
                        new ExplicitPropertyGraphlet<NoteID>()
                    );

            track.Rhythm.SetTimeSignature(new TimeSignature(new Simple(4, 4)), Duration.Eternity);
            track.Rhythm.SetMeter(MeterSignature.Default(track.Rhythm.TimeSignaturesInTime(Duration.Eternity).Single().Value.Simples[0]), Duration.Eternity);
            track.Adornment.SetStaff(Staff.Treble, Duration.Eternity);
            track.Adornment.SetKeySignature(KeySignature.Create(DiatonicToneClass.C, PitchTransform.Natural, Mode.Major), Duration.Eternity);

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
    }
}
