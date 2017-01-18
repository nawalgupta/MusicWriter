using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IScreen<View>
    {
        StorageObjectID StorageObjectID { get; }

        IScreenFactory<View> Factory { get; }

        EditorFile<View> File { get; }
        
        CommandCenter CommandCenter { get; }

        ObservableProperty<string> Name { get; }
    }
}
