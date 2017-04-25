using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IComputePartitioner
    {
        string Container { get; }

        void SetupPartitioner(
                EditorFile file,
                StorageObjectID item_jobID,
                StorageObjectID jobinfo_objID
            );

        bool PartitionChunk(
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
