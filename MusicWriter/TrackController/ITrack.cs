using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface ITrack {
        ObservableProperty<string> Name { get; }

        ObservableProperty<Time> Length { get; }

        StorageObjectID StorageObjectID { get; }

        GlobalSettings Settings { get; }

        ITrackFactory Factory { get; }

        event FieldChangedDelegate Dirtied;

        void Erase(Duration window);
        void Delete(Duration window);

        object Copy(Duration window);
        void Paste(object data, Time insert);
    }
}
