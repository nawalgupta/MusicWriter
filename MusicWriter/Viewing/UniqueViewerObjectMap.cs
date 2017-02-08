using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class UniqueViewerObjectMap<T>
        where T : IBoundObject<T>
    {
        readonly BoundList<T> boundlist;
        readonly Dictionary<string, Dictionary<T, object>> viewersmap =
            new Dictionary<string, Dictionary<T, object>>();

        public BoundList<T> BoundList {
            get { return boundlist; }
        }

        public UniqueViewerObjectMap(
                BoundList<T> boundlist
            ) {
            this.boundlist = boundlist;
        }

        public object this[T obj, string view] {
            get {
                return
                    viewersmap
                        .Lookup(view)
                        .Lookup(
                                obj, 
                                () => 
                                    boundlist
                                        .ViewerSet
                                        .CreateView(obj, view)
                            );
            }
        }
    }
}
