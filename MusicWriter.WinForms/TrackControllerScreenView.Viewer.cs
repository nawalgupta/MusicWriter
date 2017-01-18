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
        public sealed class Viewer : IScreenViewer<Control>
        {
            public bool IsCompatibleWithProduceOf(IScreenFactory<Control> factory) =>
                factory.Name == TrackControllerScreenFactory<Control>.Instance.Name;
            
            public Control CreateView(IScreen<Control> screen) {
                var view =
                    new TrackControllerScreenView();

                view.File = screen.File;
                view.Screen = screen as TrackControllerScreen<Control>;

                return view;
            }

            public static readonly Viewer Instance = new Viewer();
        }
    }
}
