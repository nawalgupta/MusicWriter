using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms
{
    partial class TrackControllerScreenView
    {
        public sealed class Viewer : IViewer<IScreen>
        {
            public bool IsCompatibleWithProduceOf(IFactory<IScreen> factory) =>
                factory.Name == TrackControllerScreen.ItemName;

            public bool SupportsView(string type) =>
                type == WinFormsViewer.Type;

            public object CreateView(IScreen screen, string type) {
                var view =
                    new TrackControllerScreenView();

                view.File = screen.File;
                view.Screen = screen as TrackControllerScreen;

                return view;
            }

            public static readonly Viewer Instance = new Viewer();
        }
    }
}
