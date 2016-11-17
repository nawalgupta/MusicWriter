using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class NewControllerFactory<T, View> : 
        ITrackControllerFactory<View> 
        where T : ITrackController<View>, new() {
        public string Name { get; set; }

        public ITrackController<View> Create() =>
            new T();
    }
}
