using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class GlobalCaret
    {
        readonly TimeMarkerUnit timemarkerunit;
        string activecaret;

        public TimeMarkerUnit TimeMarkerUnit {
            get { return timemarkerunit; }
        }

        public string ActiveCaretName {
            get { return activecaret; }
        }

        public Time ActiveCaretTime {
            get { return timemarkerunit.GetTime($"${activecaret}"); }
        }

        public ObservableProperty<Time> ActiveCaretObservableTime {
            get { return timemarkerunit.GetMarker($"${activecaret}"); }
        }

        public GlobalCaret(TimeMarkerUnit timemarkerunit) {
            this.timemarkerunit = timemarkerunit;

            timemarkerunit.AddMarker("", Time.Zero, true);
            timemarkerunit.GetMarker("").AfterChange += GlobalCaret_AfterChange;
        }

        private void GlobalCaret_AfterChange(Time old, Time @new) {
            ActiveCaretObservableTime.Value = @new;
        }

        public void InitCaret(string name) {
            timemarkerunit.AddMarker($"${name}", Time.Zero);
        }

        public void DeinitCaret(string name) {
            timemarkerunit.DeleteMarker($"${name}");
        }

        public void RenameCaret(string oldname, string newname) {
            timemarkerunit.RenameMarker($"${oldname}", $"${newname}");

            if (activecaret == oldname)
                activecaret = newname;
        }

        public void Activate(string name) {
            if (activecaret != null)
                ActiveCaretObservableTime.AfterChange -= ActiveCaretObservableTime_AfterChange;

            activecaret = name;
            timemarkerunit.SetMarker("", ActiveCaretTime);

            ActiveCaretObservableTime.AfterChange += ActiveCaretObservableTime_AfterChange;
        }

        private void ActiveCaretObservableTime_AfterChange(Time old, Time @new) {
            timemarkerunit.SetMarker("", @new);
        }
    }
}
