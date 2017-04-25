using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FunctionWaveComputePartitioner : IComputePartitioner
    {
        // Because uint32s are used, at 192K samples/sec, there can only be ~ 6.2 hours max
        public uint SamplesPerPartition { get; set; } = 1024 * 64;

        public string Container {
            get { return FunctionWaveContainer.ItemName; }
        }

        public void SetupPartitioner(
                EditorFile file,
                StorageObjectID item_objID,
                StorageObjectID jobinfo_objID
            ) {
            var jobinfo_obj =
                file.Storage[jobinfo_objID];

            var item =
                file[FunctionWaveContainer.ItemName]
                    .As<IContainer, FunctionWaveContainer>()
                    .FunctionWaves[item_objID];

            using (var writer = new BinaryWriter(jobinfo_obj.OpenWrite())) {
                writer.Write(0u);
                writer.Write(0u);
                writer.Write(item.EndSample);
            }
        }

        public bool PartitionChunk(
                EditorFile file,
                StorageObjectID item_objID,
                StorageObjectID jobinfo_objID,
                StorageObjectID partition_objID
            ) {
            var item_obj =
                file.Storage[item_objID];

            var jobinfo_obj =
                file.Storage[jobinfo_objID];

            var partition_obj =
                file.Storage[partition_objID];

            uint start;
            uint index;
            uint end_samples;
            using (var reader = new BinaryReader(jobinfo_obj.OpenRead())) {
                start = reader.ReadUInt32();
                index = reader.ReadUInt32();
                end_samples = reader.ReadUInt32();
            }
            
            var end = Math.Min(start + SamplesPerPartition, end_samples);
            using (var writer = new BinaryWriter(jobinfo_obj.OpenWrite())) {
                writer.Write(end);
                writer.Write(index + 1);
                writer.Write(end_samples);
            }

            using (var writer = new BinaryWriter(partition_obj.OpenWrite())) {
                writer.Write(start);
                writer.Write(end);
                writer.Write(index);
            }

            return end == end_samples;
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
