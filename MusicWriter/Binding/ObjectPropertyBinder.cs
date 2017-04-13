using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class ObjectPropertyBinder<T>
        : BoundObject<ObjectPropertyBinder<T>>
        where T : IBoundObject<T>
    {
        readonly string relation;
        readonly BoundList<T> boundlist;
        readonly IStorageObject obj;
        readonly IOListener
            listener_add,
            listener_remove;
        readonly ObservableProperty<T> value;

        public string Relation {
            get { return relation; }
        }

        public BoundList<T> BoundList {
            get { return boundlist; }
        }

        public ObservableProperty<T> Value {
            get { return value; }
        }

        public ObjectPropertyBinder(
                StorageObjectID storageobjectID,
                EditorFile file,
                string relation,
                BoundList<T> boundlist,
                ObservableProperty<T> value
            ) :
            base(
                    storageobjectID,
                    file,
                    null
                ) {
            this.relation = relation;
            this.boundlist = boundlist;
            this.value = value;

            obj = file.Storage[storageobjectID];

            listener_add =
                file
                    .Storage
                    .CreateListen(
                            msg => Value.Value = boundlist[msg.Object],
                            subject: storageobjectID,
                            verb: IOEvent.ChildAdded,
                            key: relation
                        );

            listener_remove =
                file
                    .Storage
                    .CreateListen(
                            msg => {
                                if (Value.Value.StorageObjectID == msg.Object)
                                    Value.Value = default(T);
                            },
                            subject: storageobjectID,
                            verb: IOEvent.ChildRemoved,
                            key: relation
                        );
        }

        public override void Bind() {
            Value.AfterChange += Value_AfterChange;

            File.Storage.Listeners.Add(listener_add);
            File.Storage.Listeners.Add(listener_remove);

            base.Bind();
        }

        public override void Unbind() {
            Value.AfterChange -= Value_AfterChange;

            File.Storage.Listeners.Remove(listener_add);
            File.Storage.Listeners.Remove(listener_remove);

            base.Unbind();
        }

        private void Value_AfterChange(T old, T @new) {
            obj.Remove(relation);

            if (@new != null)
                obj.Add(relation, @new.StorageObjectID);
        }
    }
}
