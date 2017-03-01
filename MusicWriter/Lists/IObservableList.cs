using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IObservableList<T> : IList<T>, IRearrangeableList
    {
        event ObservableListDelegates<T>.ItemAdded ItemAdded;
        event ObservableListDelegates<T>.ItemInserted ItemInserted;
        
        event ObservableListDelegates<T>.ItemRemoved ItemRemoved;
        event ObservableListDelegates<T>.ItemWithdrawn ItemWithdrawn;

        event ObservableListDelegates<T>.ItemMoved ItemMoved;

        bool HasItemAt(int i);
    }
}
