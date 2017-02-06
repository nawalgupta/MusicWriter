using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IBoundObject<T> 
        where T : IBoundObject<T>
    {
        StorageObjectID StorageObjectID { get; }

        ObservableProperty<string> Name { get; }

        EditorFile File { get; }

        IFactory<T> Factory { get; }

        void Unbind();
    }
}
