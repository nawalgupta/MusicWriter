using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    partial class FunctionEditorScreenView
    {
        public sealed class Viewer : IViewer<IScreen>
        {
            public bool IsCompatibleWithProduceOf(IFactory<IScreen> factory) =>
                factory.Name == FunctionEditorScreen.ItemName;

            public bool SupportsView(string type) =>
                type == WinFormsViewer.Type;

            public object CreateView(IScreen screen, string type) {
                var view =
                    new FunctionEditorScreenView();

                view.File = screen.File;
                view.Screen = screen as FunctionEditorScreen;
                view.Setup();

                return view;
            }

            public static readonly Viewer Instance = new Viewer();
        }
    }
}
