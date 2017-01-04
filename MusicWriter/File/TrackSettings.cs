using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class TrackSettings
    {
        readonly IStorageObject storage;
        readonly PropertyManager propertymanager;
        readonly MusicBrain musicbrain;

        public IStorageObject Storage {
            get { return storage; }
        }

        public PropertyManager PropertyManager {
            get { return propertymanager; }
        }

        public MusicBrain MusicBrain {
            get { return musicbrain; }
        }

        public TrackSettings(IStorageObject storage) {
            this.storage = storage;

            propertymanager = new PropertyManager(storage.GetOrMake("property-manager"));

            musicbrain = new MusicBrain();
            musicbrain.InsertCog(new NotePerceptualCog());
            musicbrain.InsertCog(new MeasureLayoutPerceptualCog());
        }
    }
}
