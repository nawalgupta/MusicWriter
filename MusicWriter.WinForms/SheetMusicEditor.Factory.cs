using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms {
    partial class SheetMusicEditor {
        public sealed class Factory : ITrackControllerFactory<Control> {
            public string Name {
                get { return "Sheet Music Editor"; }
            }

            public ITrackController<Control> Create(EditorFile file) {
                var controller =
                    new SheetMusicEditor();

                controller.File = file;

                return controller;
            }
        }
    }
}
