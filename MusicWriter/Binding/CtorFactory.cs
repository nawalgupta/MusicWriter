using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MusicWriter
{
    public class CtorFactory<T, T2> 
        : IFactory<T>
        where T : IBoundObject<T> 
        where T2 : T
    {
        readonly string name;
        readonly ConstructorInfo ctorinfo;

        public string Name {
            get { return name; }
        }

        public CtorFactory(string name) {
            this.name = name;

            ctorinfo =
                typeof(T2)
                    .GetConstructor(
                            new Type[] {
                                    typeof(StorageObjectID),
                                    typeof(EditorFile)
                                }
                        );
        }

        public virtual void Init(
                StorageObjectID storageobjectID,
                EditorFile file
            ) {
        }

        public T Load(
                StorageObjectID storageobjectID,
                EditorFile file
            ) =>
            (T)ctorinfo.Invoke(new object[] { storageobjectID, file });
    }
}
