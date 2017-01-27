using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FuncFactory<T> : IFactory<T> where T : IBoundObject<T>
    {
        readonly string name;
        readonly InitDelegate initer;
        readonly LoadDelegate loader;

        public delegate void InitDelegate_truncated(StorageObjectID storageobjectID, EditorFile file);
        public delegate T LoadDelegate_truncated(StorageObjectID storageobjectID, EditorFile file);

        public delegate void InitDelegate(StorageObjectID storageobjectID, EditorFile file, IFactory<T> factory);
        public delegate T LoadDelegate(StorageObjectID storageobjectID, EditorFile file, IFactory<T> factory);

        public string Name {
            get { return name; }
        }

        public InitDelegate Initer {
            get { return initer; }
        }

        public LoadDelegate Loader {
            get { return loader; }
        }

        public FuncFactory(
                string name,
                InitDelegate_truncated initer,
                LoadDelegate_truncated loader
            ) :
            this(
                    name,
                    (storageobjectID, file, fac) => initer(storageobjectID, file),
                    (storageobjectID, file, fac) => initer(storageobjectID, file)
                ) {
        }

        public FuncFactory(
                string name,
                InitDelegate initer,
                LoadDelegate loader
            ) {
            this.name = name;
            this.initer = initer;
            this.loader = loader;
        }

        public void Init(
                StorageObjectID storageobjectID, 
                EditorFile file
            ) =>
            initer(
                    storageobjectID,
                    file,
                    this
                );

        public T Load(
                StorageObjectID storageobjectID,
                EditorFile file
            ) =>
            loader(
                    storageobjectID,
                    file,
                    this
                );
    }
}
