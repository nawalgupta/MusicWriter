using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IViewer<T>
    {
        bool SupportsView(string type);

        object CreateView(T obj, string type);
    }
}
