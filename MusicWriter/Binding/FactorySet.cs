using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FactorySet<T>
        where T : IBoundObject<T>
    {
        public ObservableList<IFactory<T>> Factories { get; } =
            new ObservableList<IFactory<T>>();

        public FactorySet(params IFactory<T>[] factories) {
            foreach (var factory in factories)
                Factories.Add(factory);
        }

        public StorageObjectID Init(
                string type,
                StorageObjectID hub_objID,
                EditorFile file
            ) {
            var factory =
                Factories.FirstOrDefault(_ => _.Name == type);

            var storageobject =
                file.Storage.CreateObject();

            storageobject.GetOrMake("type").WriteAllString(type);
            factory.Init(storageobject.ID, file);

            return storageobject.ID;
        }

        public T Load(
                StorageObjectID storageobjectID,
                EditorFile file
            ) {
            var obj =
                file.Storage[storageobjectID];

            var type =
                obj
                    .Get("type")
                    .ReadAllString();

            var factory =
                Factories.FirstOrDefault(_ => _.Name == type);

            return factory.Load(storageobjectID, file);
        }
    }
}
