using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IScreenFactory<View>
    {
        string Name { get; }

        void Init(StorageObjectID storageobjectID, EditorFile<View> file);

        IScreen<View> Load(StorageObjectID storageobjectID, EditorFile<View> file);
    }
}
