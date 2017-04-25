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

        public static void AddRange<T>(
                this IObservableList<T> list,
                IEnumerable<T> range
            ) {
            foreach (var item in range)
                list.Add(item);
        }
    }
}
