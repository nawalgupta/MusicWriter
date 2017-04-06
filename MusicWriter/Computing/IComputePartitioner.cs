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

        void PartitionChunk(
                EditorFile file,
                StorageObjectID item_nodeID,
                StorageObjectID jobinfo_nodeID,
                StorageObjectID partition_nodeID
            );

        void FailChunk(
                EditorFile file,
                StorageObjectID item_nodeID,
                StorageObjectID jobinfo_nodeID,
                StorageObjectID partition_nodeID
            );
    }
}
