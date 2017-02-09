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
        readonly IFactory<T> factory;

        public virtual IFactory<T> Factory {
            get { return factory; }
        }

        public EditorFile File {
            get { return file; }
        }

        public StorageObjectID StorageObjectID {
            get { return storageobjectID; }
        }

        public BoundObject(
                StorageObjectID storageobjectID,
                EditorFile file,
                IFactory<T> factory = null
            ) {
            this.storageobjectID = storageobjectID;
            this.file = file;
            this.factory = factory;
        }

        public virtual void Bind() {
        }

        public virtual void Unbind() {
        }
    }
}
