using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class FileCapabilities<TView> {
        readonly ObservableCollection<ITrackFactory> trackfactories =
            new ObservableCollection<ITrackFactory>();

        readonly ObservableCollection<ITrackControllerFactory<TView>> controllerfactories =
            new ObservableCollection<ITrackControllerFactory<TView>>();

		readonly ObservableCollection<IPorter> porters =
			new ObservableCollection<IPorter>();

        public ObservableCollection<ITrackFactory> TrackFactories {
            get { return trackfactories; }
        }

        public ObservableCollection<ITrackControllerFactory<TView>> ControllerFactories {
            get { return controllerfactories; }
        }

		public ObservableCollection<IPorter> Porters {
			get { return porters; }
		}
    }
}
