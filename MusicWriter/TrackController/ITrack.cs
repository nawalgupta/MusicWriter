using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface ITrack : 
        IBoundObject<ITrack>,
        INamedObject
    {
        ObservableProperty<Time> Length { get; }
        
        TrackControllerSettings Settings { get; }
        
        event FieldChangedDelegate Dirtied;

        void Erase(Duration window);
        void Delete(Duration window);

        object Copy(Duration window);
        void Paste(object data, Time insert);
    }
}
