using MusicWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class TimeMarkerUnit : BoundObject<TimeMarkerUnit>
    {
        readonly IStorageObject storage;
        readonly Dictionary<string, ObservableProperty<Time>> markers =
            new Dictionary<string, ObservableProperty<Time>>();
        readonly HashSet<string> privatemarkers =
            new HashSet<string>();
        readonly IOListener
            listener_added,
            listener_rekeyed,
            listener_contentsset,
            listener_removed;

        public IStorageObject Storage {
            get { return storage; }
        }

        public TimeMarkerUnit(
                IStorageObject storage,
                EditorFile file
            ) :
            base(
                    storage.ID,
                    file,
                    null
                ) {
            this.storage = storage;

            listener_added =
                storage.CreateListen(IOEvent.ChildAdded, (key, newmarker_objID) => {
                    var name = key;
                    var time = Time.FromTicks(int.Parse(storage.Graph[newmarker_objID].ReadAllString()));

                    markers.Add(name, new ObservableProperty<Time>(time));
                });

            listener_rekeyed =
                storage
                    .Graph
                    .CreateListen(
                        msg => {
                            var oldname = msg.Relation;
                            var newname = msg.NewRelation;

                            var prop = markers[oldname];
                            markers.Remove(oldname);
                            markers.Add(newname, prop);
                        },
                        storage.ID,
                        IOEvent.ChildRekeyed
                    );

            listener_contentsset =
                storage.CreateListen(IOEvent.ChildContentsSet, (key, marker_objID) => {
                    var name = key;
                    var newtime = Time.FromTicks(int.Parse(storage.Graph[marker_objID].ReadAllString()));

                    markers[name].Value = newtime;
                });

            listener_removed =
                storage.CreateListen(IOEvent.ChildRemoved, (name, oldmarker_objID) => {
                    markers.Remove(name);
                });
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

        public override void Bind() {
            storage.Graph.Listeners.Add(listener_added);
            storage.Graph.Listeners.Add(listener_rekeyed);
            storage.Graph.Listeners.Add(listener_contentsset);
            storage.Graph.Listeners.Add(listener_removed);

            base.Bind();
        }

        public override void Unbind() {
            storage.Graph.Listeners.Remove(listener_added);
            storage.Graph.Listeners.Remove(listener_rekeyed);
            storage.Graph.Listeners.Remove(listener_contentsset);
            storage.Graph.Listeners.Remove(listener_removed);

            base.Unbind();
        }
    }
}
