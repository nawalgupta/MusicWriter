using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class TimeMarkerUnit
    {
        readonly IStorageObject storage;
        readonly Dictionary<string, ObservableProperty<Time>> markers =
            new Dictionary<string, ObservableProperty<Time>>();

        public IStorageObject Storage {
            get { return storage; }
        }

        public TimeMarkerUnit(IStorageObject storage) {
            this.storage = storage;
        }

        public ObservableProperty<Time> GetMarker(string name) =>
            markers[name];

        public Time GetTime(string name) =>
            markers[name].Value;

        public void AddMarker(
                string name,
                Time value,
                bool @private = false
            ) {
            var property =
                new ObservableProperty<Time>(value);

            if (!@private) {
                var obj =
                    storage.GetOrMake(name);

                obj.WriteAllString(property.Value.Ticks.ToString());

                property.AfterChange += (old, @new) => {
                    obj.WriteAllString(@new.Ticks.ToString());
                };

                obj.ContentsSet += delegate {
                    property.Value = Time.FromTicks(int.Parse(obj.ReadAllString()));
                };
            }

            markers.Add(name, property);
        }

        public void DeleteMarker(string name) {
            markers.Remove(name);
        }

        public void RenameMarker(string oldname, string newname) {
            var value = GetTime(oldname);

            //TODO: rename in a way that event handlers are still connected

            DeleteMarker(oldname);
            AddMarker(newname, value);
        }
    }
}
