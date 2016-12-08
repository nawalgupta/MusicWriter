using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MusicWriter {
    public sealed class Screen<View> {
        readonly EditorFile<View> file;
        readonly CommandCenter commandcenter = new CommandCenter();

        public ObservableProperty<string> Name { get; } =
            new ObservableProperty<string>("");
        
        public CommandCenter CommandCenter {
            get { return commandcenter; }
        }

        public ObservableCollection<ITrackController<View>> Controllers { get; } =
            new ObservableCollection<ITrackController<View>>();

        public Screen(EditorFile<View> file) {
            this.file = file;

            Controllers.CollectionChanged += Controllers_CollectionChanged;
        }

        private void Controllers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.NewItems != null) {
                foreach (ITrackController<View> newitem in e.NewItems) {
                    newitem.CommandCenter.SubscribeTo(commandcenter);
                }
            }

            if (e.OldItems != null) {
                foreach (ITrackController<View> olditem in e.OldItems) {
                    olditem.CommandCenter.DesubscribeFrom(commandcenter);
                }
            }
        }

        public void Load(Stream stream) {
            var xdoc =
                XDocument.Load(stream);

            var xroot =
                xdoc.Element("screen");

            Name.Value = xroot.Attribute("name").Value;

            var controllers =
                xroot
                    .Element("controllers")
                    .Elements("controller")
                    .Select(
                            xcontroller =>
                                file.GetController(xcontroller.Value)
                        );

            foreach (var controller in controllers)
                Controllers.Add(controller);
        }

        public void Save(Stream stream) {
            using (var xwriter = XmlWriter.Create(stream)) {
                xwriter.WriteStartDocument();

                xwriter.WriteStartElement("screen");

                xwriter.WriteAttributeString("name", Name.Value);

                xwriter.WriteStartElement("controllers");

                foreach(var controller in Controllers) {
                    xwriter.WriteStartElement("controller");

                    xwriter.WriteValue(controller.Name.Value);

                    xwriter.WriteEndElement(); // controller
                }

                xwriter.WriteEndElement(); // controllers

                xwriter.WriteEndElement(); // screen

                xwriter.WriteEndDocument();
            }
        }
    }
}
