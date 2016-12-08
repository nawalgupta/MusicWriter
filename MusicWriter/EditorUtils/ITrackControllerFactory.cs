using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface ITrackControllerFactory<TView> {
        string Name { get; }

        ITrackController<TView> Create(EditorFile<TView> file);

        ITrackController<TView> Load(Stream stream, EditorFile<TView> file);

        void Save(Stream stream, ITrackController<TView> controller, EditorFile<TView> file);
    }
}
