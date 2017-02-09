using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public class NamedBoundObject<T> : 
        BoundObject<T>,
        INamedObject
        where T : INamedObject, IBoundObject<T>
    {
        public ObservableProperty<string> Name { get; } =
            new ObservableProperty<string>("");

        public NamedBoundObject(
                StorageObjectID storageobjectID, 
                EditorFile file,
                IFactory<T> factory = null
            ) : 
            base(
                    storageobjectID, 
                    file, 
                    factory
                ) {
        }
    }
}
