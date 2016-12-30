using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface ITrackControllerFactory<TView> {
        string Name { get; }

        void Init(IStorageObject storage, EditorFile<TView> file);

        ITrackController<TView> Load(IStorageObject storage, EditorFile<TView> file);
    }
}
