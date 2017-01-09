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

            public void Init(
                    IStorageObject storage,
                    EditorFile<Control> file
                ) {
            }

            public ITrackController<Control> Load(
                    IStorageObject storage,
                    EditorFile<Control> file
                ) {
                var controller =
                    new SheetMusicEditor();

                controller.File = file;
                controller.Storage = storage;

                return controller;
            }
        }
    }
}
