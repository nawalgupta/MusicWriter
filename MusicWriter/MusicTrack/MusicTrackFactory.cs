using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static MusicWriter.TimeSignature;

namespace MusicWriter {
    public sealed class MusicTrackFactory : ITrackFactory {
        public static ITrackFactory Instance { get; } =
            new MusicTrackFactory();

        public string Name {
            get { return "Music Track"; }
        }

        private MusicTrackFactory() {
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

        public ITrack Load(Stream stream) {
            var melody =
                new MelodyTrack();

            var rhythm =
                new RhythmTrack();

            var adornment =
                new AdornmentTrack();

            var xdoc =
                XDocument.Load(stream);

            var xroot =
                xdoc.Element("music-track");

            var xrhythm =
                xroot.Element("rhythm");

            var timesigs =
                xrhythm
                    .Element("time-sigs")
                    .Elements("time-sig")
                    .Select(
                            xtimesig =>
                                new DuratedItem<TimeSignature> {
                                    Duration = CodeTools.ReadDuration(xtimesig.Attribute("duration").Value),
                                    Value =
                                        new TimeSignature(
                                                xtimesig
                                                    .Elements("simple")
                                                    .Select(
                                                            xsimple =>
                                                                new Simple(
                                                                        int.Parse(xsimple.Attribute("upper").Value),
                                                                        int.Parse(xsimple.Attribute("lower").Value)
                                                                    )
                                                        )
                                                    .ToArray()
                                            )
                                }
                        );

            foreach (var timesig in timesigs)
                rhythm
                    .TimeSignatures
                    .Add(timesig.Value, timesig.Duration);

            var metersigs =
                xrhythm
                    .Element("meter-sigs")
                    .Elements("meter-sig")
                    .Select(
                            xmetersig =>
                                new DuratedItem<MeterSignature> {
                                    Duration = CodeTools.ReadDuration(xmetersig.Attribute("duration").Value),
                                    Value =
                                        new MeterSignature(
                                                CodeTools.ReadDuration(xmetersig.Attribute("duration").Value).Length,
                                                xmetersig
                                                    .Elements("cells")
                                                    .Select(
                                                            xcell =>
                                                                new Cell {
                                                                    Length = CodeTools.ReadTime(xcell.Attribute("length").Value),
                                                                    Stress = int.Parse(xcell.Attribute("stress").Value)
                                                                }
                                                        )
                                                    .ToArray()
                                            )
                                }
                        );

            foreach (var metersig in metersigs)
                rhythm
                    .MeterSignatures
                    .Add(metersig.Value, metersig.Duration);

            var notes =
                xroot
                    .Element("melody")
                    .Elements("note")
                    .Select(
                            xnote =>
                                new Note(
                                        new NoteID(int.Parse(xnote.Attribute("id").Value)),
                                        CodeTools.ReadDuration(xnote.Attribute("duration").Value),
                                        new SemiTone(int.Parse(xnote.Attribute("tone").Value))
                                    )
                        );

            foreach (var note in notes)
                melody.AddNote(note);

            var memory =
                new PerceptualMemory();
            
            var propertygraphlet =
                new ExplicitPropertyGraphlet<NoteID>();

            var track =
                new MusicTrack(
                        melody,
                        rhythm,
                        adornment,
                        memory,
                        propertygraphlet
                    );

            memory.InsertMemoryModule(new EditableMemoryModule<NoteLayout>());
            memory.InsertMemoryModule(new EditableMemoryModule<ChordLayout>());
            memory.InsertMemoryModule(new EditableMemoryModule<MeasureLayout>());
            memory.InsertMemoryModule(new IgnorantMemoryModule<Cell>(track.Rhythm));
            memory.InsertMemoryModule(new IgnorantMemoryModule<Simple>(track.Rhythm));
            memory.InsertMemoryModule(new IgnorantMemoryModule<Measure>(track.Rhythm));
            memory.InsertMemoryModule(new IgnorantMemoryModule<Note>(track.Melody));
            memory.InsertMemoryModule(new IgnorantMemoryModule<KeySignature>(track.Adornment.KeySignatures));
            memory.InsertMemoryModule(new IgnorantMemoryModule<Staff>(track.Adornment.Staffs));
            memory.InsertMemoryModule(new NotePerceptualCog.MemoryModule());

            return track;
        }

        public void Save(ITrack track, Stream stream) {
            var music = track as MusicTrack;

            using (var xwriter = XmlWriter.Create(stream)) {
                xwriter.WriteStartDocument();
                xwriter.WriteStartElement("music-track");

                xwriter.WriteStartElement("rhythm");

                xwriter.WriteStartElement("time-sigs");

                foreach (var timesig in music.Rhythm.TimeSignatures.AllItems) {
                    xwriter.WriteStartElement("time-sig");

                    xwriter.WriteAttributeString("duration", CodeTools.WriteDuration(timesig.Duration));

                    foreach (var simple in timesig.Value.Simples) {
                        xwriter.WriteStartElement("simple");

                        xwriter.WriteAttributeString("upper", simple.Upper.ToString());
                        xwriter.WriteAttributeString("lower", simple.Lower.ToString());

                        xwriter.WriteEndElement(); // simple
                    }

                    xwriter.WriteEndElement(); // time-sig
                }

                xwriter.WriteEndElement(); // time-sigs

                xwriter.WriteStartElement("meter-sigs");

                foreach(var metersig in music.Rhythm.MeterSignatures.AllItems) {
                    xwriter.WriteStartElement("meter-sig");

                    xwriter.WriteAttributeString("duration", CodeTools.WriteDuration(metersig.Duration));
                    
                    foreach(var cell in metersig.Value.Cells) {
                        xwriter.WriteStartElement("cell");

                        xwriter.WriteAttributeString("stress", cell.Stress.ToString());
                        xwriter.WriteAttributeString("length", CodeTools.WriteTime(cell.Length));

                        xwriter.WriteEndElement(); // cell
                    }

                    xwriter.WriteEndElement(); // meter-sig
                }

                xwriter.WriteEndElement(); // meter-sigs

                xwriter.WriteEndElement(); // rhythm

                xwriter.WriteStartElement("melody");

                foreach(var note in music.Melody.AllNotes()) {
                    xwriter.WriteStartElement("note");

                    xwriter.WriteAttributeString("id", note.ID.ToString());
                    xwriter.WriteAttributeString("duration", CodeTools.WriteDuration(note.Duration));
                    xwriter.WriteAttributeString("tone", note.Tone.Semitones.ToString());

                    xwriter.WriteEndElement(); // note
                }

                xwriter.WriteEndElement(); // melody

                xwriter.WriteEndElement(); // music-track
                xwriter.WriteEndDocument();
            }
        }
    }
}
