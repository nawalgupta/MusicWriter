using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class NumberField
    {
        readonly Dictionary<string, int> dimensions =
            new Dictionary<string, int>();

        int dimensionscount = 0;

        float[] samples = new float[0];
        readonly object locker =
            new object();

        public int GetDimension(string name) =>
            dimensions[name];

        public void AddDimension(string name) {
            lock (locker) {
                Array.Resize(ref samples, samples.Length / dimensionscount * (dimensionscount + 1));

                for (int i = samples.Length / dimensionscount - 1; i >= 0; i--) {
                    for (int k = dimensionscount - 1; k >= 0; k--)
                        samples[i * (dimensionscount + 1) + k] = samples[i * dimensionscount + k];
                    
                    samples[i * (dimensionscount + 1)] = 0;
                }

                dimensions.Add(name, dimensionscount++);
            }
        }

        public void RemoveDimension(string name) {
            lock (locker) {
                var i = dimensions[name];
                
                for (int j = samples.Length / dimensionscount - 1; j >= 0; j--)
                    for (int k = 0; k < dimensionscount; k++)
                        samples[j * (dimensionscount - 1) + i] = samples[j * dimensionscount + i + 1];
                
                Array.Resize(ref samples, samples.Length - samples.Length / dimensionscount);
                dimensionscount--;

                dimensions.Remove(name);
                foreach (var dimension in dimensions.Keys.ToArray())
                    if (dimensions[dimension] > i)
                        dimensions[dimension]--;
            }
        }

        public void Serialize(Stream stream) {
            using (var bw = new BinaryWriter(stream)) {
                bw.Write(dimensionscount);

                foreach (var dimension in dimensions) {
                    bw.Write(dimension.Value);
                    bw.Write(dimension.Key);
                }

                bw.Write(samples.Length);
                for (int i = samples.Length - 1; i >= 0; i--)
                    bw.Write(samples[i]);
            }
        }

        public void Deserialize(Stream stream) {
            using (var br = new BinaryReader(stream)) {
                dimensionscount = br.ReadInt32();

                for (int i = 0; i < dimensionscount; i++) {
                    var index = br.ReadInt32();
                    var name = br.ReadString();

                    dimensions.Add(name, index);
                }

                samples = new float[br.ReadInt32()];
                for (int i = samples.Length - 1; i >= 0; i--)
                    samples[i] = br.ReadSingle();
            }
        }

        public void Polate(
                float[] point,
                bool[] known,
                bool[] desired
            ) {
            // "point" is an array of the values of the point at known dimensions
            // "known" is an array of booleans meaning which dimensions' values are known
            // "desired" is an array of booleans meaning which dimensions' values are desired; should be calculated

            //TODO: fix this algorithm or replace it with a better one or at least test it.

            float[] finalpoint = new float[dimensionscount];
            var totalinfluence = 0f;

            for (int i = samples.Length - dimensionscount; i >= 0; i -= dimensionscount) {
                var distance_square = 0f;

                for (int j = dimensionscount - 1; j >= 0; j--) {
                    if (known[j]) {
                        var d = samples[i * dimensionscount + j] - point[j];

                        distance_square += d * d;
                    }
                }

                var distance = (float)Math.Sqrt(distance_square);

                var influence = 1f / distance;

                for (int j = dimensionscount - 1; j >= 0; j--)
                    if (desired[j])
                        finalpoint[j] = samples[i * dimensionscount + j] * influence;

                totalinfluence += influence;
            }

            for (int j = dimensionscount - 1; j >= 0; j--)
                if (desired[j])
                    point[j] = finalpoint[j];
        }
    }
}
