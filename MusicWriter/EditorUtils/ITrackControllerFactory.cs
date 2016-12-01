using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface ITrackControllerFactory<TView> {
        string Name { get; }

        ITrackController<TView> Create(EditorFile file);
    }
}
