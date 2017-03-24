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
        readonly bool includefactoryargument;
        readonly object[] extraarguments;

        public string Name {
            get { return name; }
        }

        public bool IncludeFactoryArgument {
            get { return includefactoryargument; }
        }

        public object[] ExtraArguments {
            get { return extraarguments; }
        }

        public CtorFactory(
                string name,
                bool includefactoryargument,
                params object[] extraarguments
            ) {
            this.name = name;
            this.includefactoryargument = includefactoryargument;
            this.extraarguments = extraarguments;

            ctorinfo =
                typeof(T2)
                    .GetConstructor(
                            (includefactoryargument ?
                                new Type[] {
                                        typeof(StorageObjectID),
                                        typeof(EditorFile),
                                        typeof(IFactory<T>)
                                    } :
                                new Type[] {
                                        typeof(StorageObjectID),
                                        typeof(EditorFile)
                                    })
                                .Concat(extraarguments.Select(arg => arg.GetType())).ToArray()
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
            (T)
            ctorinfo
                .Invoke(
                        (includefactoryargument ?
                            new object[] {
                                    storageobjectID,
                                    file,
                                    this
                                } :
                            new object[] {
                                    storageobjectID,
                                    file
                                })
                            .Concat(extraarguments)
                            .ToArray()
                    );
    }
}
