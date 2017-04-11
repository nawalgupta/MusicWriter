using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FunctionWaveComputePartitioner : IComputePartition
    {
        // Because uint32s are used, at 192K samples/sec, there can only be ~ 6.2 hours max
        public uint SamplesPerPartition { get; set; } = 1024 * 64;

        public string Container {
            get { return FunctionWaveContainer.ItemName; }
        }

        public void SetupPartitioner(
                EditorFile file,
                StorageObjectID jobinfo_objID
            ) {

        }

        public void PartitionChunk(
                EditorFile file,
                StorageObjectID item_objID,
                StorageObjectID jobinfo_objID,
                StorageObjectID partition_objID
            ) {
            var jobinfo_obj =
                file.Storage[jobinfo_objID];

            var partition_obj =
                file.Storage[partition_objID];
            
            uint start;
            uint index;
            using (var reader = new BinaryReader(jobinfo_obj.OpenRead())) {
                start = reader.ReadUInt32();
                index = reader.ReadUInt32();
            }

            var end = start + SamplesPerPartition;
            using (var writer = new BinaryWriter(jobinfo_obj.OpenWrite())) {
                writer.Write(end);
                writer.Write(index + 1);
            }

            using (var writer = new BinaryWriter(partition_obj.OpenWrite())) {
                writer.Write(start);
                writer.Write(end);
                writer.Write(index);
            }
        }

        public void FailChunk(
                EditorFile file, 
                StorageObjectID item_objID, 
                StorageObjectID jobinfo_objID, 
                StorageObjectID partition_objID
            ) {
            //TODO
            throw new NotImplementedException();
        }
    }
}
