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
        readonly HashSet<string> privatemarkers =
            new HashSet<string>();

        public IStorageObject Storage {
            get { return storage; }
        }

        public TimeMarkerUnit(IStorageObject storage) {
            this.storage = storage;

            Setup();
        }

        void Setup() {
            storage.ChildAdded += (storage_objID, newmarker_objID, key) => {
                var name = key;
                var time = Time.FromTicks(int.Parse(storage.Graph[newmarker_objID].ReadAllString()));

                markers.Add(name, new ObservableProperty<Time>(time));
            };

            storage.ChildRenamed += (storage_objID, marker_objID, oldkey, newkey) => {
                var oldname = oldkey;
                var newname = newkey;

                var prop = markers[oldname];
                markers.Remove(oldname);
                markers.Add(newname, prop);
            };

            storage.ChildContentsSet += (storage_objID, marker_objID, key) => {
                var name = key;
                var newtime = Time.FromTicks(int.Parse(storage.Graph[marker_objID].ReadAllString()));

                markers[name].Value = newtime;
            };

            storage.ChildRemoved += (storage_objID, oldmarker_objID, key) => {
                var name = key;

                markers.Remove(name);
            };
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
            if (!@private) {
                var obj =
                    storage.Graph.CreateObject();

                obj.WriteAllString(value.Ticks.ToString());

                storage.Add(name, obj.ID);
            }
            else {
                markers.Add(name, new ObservableProperty<Time>(value));
            }
        }

        public void DeleteMarker(string name) {
            if (!privatemarkers.Contains(name))
                storage.Get(name).Delete();
            else {
                markers.Remove(name);
            }
        }

        public bool HasMarker(string name) {
            return markers.ContainsKey(name);
        }

        public void SetMarker(string name, Time value) {
            if (!privatemarkers.Contains(name))
                storage.Get(name).WriteAllString(value.Ticks.ToString());
            else {
                markers[name].Value = value;
            }
        }

        public void RenameMarker(string oldname, string newname) {
            if (!privatemarkers.Contains(oldname))
                storage.Rename(oldname, newname);
            else {
                var prop = markers[oldname];
                markers.Add(newname, prop);
                markers.Remove(oldname);
            }
        }
    }
}
