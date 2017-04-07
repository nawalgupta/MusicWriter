using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FunctionComputePartitioner : IComputePartition
    {
        public string Container {
            get { return FunctionContainer.ItemName; }
        }

        public void PartitionChunk(
                EditorFile file,
                StorageObjectID item_nodeID,
                StorageObjectID jobinfo_nodeID,
                StorageObjectID partition_nodeID
            ) {
            throw new NotImplementedException();
        }

        public void FailChunk(
                EditorFile file, 
                StorageObjectID item_nodeID, 
                StorageObjectID jobinfo_nodeID, 
                StorageObjectID partition_nodeID
            ) {
            throw new NotImplementedException();
        }
    }
}
