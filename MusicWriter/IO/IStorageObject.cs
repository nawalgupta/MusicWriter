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

        StorageObjectID this[string key] { get; }

        event StorageObjectChildChangedDelegate ChildAdded;
        event StorageObjectChildChangedDelegate ChildRemoved;
        event StorageObjectChildChangedDelegate ChildRenamed;
        event StorageObjectChildChangedDelegate ChildContentsChanged;

        event StorageObjectChangedDelegate ContentsChanged;
        event StorageObjectChangedDelegate Deleted;

        string GetRelation(StorageObjectID child);
        IStorageObject Open(string child);

        Stream OpenRead();
        Stream OpenWrite();

        void Add(string key, StorageObjectID id);
        void Rename(string oldkey, string newkey);
        void Rename(StorageObjectID child, string newkey);
        void Remove(string key);
        void Remove(StorageObjectID child);

        void Delete();
    }
}
