using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IIOMessageReactor
    {
        IObservableList<IOMessage> Messages { get; }
        IObservableList<IOListener> Listeners { get; }
    }
}
