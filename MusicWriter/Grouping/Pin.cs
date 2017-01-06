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

            var marker_obj = storage.GetOrMake("marker");
            marker_obj.ContentsSet += delegate {
                time.MarkerName.Value = marker_obj.ReadAllString();
            };
            time.MarkerName.AfterChange += (old, @new) => {
                marker_obj.WriteAllString(@new);
            };

            var offset_obj = storage.GetOrMake("offset");
            offset_obj.ContentsSet += delegate {
                time.Offset.Value = MusicWriter.Time.FromTicks(int.Parse(offset_obj.ReadAllString()));
            };
            time.Offset.AfterChange += (old, @new) => {
                offset_obj.WriteAllString(@new.Ticks.ToString());
            };
        }
    }
}
