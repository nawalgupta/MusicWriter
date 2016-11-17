using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface ITrack {
        ObservableProperty<string> Name { get; }

        void Erase(Duration window);

        object Copy(Duration window);
        
        void Paste(object data, Time insert);
    }
}
