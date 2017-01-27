using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class TrackControllerSettings
    {
        readonly IStorageObject storage;
        readonly PropertyManager propertymanager;
        readonly MusicBrain musicbrain;
        readonly AssortedFilesManager assortedfilesmanager;
        readonly TimeMarkerUnit timemarkerunit;
        readonly GlobalCaret globalcaret;

        public IStorageObject Storage {
            get { return storage; }
        }

        public PropertyManager PropertyManager {
            get { return propertymanager; }
        }

        public MusicBrain MusicBrain {
            get { return musicbrain; }
        }

        public AssortedFilesManager AssortedFilesManager {
            get { return assortedfilesmanager; }
        }

        public TimeMarkerUnit TimeMarkerUnit {
            get { return timemarkerunit; }
        }

        public GlobalCaret GlobalCaret {
            get { return globalcaret; }
        }

        public TrackControllerSettings(IStorageObject storage) {
            this.storage = storage;

            propertymanager = new PropertyManager(storage.GetOrMake("property-manager"));

            musicbrain = new MusicBrain();
            musicbrain.InsertCog(new NotePerceptualCog());
            musicbrain.InsertCog(new MeasureLayoutPerceptualCog());

            assortedfilesmanager = new AssortedFilesManager(storage.GetOrMake("assorted-files"));

            timemarkerunit = new TimeMarkerUnit(storage.GetOrMake("time-markers"));

            globalcaret = new GlobalCaret(timemarkerunit);
        }
    }
}
