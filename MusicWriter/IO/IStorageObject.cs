using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IStorageObject
    {
        IStorageGraph Graph { get; }
        StorageObjectID ID { get; }

        IEnumerable<StorageObjectID> Children { get; }
        IEnumerable<KeyValuePair<string, StorageObjectID>> RelationalChildren { get; }

        bool IsEmpty { get; }

        StorageObjectID this[string key] { get; }

        string GetRelation(StorageObjectID child);
        IEnumerable<string> GetRelations(StorageObjectID child);
        IStorageObject Open(string child);

        Stream OpenRead();
        Stream OpenWrite();

        bool HasChild(string relation);
        bool HasChild(StorageObjectID storageobjectID);
        void Add(string key, StorageObjectID id);
        void Rename(string oldkey, string newkey);
        void Rename(StorageObjectID child, string newkey);
        void Remove(string key);
        void Remove(StorageObjectID child);
        
        void Delete();
    }
}
