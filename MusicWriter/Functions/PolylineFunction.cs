using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class PolylineFunction : IFunction, IStoredDataFunction, IDirectlyIntegratableFunction
    {
        readonly List<float> times =
            new List<float>();
        readonly List<float> values =
            new List<float>();

        readonly IStorageObject storage;

        public IFunctionFactory Factory {
            get { return FactoryClass.Instance; }
        }

        public IStorageObject Storage {
            get { return storage; }
        }

        StorageObjectID IStoredDataFunction.Storage {
            get { return storage.ID; }
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

            public IFunction Create(
                    IFunction context = null,
                    IFunction[] args = null,
                    IStorageObject data = null,
                    params float[] numbers
                ) {
                throw new InvalidOperationException();
            }

            private FactoryClass() { }

            public static readonly IFunctionFactory Instance = new FactoryClass();
        }
        
        public PolylineFunction(
                IStorageObject storage,
                float constant = 0
            ) {
            this.storage = storage;

            Setup();

            if (storage.IsEmpty)
                Add(0f, constant);
        }

        void Setup() {
            storage.ChildAdded += (storage_objID, pt_objID, key) => {
                var t = float.Parse(key);
                var v = float.Parse(storage.Graph[pt_objID].ReadAllString());

                Add_ram(t, v);
            };

            storage.ChildRenamed += (storage_objID, pt_objID, pt_old_t, pt_new_t) => {
                var t0 = float.Parse(pt_old_t);
                var t1 = float.Parse(pt_new_t);

                MoveX_ram(t0, t1);
            };

            storage.ChildContentsSet += (storage_objID, pt_objID, key) => {
                var t = float.Parse(key);
                var v1 = float.Parse(storage.Graph[pt_objID].ReadAllString());

                MoveY_ram(t, v1);
            };

            storage.ChildRemoved += (storage_objID, pt_objID, key) => {
                var t = float.Parse(key);
                var v = float.Parse(storage.Graph[pt_objID].ReadAllString());

                RemoveExact_ram(t, v);
            };
        }

        public void AddConstant(float t, float value) {
            var i = bsearch_time_left(t);
            float? t1 =
                i + 1 != times.Count ?
                    times[i] - (1 / 256f) :
                    default(float?);

            if (!storage.HasChild(t.ToString())) {
                var pt0_obj = storage.Graph.CreateObject();
                pt0_obj.WriteAllString(value.ToString());
                storage.Add(t.ToString(), pt0_obj.ID);
            }
            else {
                storage
                    .Get(t.ToString())
                    .WriteAllString(value.ToString());
            }

            if (t1.HasValue) {
                var pt1_obj = storage.Graph.CreateObject();
                pt1_obj.WriteAllString(value.ToString());
                storage.Add(t1.Value.ToString(), pt1_obj.ID);
            }
        }

        public void Add(float t, float value) {
            if (storage.HasChild(t.ToString()))
                storage.Get(t.ToString()).WriteAllString(value.ToString());
            else {
                var pt_obj = storage.Graph.CreateObject();
                pt_obj.WriteAllString(value.ToString());
                storage.Add(t.ToString(), pt_obj.ID);
            }
        }

        void Add_ram(float t, float value) {
            var i_left = bsearch_time_left(t);

            times.Insert(i_left + 1, t);
            values.Insert(i_left + 1, value);
        }

        public void Remove(float t) =>
            storage.Get(times[bsearch_time_left(t)].ToString()).Delete();

        void Remove_ram(float t) {
            var i_left = bsearch_time_left(t);

            times.RemoveAt(i_left);
            values.RemoveAt(i_left);
        }

        public void RemoveExact(float t, float v) {
            var i_left =
                bsearch_time_left(t);

            if (times[i_left] != t)
                throw new ArgumentException();

            var pt_obj =
                storage.Get(t.ToString());

            if (pt_obj.ReadAllString() != v.ToString())
                throw new ArgumentException();

            pt_obj.Delete();
        }

        void RemoveExact_ram(float t, float v) {
            var i_left = bsearch_time_left(t);

            if (times[i_left] != t)
                throw new ArgumentException();

            if (values[i_left] != v)
                throw new ArgumentException();

            times.RemoveAt(i_left);
            values.RemoveAt(i_left);
        }

        public void MoveX(float t0, float t1) {
            var v = values[times.IndexOf(t0)];

            Remove(t0);
            Add(t1, v);
        }

        void MoveX_ram(float t0, float t1) {
            var i0_left = bsearch_time_left(t0);

            if (times[i0_left] != t0)
                throw new ArgumentException();
            
            var i1_left = bsearch_time_left(t1);

            if (times[i1_left] == t1)
                throw new ArgumentException();

            var v = values[i0_left];

            times.RemoveAt(i0_left);
            values.RemoveAt(i0_left);

            times.Insert(i1_left, t1);
            values.Insert(i1_left, v);
        }

        public void MoveXY(float t0, float t1, float v1) {
            Remove(t0);
            Add(t1, v1);
        }

        void MoveXY_ram(float t0, float t1, float v1) {
            var i0_left = bsearch_time_left(t0);

            if (times[i0_left] != t0)
                throw new ArgumentException();

            var i1_left = bsearch_time_left(t1);

            if (times[i1_left] == t1)
                throw new ArgumentException();

            times.RemoveAt(i0_left);
            values.RemoveAt(i0_left);

            times.Insert(i1_left, t1);
            values.Insert(i1_left, v1);
        }

        public void MoveY(float t, float v1) {
            var pt_obj = storage.Get(t.ToString());
            pt_obj.WriteAllString(v1.ToString());
        }

        void MoveY_ram(float t, float v1) {
            var i_left = bsearch_time_left(t);

            if (times[i_left] != t)
                throw new ArgumentException();

            values[i_left] = v1;
        }

        public float GetValue(FunctionCall arg) {
            if (values.Count == 0)
                return float.NaN;

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
            var i = times.BinarySearch(time);

            if (i == -1)
                return -1;
            else if (i < -1)
                return ~i - 1;
            else return i;
        }

        public IFunction Integrate() {
            throw new NotImplementedException();
        }
    }
}
