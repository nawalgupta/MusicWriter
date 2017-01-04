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

        IEnumerable<StorageObjectID> Objects { get; }

        IStorageObject this[StorageObjectID id] { get; }
        
        event StorageObjectChildChangedDelegate ArrowAdded;
        event StorageObjectChildChangedDelegate ArrowRemoved;
        event StorageObjectChildRekeyedDelegate ArrowRenamed;

        event StorageObjectChangedDelegate NodeCreated;
        event StorageObjectChangedDelegate NodeContentsSet;
        event StorageObjectChangedDelegate NodeDeleted;

        IEnumerable<KeyValuePair<string, StorageObjectID>> Outgoing(StorageObjectID source);
        IEnumerable<StorageObjectID> Incoming(StorageObjectID sink);

        bool HasChild(StorageObjectID source, string relation);
        string GetRelation(StorageObjectID source, StorageObjectID sink);

        StorageObjectID Create();
        void Delete(StorageObjectID id);

        IEnumerable<StorageObjectID> Isolated();
    }
}
