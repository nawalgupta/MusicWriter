using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed partial class PolylineData : NamedBoundObject<PolylineData>
    {
        public const string ItemName = "musicwriter.data.polyline";
        
        readonly List<double> times =
            new List<double>();
        readonly List<double> values =
            new List<double>();
        readonly IOListener
            listener_add,
            listener_rekey,
            listener_contentsset,
            listener_remove;

        public override IFactory<PolylineData> Factory {
            get { return FactoryInstance; }
        }

        readonly IStorageObject storage;

        public PolylineData(
                StorageObjectID storageobjectID,
                EditorFile file
            )
            : this(
                    file.Storage[storageobjectID],
                    file
                 ) {
        }
        
        public PolylineData(
                IStorageObject storage,
                EditorFile file,
                float constant = 0
            ) :
            base(
                    storage.ID,
                    file,
                    FactoryInstance
                ) {
            this.storage = storage;
            
            if (storage.IsEmpty)
                Add(0f, constant);

            listener_add =
                storage.CreateListen(IOEvent.ChildAdded, (key, pt_objID) => {
                    var t = double.Parse(key);
                    var v = double.Parse(storage.Graph[pt_objID].ReadAllString());

                    Add_ram(t, v);
                });

            listener_rekey =
                storage.Graph.CreateListen(
                    msg => {
                        var t0 = double.Parse(msg.Relation);
                        var t1 = double.Parse(msg.NewRelation);

                        MoveX_ram(t0, t1);
                    },
                    storage.ID,
                    IOEvent.ChildRekeyed
                );

            listener_contentsset =
                storage.CreateListen(IOEvent.ChildContentsSet, (key, pt_objID) => {
                    var t = double.Parse(key);
                    var v1 = double.Parse(storage.Graph[pt_objID].ReadAllString());

                    MoveY_ram(t, v1);
                });

            listener_remove =
                storage.CreateListen(IOEvent.ChildRemoved, (key, pt_objID) => {
                    var t = double.Parse(key);
                    var v = double.Parse(storage.Graph[pt_objID].ReadAllString());

                    RemoveExact_ram(t, v);
                });
        }

        public override void Bind() {
            File.Storage.Listeners.Add(listener_add);
            File.Storage.Listeners.Add(listener_rekey);
            File.Storage.Listeners.Add(listener_contentsset);
            File.Storage.Listeners.Add(listener_remove);
            
            base.Bind();
        }

        public override void Unbind() {
            File.Storage.Listeners.Remove(listener_add);
            File.Storage.Listeners.Remove(listener_rekey);
            File.Storage.Listeners.Remove(listener_contentsset);
            File.Storage.Listeners.Remove(listener_remove);

            base.Unbind();
        }
        
        public void AddConstant(double t, double value) {
            var i = bsearch_time_left(t);
            double? t1 =
                i + 1 != times.Count ?
                    times[i] - (1 / 256f) :
                    default(double?);

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

        public void Add(double t, double value) {
            if (storage.HasChild(t.ToString()))
                storage.Get(t.ToString()).WriteAllString(value.ToString());
            else {
                var pt_obj = storage.Graph.CreateObject();
                pt_obj.WriteAllString(value.ToString());
                storage.Add(t.ToString(), pt_obj.ID);
            }
        }

        void Add_ram(double t, double value) {
            var i_left = bsearch_time_left(t);

            times.Insert(i_left + 1, t);
            values.Insert(i_left + 1, value);
        }

        public void Remove(double t) =>
            storage.Get(times[bsearch_time_left(t)].ToString()).Delete();

        void Remove_ram(double t) {
            var i_left = bsearch_time_left(t);

            times.RemoveAt(i_left);
            values.RemoveAt(i_left);
        }

        public void RemoveExact(double t, double v) {
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

        void RemoveExact_ram(double t, double v) {
            var i_left = bsearch_time_left(t);

            if (times[i_left] != t)
                throw new ArgumentException();

            if (values[i_left] != v)
                throw new ArgumentException();

            times.RemoveAt(i_left);
            values.RemoveAt(i_left);
        }

        public void MoveX(double t0, double t1) {
            var v = values[times.IndexOf(t0)];

            Remove(t0);
            Add(t1, v);
        }

        void MoveX_ram(double t0, double t1) {
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

        public void MoveXY(double t0, double t1, double v1) {
            Remove(t0);
            Add(t1, v1);
        }

        void MoveXY_ram(double t0, double t1, double v1) {
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

        public void MoveY(double t, double v1) {
            var pt_obj = storage.Get(t.ToString());
            pt_obj.WriteAllString(v1.ToString());
        }

        void MoveY_ram(double t, double v1) {
            var i_left = bsearch_time_left(t);

            if (times[i_left] != t)
                throw new ArgumentException();

            values[i_left] = v1;
        }
        
        public double GetValue(double t) {
            if (values.Count == 0)
                return float.NaN;

            var i_left = bsearch_time_left(t);

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

                var t_local = t - t_left;

                return t_local * m + v_left;
            }
        }
        
        public double GetIntegratedValue(double t) {
            if (values.Count == 0)
                return double.NaN;

            if (t == 0)
                return 0;

            var i_left = bsearch_time_left(t);

            var area = 0.0;
            var t_left = times[0];
            var v_left = values[0];
            for (int i = 0; i <= i_left; i++) {
                var t_right = times[i + 1];
                var v_right = values[i + 1];

                var t_diff = t_right - t_left;
                var v_diff = v_right - v_left;

                // The graph is broken into a bunch of right triangles
                // sitting on rectangles - find the area of each, but
                // the very last one, the one that time [t] intersects,
                // split into a fractional piece.

                if (i != i_left) {
                    // rectangle
                    area += v_left * t_diff;

                    // triangle
                    area += 0.5f * v_diff * t_diff;

                    if (t_right == t)
                        break; // area of next segment would be 0
                }
                else {
                    var t_local = t - t_left;

                    // rectangle
                    area += v_left * t_local;

                    // triangle
                    area += 0.5f * v_diff * t_local * t_local / t_diff;
                }

                t_left = t_right;
                v_left = v_right;
            }

            return area;
        }

        public bool GetInvertedIntegratedValue(double area, out double t) {
            if (values.Count == 0) {
                t = float.NaN;

                return false;
            }

            double t_left, t_right, t_diff;
            double v_left, v_right, v_diff;
            double localarea;

            t_left = times[0];
            v_left = values[0];

            int i;
            for (i = 1; i < times.Count; i++) {
                t_right = times[i];
                v_right = values[i];

                t_diff = t_right - t_left;
                v_diff = v_right - v_left;

                // rectangle
                localarea = t_diff * v_left;

                // triangle
                localarea += 0.5f * t_diff * v_diff;

                if (area >= localarea)
                    area -= localarea;
                else {
                    // use quadratic formula to find missing area - WRONG
                    var m = v_diff / t_diff;
                    var t_local = (-v_left + (float)Math.Sqrt(v_left * v_left + 2 * m * area)) / m;
                    t = t_left + t_local;

                    return true;
                }

                t_left = t_right;
                v_left = v_right;
            }

            if (area != 0) {
                if (i == times.Count) {
                    if (v_left > 0) {
                        t = t_left + area / v_left;
                        return true;
                    }
                }

                t = float.NaN;
                return false;
            }

            t = t_left;
            return true;
        }

        int bsearch_time_left(double time) {
            var i = times.BinarySearch(time);

            if (i == -1)
                return -1;
            else if (i < -1)
                return ~i - 1;
            else return i;
        }

        public static IFactory<PolylineData> FactoryInstance { get; }=
            new CtorFactory<PolylineData, PolylineData>(
                    ItemName,
                    false
                );
    }
}
