using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Screen<View> {
        readonly FileCapabilities<View> capabilities;

        public ObservableProperty<string> Name { get; } =
            new ObservableProperty<string>("");

        public FileCapabilities<View> Capabilities {
            get { return capabilities; }
        }

        public ObservableCollection<ITrackController<View>> Controllers { get; } =
            new ObservableCollection<ITrackController<View>>();

        public Screen(
                FileCapabilities<View> capabilities
            ) {
            this.capabilities = capabilities;
        }
    }
}
