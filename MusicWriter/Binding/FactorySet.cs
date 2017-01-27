﻿using System;
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

            file.Storage[hub_objID].Add("", storageobject.ID);

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

            var value = factory.Load(storageobjectID, file);

            value.Name.Bind(obj.GetOrMake("name"));

            return value;
        }
    }
}
