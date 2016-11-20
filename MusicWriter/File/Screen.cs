using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class Screen<View> {
        readonly FileCapabilities<View> capabilities;
        readonly EditorFile file;

        public ObservableProperty<string> Name { get; } =
            new ObservableProperty<string>("");

        public FileCapabilities<View> Capabilities {
            get { return capabilities; }
        }
        
        public EditorFile File {
            get { return file; }
        }

        public ObservableCollection<ITrackController<View>> Controllers { get; } =
            new ObservableCollection<ITrackController<View>>();

        public Screen(
                FileCapabilities<View> capabilities,
                EditorFile file
            ) {
            this.capabilities = capabilities;
            this.file = file;

            Controllers.CollectionChanged += Controllers_CollectionChanged;
        }

        private void Controllers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            foreach (ITrackController<View> newitem in e.NewItems) {
                // do anything that needs to be carried accross all this screens controllers here
            }
        }
    }
}
