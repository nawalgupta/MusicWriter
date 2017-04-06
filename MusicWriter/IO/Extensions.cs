using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public static class StorageExtensions {
        public static void Transfer(
                this IStorageGraph source,
                IStorageGraph destination
            ) {
            var translationIDmap =
                new Dictionary<StorageObjectID, StorageObjectID>();

            foreach (var oldobjID in source.ObjectIDs) {
                IStorageObject obj_source, obj_destination;

                if (oldobjID != source.Root) {
                    obj_source = source[oldobjID];

                    translationIDmap.Add(oldobjID, (obj_destination = destination.CreateObject()).ID);
                }
                else {
                    obj_source = source[source.Root];
                    obj_destination = destination[destination.Root];

                    translationIDmap.Add(oldobjID, destination.Root);
                }

                using (var stream_src = obj_source.OpenRead()) {
                    using (var stream_dst = obj_destination.OpenWrite()) {
                        stream_src.CopyTo(stream_dst);
                    }
                }
            }

            foreach (var oldobjID in source.ObjectIDs) {
                var newobjID = translationIDmap[oldobjID];
                var newobj = destination[newobjID];

                foreach (var outgoing in source.Outgoing(oldobjID))
                    newobj.Add(outgoing.Key, translationIDmap[outgoing.Value]);
            }
        }

        public static string ReadAllString(this IStorageObject obj) {
            using (var stream = obj.OpenRead()) {
                using (var tr = new StreamReader(stream)) {
                    return tr.ReadToEnd();
                }
            }
        }

        public static void WriteAllString(this IStorageObject obj, string value) {
            using (var stream = obj.OpenWrite()) {
                using (var tw = new StreamWriter(stream)) {
                    tw.Write(value);
                }
            }
        }

        public static IOListener CreateListen(
                this IStorageGraph graph,
                Action<IOMessage> responder,
                StorageObjectID subject,
                IOEvent verb,
                StorageObjectID @object = default(StorageObjectID),
                string key = default(string),
                string newkey = default(string)
            ) =>
            new IOListener(
                    new IOMessage(
                            subject,
                            verb,
                            key,
                            newkey,
                            @object
                        ),
                    responder
                );

        public static IOListener CreateListen_Node(
                this IStorageGraph graph,
                StorageObjectID subject,
                IOEvent verb,
                Action<string, StorageObjectID> childresponder
            ) =>
            new IOListener(
                    new IOMessage(
                            subject,
                            verb
                        ),
                    message =>
                        childresponder(message.Relation, message.Object)
                );

        public static IOListener CreateListen_Node(
                this IStorageGraph graph,
                StorageObjectID subject,
                IOEvent verb,
                Action<StorageObjectID> childresponder
            ) =>
            CreateListen_Node(
                    graph,
                    subject,
                    verb,
                    (key, @object) =>
                        childresponder(@object)
                );

        public static IOListener CreateListen_Node(
                this IStorageGraph graph,
                StorageObjectID subject,
                IOEvent verb,
                Action responder
            ) =>
            CreateListen_Node(
                    graph,
                    subject,
                    verb,
                    (key, @object) =>
                        responder()
                );

        public static IOListener CreateListen(
                this IStorageObject storage,
                IOEvent verb,
                Action<string, StorageObjectID> childresponder
            ) =>
            storage
                .Graph
                .CreateListen_Node(
                        storage.ID,
                        verb,
                        childresponder
                    );

        public static IOListener CreateListen(
                this IStorageObject storage,
                IOEvent verb,
                Action<StorageObjectID> childresponder
            ) =>
            storage
                .Graph
                .CreateListen_Node(
                        storage.ID,
                        verb,
                        childresponder
                    );

        public static IOListener CreateListen(
                this IStorageObject storage,
                IOEvent verb,
                Action responder
            ) =>
            storage
                .Graph
                .CreateListen_Node(
                        storage.ID,
                        verb,
                        responder
                    );

        public static IStorageObject Object<T>(this T item)
            where T : IBoundObject<T> =>
            item.Object<T, T>();

        public static IStorageObject Object<T, T2>(this T item)
            where T : IBoundObject<T2>
            where T2 : IBoundObject<T2> =>
            item.File.Storage[item.StorageObjectID];

        public static void Delete<T>(this T item)
            where T : IBoundObject<T> =>
            item.Delete<T, T>();

        public static void Delete<T, T2>(this T item)
            where T : IBoundObject<T2>
            where T2 : IBoundObject<T2> =>
            item.Object<T, T2>().Delete();

        public static bool AcquireExclusiveOwnership(this IStorageObject storageobj) {
            var str = Guid.NewGuid().ToString();
            var node = storageobj.GetOrMake("rand");

            node.WriteAllString(str);
            //TODO: flush database for this node
            Task.Delay(20).Wait();

            return node.ReadAllString() == str;
        }
    }
}
