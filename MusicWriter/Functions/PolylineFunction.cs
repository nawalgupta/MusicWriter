using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class PolylineFunction : IFunction, IDirectlyIntegratableFunction
    {
        readonly List<float> times =
            new List<float>();
        readonly List<float> values =
            new List<float>();

        public IFunctionFactory Factory {
            get { return FactoryClass.Instance; }
        }

        public sealed class FactoryClass : IFunctionFactory
        {
            public string Name {
                get { return "Polyline Function"; }
            }

            public string CodeName {
                get { return "polyline"; }
            }

            public bool AcceptsParameters {
                get { return false; }
            }

            public bool StoresBinaryData {
                get { return true; }
            }

            public IFunction Create() =>
                new PolylineFunction();

            public IFunction Create(IFunction[] args) {
                throw new InvalidOperationException();
            }

            public IFunction Create(params float[] args) {
                throw new NotImplementedException();
            }

            public IFunction Create(IFunction context) {
                throw new InvalidOperationException();
            }

            public IFunction Create(IFunction context, params float[] args) {
                throw new InvalidOperationException();
            }

            public IFunction Create(IFunction[] args, params float[] numbers) {
                throw new InvalidOperationException();
            }

            public IFunction Create(IFunction context, IFunction[] args, params float[] numbers) {
                throw new InvalidOperationException();
            }

            public IFunction Deserialize(Stream stream) {
                using (var br = new BinaryReader(stream)) {
                    var n = br.ReadInt32();

                    var retval = new PolylineFunction();

                    for (int i = n - 1; i >= 0; i--) {
                        var time = br.ReadSingle();
                        var value = br.ReadSingle();

                        retval.Add(time, value);
                    }

                    return retval;
                }
            }

            public void Serialize(Stream stream, IFunction function) {
                var poly =
                    function as PolylineFunction;

                using (var bw = new BinaryWriter(stream)) {
                    bw.Write(poly.values.Count);

                    for (int i = poly.values.Count - 1; i >= 0; i--) {
                        bw.Write(poly.times[i]);
                        bw.Write(poly.values[i]);
                    }
                }
            }

            public IFunction Deserialize(Stream stream, IFunction context) {
                throw new InvalidOperationException();
            }

            public IFunction Deserialize(Stream stream, IFunction[] arguments) {
                throw new InvalidOperationException();
            }

            public IFunction Deserialize(Stream stream, IFunction context, IFunction[] arguments) {
                throw new InvalidOperationException();
            }

            private FactoryClass() { }

            public static readonly IFunctionFactory Instance = new FactoryClass();
        }

        public PolylineFunction() {
        }

        public PolylineFunction(float constant) {
            Add(0f, constant);
        }

        public void AddConstant(float t, float value) {
            var i = bsearch_time_left(t);

            times.Insert(i + 1, t);
            values.Insert(i + 1, value);

            if (i > 0 && times[i] < t) {
                times.Insert(i + 1, t);
                values.Insert(i + 1, values[i]);
            }
        }

        public void Add(float t, float value) {
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

                var t_diff = t_right - t_left;
                var v_diff = v_right - v_left;

                var m = v_diff / t_diff;

                var t_local = arg.LocalTime - t_left;

                return t_local * m + v_left;
            }
        }

        int bsearch_time_left(float time) {
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
            var lastt = 0f;
            var lastv = 0f;
            for(var i = 0; i < times.Count; i++) {
                var t = times[i];
                var v = values[i];
                var derivative = v - lastv;

                var tdiff = t - lastt;

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
