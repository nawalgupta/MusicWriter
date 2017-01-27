using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IStorageGraph
    {
        StorageObjectID Root { get; }

        IEnumerable<StorageObjectID> ObjectIDs { get; }

        IStorageObject this[StorageObjectID id] { get; }

        IObservableList<IOMessage> Messages { get; }
        IObservableList<IOListener> Listeners { get; }

        IEnumerable<KeyValuePair<string, StorageObjectID>> Outgoing(StorageObjectID source);
        IEnumerable<KeyValuePair<string, StorageObjectID>> Incoming(StorageObjectID sink);

        bool HasChild(StorageObjectID source, string relation);
        bool HasChild(StorageObjectID source, StorageObjectID sink);
        bool HasChild(StorageObjectID source, StorageObjectID sink, string relation);
        string GetRelation(StorageObjectID source, StorageObjectID sink);
        IEnumerable<string> GetRelations(StorageObjectID source, StorageObjectID sink);

        StorageObjectID Create();
        void Delete(StorageObjectID id);

        IEnumerable<StorageObjectID> Isolated();

        void Flush();
    }
}
