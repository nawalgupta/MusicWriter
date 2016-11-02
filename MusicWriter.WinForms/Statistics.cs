using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms {
    public static class Statistics {
        public static float AvgMin(IEnumerable<float> values) {
            var array = values.ToArray();
            Array.Sort(array);

            // calculate the weighted mean of values against 8t(t-0.5)^2
            float sum_value = 0F;
            float sum_weight = 0F;
            var length = (float)array.Length;

            for (int i = 0; i < array.Length; i++) {
                var t = i / length;

                var weight = 4 * t * Square(t - 0.5F);

                sum_value += array[i] * weight;
                sum_weight += weight;
            }

            return sum_value / sum_weight;
        }

        public static float Square(float x) => x * x;
    }
}
