using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FunctionWaveComputeSlave : IComputeSlave
    {
        public string Container {
            get { return FunctionWaveContainer.ItemName; }
        }

        public void Complete(
                ComputeJob job,
                StorageObjectID partition_objID
            ) {
            uint start, end;
            uint index;
            float samples_per_unit;

            using (var reader = new BinaryReader(job.File.Storage[partition_objID].OpenRead())) {
                start = reader.ReadUInt32();
                end = reader.ReadUInt32();
                index = reader.ReadUInt32();
            }

            var wave_obj =
                job.File.Storage[job.WorkItemStorageObjectID];

            using (var reader = new BinaryReader(wave_obj.OpenRead())) {
                samples_per_unit = reader.ReadSingle();
            }
            
            var func_objID =
                wave_obj["function"];

            var func =
                job
                    .File
                    [FunctionContainer.ItemName]
                    .As<IContainer, FunctionContainer>()
                    .FunctionSources
                    [func_objID]
                    .Function
                    .Value;
            
            var frag_obj =
                wave_obj.Get(index.ToString());

            using (var writer = new BinaryWriter(frag_obj.OpenWrite())) {
                for (uint sample_i = start; sample_i < end; sample_i++) {
                    var t = sample_i / samples_per_unit;
                    var call = new FunctionCall(t);
                    var value = func.GetValue(call);

                    writer.Write(value);
                }
            }
        }
    }
}
