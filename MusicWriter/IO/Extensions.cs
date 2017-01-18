﻿using System;
using System.Collections.Generic;
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

            foreach (var oldobjID in source.Objects) {
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

            foreach (var oldobjID in source.Objects) {
                var newobjID = translationIDmap[oldobjID];
                var newobj = destination[newobjID];

                foreach (var outgoing in source.Outgoing(oldobjID))
                    newobj.Add(outgoing.Key, translationIDmap[outgoing.Value]);
            }
        }
    }
}