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
    partial class SheetMusicEditorView {
        public sealed class Viewer : IViewer<ITrackController>
        {
            public bool SupportsView(string type) =>
                type == WinFormsViewer.Type;

            public bool SupportsModel(ITrackController obj) =>
                obj is SheetMusicEditor;

            public object CreateView(ITrackController obj, string type) {
                if (type != WinFormsViewer.Type)
                    throw new NotSupportedException();

                var editor =
                    obj as SheetMusicEditor;

                if(editor == null)
                    return null;

                var view =
                    new SheetMusicEditorView();
                
                view.Editor = editor;

                return view;
            }

            public static IViewer<ITrackController> Instance { get; } =
                new Viewer();
        }
    }
}
