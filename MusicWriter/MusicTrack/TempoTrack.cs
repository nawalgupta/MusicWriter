using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class TempoTrack :
        BoundObject<TempoTrack>
    {
        readonly PolylineData notelengthdata;
        readonly IStorageObject obj;

        public PolylineData NoteLengthData {
            get { return notelengthdata; }
        }
        
        public TempoTrack(
                StorageObjectID storageobjectID,
                EditorFile file
            ) :
            base(
                    storageobjectID,
                    file,
                    null //TODO
                ) {
            obj = this.Object();

            notelengthdata = 
                new PolylineData(
                        obj.GetOrMake("note-length"),
                        file, 
                        2
                    );
        }

        public override void Bind() {
            notelengthdata.Bind();

            base.Bind();
        }

        public override void Unbind() {
            notelengthdata.Unbind();

            base.Unbind();
        }

        public Time GetTime(double seconds, Time tracklength) {
            // binary search
            //TODO: as well as Integrate(this IFunction), make Invert(this IFunction)
            // with a native interface for invertible functions. Use this to make the
            // PolyLine function integratable and its integration invertible.
            // Reciprocate the tempo, integrate that, then invert that so you can plug
            // and chug time (sec) for x and get time (notes) as y.
            var precision = 1.0 / (128 * 3 * 5 * 7);
            double pointer = tracklength.Notes / 2;
            double pointer_size = tracklength.Notes / 4;

            double integral, integral_discrepency;

            do {
                integral = notelengthdata.GetIntegratedValue(pointer);
                integral_discrepency = seconds - integral;

                if (integral_discrepency > precision)
                    pointer += pointer_size;
                else if (integral_discrepency < -precision)
                    pointer -= pointer_size;
                else break;

                pointer_size /= 2;
            } while (true);

            return Time.FromNotes(pointer);
        }

        public void SetTempo(Time time, double notes_per_minute) {
            notelengthdata.AddConstant(time.Notes, 60.0 / notes_per_minute);
        }
    }
}
