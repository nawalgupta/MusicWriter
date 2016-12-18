using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public static class Statistics {
        public static float AvgMin(IEnumerable<float> values) {
            var array = values.ToArray();
            Array.Sort(array);

            if (array.Length == 0)
                return 0f;

            // calculate the weighted mean of values against 8t(t-0.5)^2
            float sum_value = 0F;
            float sum_weight = 0F;
            var length = (float)array.Length;

            for (int i = 0; i < array.Length; i++) {
                var t = i / length + 0.01F;

                var weight = 4 * t * Square(t - 0.5F);

                sum_value += array[i] * weight;
                sum_weight += weight;
            }

            return sum_value / sum_weight;
        }

        public static float Square(float x) => x * x;

        public static T Median<T>(this IEnumerable<T> collection) {
            var items =
                collection.ToArray();

            if (items.Length == 1)
                return items[0];
            else if (items.Length == 0)
                return default(T);
            else {
                return items[items.Length / 2];
            }
        }

        public static float Mean(this float[] values) {
            var sum = 0F;

            for (int i = values.Length - 1; i >= 0; i--)
                sum += values[i];

            return sum / values.Length;
        }

        public static bool LinearRegression(
                float[] Xs,
                float[] Ys,
                out float m,
                out float b
            ) {
            // based on https://en.wikipedia.org/wiki/Simple_linear_regression

            var xmean =
                Xs.Mean();

            var ymean =
                Ys.Mean();
            
            var num = 0F;
            var den = 0F;

            for (int i = Xs.Length - 1; i >= 0; i--) {
                var Xmeandiff =
                    Xs[i] - xmean;

                var Ymeandiff =
                    Ys[i] - ymean;
                
                num += Xmeandiff * Ymeandiff;
                den += Square(Xmeandiff);
            }

            m = num / den;
            b = ymean - m * xmean;

            //TODO: really should run a test against Pearson correlation
            // coefficient to see if data is correlated.
            
            return true;
        }
    }
}
