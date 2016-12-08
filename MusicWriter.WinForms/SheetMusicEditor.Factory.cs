using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace MusicWriter.WinForms {
    partial class SheetMusicEditor {
        public sealed class FactoryClass : ITrackControllerFactory<Control> {
            public static ITrackControllerFactory<Control> Instance { get; } =
                new FactoryClass();

            public string Name {
                get { return "Sheet Music Editor"; }
            }

            private FactoryClass() {
            }
            
            public ITrackController<Control> Create(EditorFile<Control> file) {
                var controller =
                    new SheetMusicEditor();

                controller.File = file;

                return controller;
            }

            public ITrackController<Control> Load(
                    Stream stream,
                    EditorFile<Control> file
                ) {
                var controller =
                    new SheetMusicEditor();

                controller.File = file;

                var xdoc =
                    XDocument.Load(stream);

                var xroot =
                    xdoc.Element("sheet-music-editor");

                var tracks =
                    xroot
                        .Element("tracks")
                        .Elements("track")
                        .Select(
                                xtrack =>
                                    file[xtrack.Value]
                            );
                
                foreach (var track in tracks)
                    controller.tracks.Add(track);

                var xcursor =
                    xroot.Element("cursor");
                
                controller.MusicCursor.Tone = new SemiTone(int.Parse(xcursor.Attribute("tone").Value));
                controller.MusicCursor.Caret.Side = (Caret.FocusSide)Enum.Parse(typeof(Caret.FocusSide), xcursor.Attribute("focus").Value);
                var cursor_duration = CodeTools.ReadDuration(xcursor.Attribute("duration").Value);
                controller.MusicCursor.Caret.Duration.Start = cursor_duration.Start;
                controller.MusicCursor.Caret.Duration.Length = cursor_duration.Length;
                controller.MusicCursor.Caret.Unit = CodeTools.ReadTime(xcursor.Attribute("unit").Value);

                var xpin =
                    xroot.Element("pin");

                controller.Pin.PinMode = (PinMode)Enum.Parse(typeof(PinMode), xpin.Attribute("mode").Value);
                controller.Pin.Time.GroupName.Value = xpin.Attribute("name").Value;
                controller.Pin.Time.Offset.Value = CodeTools.ReadTime(xpin.Attribute("offset").Value);

                var xstate =
                    xroot.Element("state");

                var activetrackname =
                    xstate.Attribute("active-track").Value;

                controller.activetrack_index = controller.tracks.SpecialCollection.IndexOf((MusicTrack)file[activetrackname]);

                return controller;
            }

            public void Save(
                    Stream stream,
                    ITrackController<Control> controller,
                    EditorFile<Control> file
                ) {
                var editor = controller as SheetMusicEditor;

                using (var xwriter = XmlWriter.Create(stream)) {
                    xwriter.WriteStartDocument();

                    xwriter.WriteStartElement("sheet-music-editor");

                    xwriter.WriteStartElement("cursor");
                    xwriter.WriteAttributeString("tone", editor.MusicCursor.Tone.Semitones.ToString());
                    xwriter.WriteAttributeString("duration", CodeTools.WriteDuration(editor.MusicCursor.Caret.Duration));
                    xwriter.WriteAttributeString("unit", CodeTools.WriteTime(editor.MusicCursor.Caret.Unit));
                    xwriter.WriteAttributeString("focus", editor.MusicCursor.Caret.Side.ToString());
                    xwriter.WriteEndElement(); // cursor
                    
                    xwriter.WriteStartElement("pin");
                    xwriter.WriteAttributeString("mode", editor.Pin.PinMode.ToString());
                    xwriter.WriteAttributeString("name", editor.Pin.Time.GroupName.Value);
                    xwriter.WriteAttributeString("offset", CodeTools.WriteTime(editor.Pin.Time.Offset.Value));
                    xwriter.WriteEndElement(); // pin

                    xwriter.WriteStartElement("tracks");

                    foreach (var track in editor.tracks) {
                        xwriter.WriteStartElement("track");

                        xwriter.WriteValue(track.Name.Value);

                        xwriter.WriteEndElement(); // track
                    }

                    xwriter.WriteEndElement(); // tracks

                    xwriter.WriteStartElement("state");
                    xwriter.WriteAttributeString("active-track", editor.ActiveTrack.Name.Value);
                    xwriter.WriteEndElement(); // state

                    xwriter.WriteEndElement(); // sheet-music-editor

                    xwriter.WriteEndDocument();
                }
            }
        }
    }
}
