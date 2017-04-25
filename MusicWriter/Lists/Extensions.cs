using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public static class ListExtensions
    {
        public static void Bind<T>(
                this IObservableList<T> source,
                IObservableList<T> sink
            ) {
            source.ItemAdded += sink.Add;
            source.ItemRemoved += _ => sink.Remove(_);
        }
    }
}
