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

        public static PropertyBinder<int> Bind(
                this ObservableProperty<int> property,
                IStorageObject storageobject
            ) =>
            Bind(
                    property,
                    storageobject,
                    Convert.ToInt32,
                    Convert.ToString
                );

        public static PropertyBinder<Time> Bind(
                this ObservableProperty<Time> property,
                IStorageObject storageobject
            ) =>
            Bind(
                    property,
                    storageobject,
                    CodeTools.ReadTime,
                    CodeTools.WriteTime
                );

        public static PropertyBinder<Duration> Bind(
                this ObservableProperty<Duration> property,
                IStorageObject storageobject
            ) =>
            Bind(
                    property,
                    storageobject,
                    CodeTools.ReadDuration,
                    CodeTools.WriteDuration
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

        public static PropertyBinder<T> Bind<T>(
                this ObservableProperty<T> property,
                IStorageObject storageobject,
                Func<string, T> deserializer,
                Func<T, string> serializer
            ) =>
            Bind(
                    property,
                    storageobject,
                    (IStorageObject obj) => deserializer(obj.ReadAllString()),
                    (obj, @string) => obj.WriteAllString(serializer(@string))
                );

        public static DurationFieldBinder<T> Bind<T>(
                this DurationField<T> field,
                StorageObjectID storageobjectID,
                EditorFile file
            ) =>
            new DurationFieldBinder<T>(
                    storageobjectID,
                    file,
                    field
                );
    }
}
