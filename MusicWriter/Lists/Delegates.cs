using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public static class ObservableListDelegates<T>
    {
        public delegate void ItemAdded(T item);
        public delegate void ItemRemoved(T item);
        public delegate void ItemInserted(T item, int index);
        public delegate void ItemWithdrawn(T item, int index);
        public delegate void ItemMoved(T item, int oldindex, int newindex);
    }
}
