using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public static class BinderExtensions
    {
        public static PropertyBinder<string> Bind(
                this ObservableProperty<string> property,
                IStorageObject storageobject
            ) =>
            Bind(
                    property,
                    storageobject,
                    StorageExtensions.ReadAllString,
                    StorageExtensions.WriteAllString
                );

        public static PropertyBinder<T> Bind<T>(
                this ObservableProperty<T> property,
                IStorageObject storageobject,
                Func<IStorageObject, T> deserializer,
                Action<IStorageObject, T> serializer
            ) =>
            new PropertyBinder<T>(
                    storageobject,
                    property,
                    deserializer,
                    serializer
                );
        
    }
}
