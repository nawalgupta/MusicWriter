using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class PropertyBinder<T> 
        : BoundObject<PropertyBinder<T>>
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
            ) :
            base(
                    storageobject.ID,
                    null //TODO: abstract BoundObjects so that those that only use File.Storage can just hold that
                ) {
            this.storageobject = storageobject;
            this.property = property;
            this.deserializer = deserializer;
            this.serializer = serializer;
            
            listener =
                storageobject
                    .CreateListen(
                            IOEvent.ObjectContentsSet,
                            StorageObject_ContentsSet
                        );
        }

        public override void Bind() {
            storageobject.Graph.Listeners.Add(listener);
            property.AfterChange += Property_AfterChange;
            if (StorageObject.IsEmpty)
                serializer(StorageObject, Property.Value);

            base.Bind();
        }

        public override void Unbind() {
            storageobject.Graph.Listeners.Remove(listener);
            property.AfterChange -= Property_AfterChange;

            base.Unbind();
        }

        private void StorageObject_ContentsSet(StorageObjectID affected) =>
            property.Value = deserializer(storageobject);

        private void Property_AfterChange(T old, T @new) =>
            serializer(storageobject, @new);
    }
}
