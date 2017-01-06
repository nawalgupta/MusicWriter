using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class NamedTime
    {
        readonly TimeMarkerUnit timemarkerunit;

        public ObservableProperty<string> MarkerName { get; } = new ObservableProperty<string>();
        public ObservableProperty<Time> Offset { get; } = new ObservableProperty<Time>(Time.Zero);

        public ObservableProperty<Time> ActualTime { get; } = new ObservableProperty<Time>(Time.Zero);

        public TimeMarkerUnit TimeMarkerUnit {
            get { return timemarkerunit; }
        }

        public NamedTime(TimeMarkerUnit timemarkerunit) {
            this.timemarkerunit = timemarkerunit;

            MarkerName.AfterChange += MarkerName_AfterChange;
            Offset.AfterChange += delegate { Update(); };
        }

        ObservableProperty<Time> markertimeprop;
        private void MarkerName_AfterChange(string old, string @new) {
            if (markertimeprop != null) {
                markertimeprop.AfterChange -= Markertimeprop_AfterChange;
            }

            markertimeprop = timemarkerunit.GetMarker(@new);
            markertimeprop.AfterChange += Markertimeprop_AfterChange;

            Update();
        }

        private void Markertimeprop_AfterChange(Time old, Time @new) {
            Update();
        }

        void Update() {
            ActualTime.Value = timemarkerunit.GetTime(MarkerName.Value) + Offset.Value;
        }
    }
}
