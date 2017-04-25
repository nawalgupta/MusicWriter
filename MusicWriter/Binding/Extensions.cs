﻿using System;
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

        public static PropertyBinder<float> Bind(
                this ObservableProperty<float> property,
                IStorageObject storageobject
            ) =>
            Bind(
                    property,
                    storageobject,
                    Convert.ToSingle,
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

        public static PropertyBinder<SemiTone> Bind(
                this ObservableProperty<SemiTone> property,
                IStorageObject storageobject
            ) =>
            Bind(
                    property,
                    storageobject,
                    SemiTone.Parse,
                    ObviousExtensions.ToString
                );

        public static PropertyBinder<PerceptualTime> Bind(
                this ObservableProperty<PerceptualTime> property,
                IStorageObject storageobject
            ) =>
            Bind(
                    property,
                    storageobject,
                    PerceptualTime.Parse,
                    ObviousExtensions.ToString
                );

        public static PropertyBinder<Mode> Bind(
                this ObservableProperty<Mode> property,
                IStorageObject storageobject
            ) =>
            Bind(
                    property,
                    storageobject,
                    ObviousExtensions.EnumParse<Mode>,
                    ObviousExtensions.ToString
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

        public static PropertyBinder<T> BindEnum<T>(
                this ObservableProperty<T> property,
                IStorageObject storageobject
            ) =>
            property
                .Bind(
                        storageobject,
                        ObviousExtensions.EnumParse<T>,
                        ObviousExtensions.ToString
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

        public static ObjectPropertyBinder<T> BindObject<T>(
                this ObservableProperty<T> property,
                StorageObjectID hub,
                string relation,
                BoundList<T> boundlist
            ) where T : IBoundObject<T> =>
            new ObjectPropertyBinder<T>(
                    hub,
                    boundlist.File,
                    relation,
                    boundlist,
                    property
                );
    }
}
