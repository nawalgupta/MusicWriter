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
        readonly InputController inputcontroller;

        public ObservableProperty<string> Name { get; } =
            new ObservableProperty<string>("");

        public FileCapabilities<View> Capabilities {
            get { return capabilities; }
        }
        
        public EditorFile File {
            get { return file; }
        }

        public InputController InputController {
            get { return inputcontroller; }
        }

        public ObservableCollection<ITrackController<View>> Controllers { get; } =
            new ObservableCollection<ITrackController<View>>();

        public Screen(
                FileCapabilities<View> capabilities,
                EditorFile file,
                InputController inputcontroller
            ) {
            this.capabilities = capabilities;
            this.file = file;
            this.inputcontroller = inputcontroller;

            Controllers.CollectionChanged += Controllers_CollectionChanged;
        }

        private void Controllers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            foreach (ITrackController<View> newitem in e.NewItems) {
                newitem.InputController = InputController;
            }
        }
    }
}
