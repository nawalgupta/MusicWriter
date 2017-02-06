using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class Pin
    {
        readonly IStorageObject storage;
        readonly NamedTime time;
        readonly IOListener
            listener_marker_contentsset,
            listener_offset_contentsset;

        public IStorageObject Storage {
            get { return storage; }
        }

        public NamedTime Time {
            get { return time; }
        }

        public Pin(
                IStorageObject storage,
                TimeMarkerUnit timemarkerunit
            ) {
            this.storage = storage;
            time = new NamedTime(timemarkerunit);

            //TODO: see if this code can be replaced with a PropertyBinder object
            //TODO: see if changing time.MarkerName is the right action to do to rename the marker

            var marker_obj = storage.GetOrMake("marker");
            listener_marker_contentsset =
                marker_obj.Listen(IOEvent.ObjectContentsSet, () => {
                    time.MarkerName.Value = marker_obj.ReadAllString();
                });
            time.MarkerName.AfterChange += (old, @new) => {
                marker_obj.WriteAllString(@new);
            };

            var offset_obj = storage.GetOrMake("offset");
            offset_obj.WriteAllString("0");
            listener_offset_contentsset =
                offset_obj.Listen(IOEvent.ObjectContentsSet, () => {
                    time.Offset.Value = MusicWriter.Time.FromTicks(int.Parse(offset_obj.ReadAllString()));
                });
            time.Offset.AfterChange += (old, @new) => {
                offset_obj.WriteAllString(@new.Ticks.ToString());
            };
        }
    }
}
