using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public abstract class BoundObject<T> : IBoundObject<T> where T : IBoundObject<T>
    {
        readonly EditorFile file;
        readonly StorageObjectID storageobjectID;

        public abstract IFactory<T> Factory { get; }

        public EditorFile File {
            get { return file; }
        }

        public ObservableProperty<string> Name { get; } =
            new ObservableProperty<string>();

        public StorageObjectID StorageObjectID {
            get { return storageobjectID; }
        }

        public BoundObject(
                StorageObjectID storageobjectID,
                EditorFile file
            ) {
            this.storageobjectID = storageobjectID;
            this.file = file;
        }
    }
}
