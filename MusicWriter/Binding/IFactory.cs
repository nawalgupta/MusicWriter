using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IFactory<T>
        where T : IBoundObject<T>
    {
        string Name { get; }

        void Init(
                StorageObjectID storageobjectID,
                EditorFile file
            );

        T Load(
                StorageObjectID storageobjectID,
                EditorFile file
            );
    }
}
