using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class PropertyBinder<T> : IDisposable
    {
        readonly IStorageObject storageobject;
        readonly ObservableProperty<T> property;
        readonly Func<IStorageObject, T> deserializer;
        readonly Action<IStorageObject, T> serializer;
        readonly IOListener listener;

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

            listener = storageobject.CreateListen(IOEvent.ObjectContentsSet, StorageObject_ContentsSet);
            property.AfterChange += Property_AfterChange;

            storageobject.Graph.Listeners.Add(listener);
        }

        private void StorageObject_ContentsSet(StorageObjectID affected) =>
            property.Value = deserializer(storageobject);

        private void Property_AfterChange(T old, T @new) =>
            serializer(storageobject, @new);

        bool disposed = false;
        public void Dispose() {
            if (!disposed) {
                disposed = true;

                storageobject.Graph.Listeners.Remove(listener);
                property.AfterChange -= Property_AfterChange;
            }
        }
    }
}
