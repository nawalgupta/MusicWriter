using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IComputePartition
    {
        string Container { get; }

        void SetupPartitioner(
                EditorFile file,
                StorageObjectID jobinfo_objID
            );

        void PartitionChunk(
                EditorFile file,
                StorageObjectID item_objID,
                StorageObjectID jobinfo_objID,
                StorageObjectID partition_objID
            );

        void FailChunk(
                EditorFile file,
                StorageObjectID item_objID,
                StorageObjectID jobinfo_objID,
                StorageObjectID partition_objID
            );
    }
}
