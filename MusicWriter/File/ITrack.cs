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

        TrackSettings Settings { get; }

        ITrackFactory Factory { get; }

        event Action Dirtied;

        void Erase(Duration window);
        void Delete(Duration window);

        object Copy(Duration window);
        void Paste(object data, Time insert);
    }
}
