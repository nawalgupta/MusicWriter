using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public static class StorageExtensions
    {
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

        public static IOListener Listen(
                this IStorageGraph graph,
                Action<IOMessage> responder,
                StorageObjectID subject,
                IOEvent verb,
                StorageObjectID @object = default(StorageObjectID),
                string key = default(string),
                string newkey = default(string)
            ) {
            var filter =
                new IOMessage(
                        subject,
                        verb,
                        key,
                        newkey,
                        @object
                    );

            var listener =
                new IOListener(
                        filter,
                        responder
                    );

            graph.Listeners.Add(listener);

            return listener;
        }

        public static IOListener Listen_Node(
                this IStorageGraph graph,
                StorageObjectID subject,
                IOEvent verb,
                Action<string, StorageObjectID> childresponder
            ) {
            var filter =
                new IOMessage(
                        subject,
                        verb
                    );

            var listener =
                new IOListener(
                        filter,
                        message =>
                            childresponder(message.Relation, message.Object)
                    );

            graph.Listeners.Add(listener);

            return listener;
        }

        public static IOListener Listen_Node(
                this IStorageGraph graph,
                StorageObjectID subject,
                IOEvent verb,
                Action<StorageObjectID> childresponder
            ) =>
            Listen_Node(
                    graph,
                    subject,
                    verb,
                    (key, @object) =>
                        childresponder(@object)
                );

        public static IOListener Listen_Node(
                this IStorageGraph graph,
                StorageObjectID subject,
                IOEvent verb,
                Action responder
            ) =>
            Listen_Node(
                    graph,
                    subject,
                    verb,
                    (key, @object) =>
                        responder()
                );

        public static IOListener Listen(
                this IStorageObject storage,
                IOEvent verb,
                Action<string, StorageObjectID> childresponder
            ) =>
            storage
                .Graph
                .Listen_Node(
                        storage.ID,
                        verb,
                        childresponder
                    );

        public static IOListener Listen(
                this IStorageObject storage,
                IOEvent verb,
                Action<StorageObjectID> childresponder
            ) =>
            storage
                .Graph
                .Listen_Node(
                        storage.ID,
                        verb,
                        childresponder
                    );

        public static IOListener Listen(
                this IStorageObject storage,
                IOEvent verb,
                Action responder
            ) =>
            storage
                .Graph
                .Listen_Node(
                        storage.ID,
                        verb,
                        responder
                    );

        public static void Delete<T>(this T item) where T : IBoundObject<T> =>
            item.File.Storage.Delete(item.StorageObjectID);
    }
}
