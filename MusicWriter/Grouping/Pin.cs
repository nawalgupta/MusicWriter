using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class Pin : BoundObject<Pin>
    {
        readonly NamedTime time;
        readonly PropertyBinder<string> binder_markername;
        readonly PropertyBinder<Time> binder_offset;

        public NamedTime Time {
            get { return time; }
        }

        public Pin(
                StorageObjectID storageobjectID,
                EditorFile file,
                TimeMarkerUnit timemarkerunit
            ) :
            base(
                    storageobjectID,
                    file,
                    null //TODO
                ) {
            time = new NamedTime(timemarkerunit);
            
            //TODO: see if changing time.MarkerName is the right action to do to rename the marker

            var obj = this.Object();
            
            binder_markername = time.MarkerName.Bind(obj.GetOrMake("marker"));
            binder_offset = time.Offset.Bind(obj.GetOrMake("offset"));
        }
    }
}
