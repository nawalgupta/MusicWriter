using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class PropertyBinder<T>
    {
        readonly IStorageObject storageobject;
        readonly ObservableProperty<T> property;
        readonly Func<IStorageObject, T> deserializer;
        readonly Action<IStorageObject, T> serializer;

        public IStorageObject StorageObject {
            get { return storageobject; }
        }

        public ObservableProperty<T> Property {
            get { return property; }
        }

        public Func<IStorageObject, T> Deserializer {
            get { return deserializer; }
        }

        public Action<IStorageObject, T> Serializer {
            get { return serializer; }
        }

        public PropertyBinder(
                IStorageObject storageobject,
                ObservableProperty<T> property,
                Func<IStorageObject, T> deserializer,
                Action<IStorageObject, T> serializer
            ) {
            this.storageobject = storageobject;
            this.property = property;
            this.deserializer = deserializer;
            this.serializer = serializer;

            storageobject.ContentsSet += StorageObject_ContentsSet;
            property.AfterChange += Property_AfterChange;
        }

        private void StorageObject_ContentsSet(StorageObjectID affected) =>
            property.Value = deserializer(storageobject);

        private void Property_AfterChange(T old, T @new) =>
            serializer(storageobject, @new);
    }
}
