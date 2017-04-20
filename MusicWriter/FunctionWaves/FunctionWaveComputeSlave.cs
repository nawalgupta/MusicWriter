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
            int bits_per_sample;

            using (var reader = new BinaryReader(job.File.Storage[partition_objID].OpenRead())) {
                start = reader.ReadUInt32();
                end = reader.ReadUInt32();
                index = reader.ReadUInt32();
            }

            var wave_obj =
                job.File.Storage[job.WorkItemStorageObjectID];

            using (var reader = new BinaryReader(wave_obj.OpenRead())) {
                samples_per_unit = reader.ReadSingle();
                bits_per_sample = reader.ReadInt32();
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

                    if (value < -1)
                        value = -1;

                    if (value > +1)
                        value = +1;

                    //TODO: is this a common improper floating-point handling technique?
                    var clamped =
                        value / 2 + 1;

                    switch (bits_per_sample) {
                        case 8:
                            writer.Write((byte)(value * byte.MaxValue));
                            break;

                        case 16:
                            writer.Write((ushort)(value * ushort.MaxValue));
                            break;

                        case 32:
                            writer.Write((uint)(value * uint.MaxValue));
                            break;

                        case 64:
                            writer.Write((ulong)(value * ulong.MaxValue));
                            break;

                        default:
                            throw new InvalidOperationException();
                    }
                }
            }
        }
    }
}
