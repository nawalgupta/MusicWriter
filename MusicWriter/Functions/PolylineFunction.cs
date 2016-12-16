using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class PolylineFunction : IFunction, IDirectlyIntegratableFunction
    {
        readonly List<Time> times =
            new List<Time>();
        readonly List<float> values =
            new List<float>();

        public void Add(Time t, float value) {
            var i_left = bsearch_time_left(t);

            times.Insert(i_left + 1, t);
            values.Insert(i_left + 1, value);
        }

        public float GetValue(FunctionCall arg) {
            var i_left = bsearch_time_left(arg.LocalTime);

            if (i_left + 1 == values.Count)
                return values[i_left];
            else {
                var i_right = i_left + 1;

                var t_left = times[i_left];
                var t_right = times[i_right];

                var v_left = values[i_left];
                var v_right = values[i_right];

                var t_diff = (t_right - t_left).Notes;
                var v_diff = v_right - v_left;

                var m = v_diff / t_diff;

                var t_local = (arg.LocalTime - t_left).Notes;

                return t_local * m + v_left;
            }
        }

        int bsearch_time_left(Time time) {
            //NOTE: this is definitely not a super-efficient version of the binary search,
            // but it will still cut lookup time to O(log2(n))

            var n = (int)Math.Pow(2, Math.Ceiling(Math.Log(times.Count, 2)) - 1);
            var i = n - times.Count > 1 ? times.Count % 2 : 0;

            if (times.Count == 0)
                return -1;

            while (n > 1) {
                if (i == times.Count)
                    break;

                var t = times[i];

                if (t > time)
                    i -= n;
                else i += n;

                n /= 2;
            }

            if (i == times.Count)
                return i;

            if (i >= 0)
                while (times[i] > time)
                    i--;

            return i;
        }

        public IFunction Integrate() {
            var newline = new PolylineFunction();

            var accumulator = 0f;
            var lastt = Time.Zero;
            var lastv = 0f;
            for(var i = 0; i < times.Count; i++) {
                var t = times[i];
                var v = values[i];
                var derivative = v - lastv;

                var tdiff = (t - lastt).Notes;

                var area = lastv * tdiff + 0.5f * derivative * tdiff;
                
                accumulator += area;
                lastt = t;
                lastv = v;

                newline.times.Add(t);
                newline.values.Add(accumulator);
            }

            return newline;
        }
    }
}
