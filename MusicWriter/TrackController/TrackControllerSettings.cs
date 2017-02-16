using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class TrackControllerSettings : BoundObject<TrackControllerSettings>
    {
        readonly IStorageObject storage;
        readonly PropertyManager propertymanager;
        readonly MusicBrain musicbrain;
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

        public TimeMarkerUnit TimeMarkerUnit {
            get { return timemarkerunit; }
        }

        public GlobalCaret GlobalCaret {
            get { return globalcaret; }
        }

        public TrackControllerSettings(
                IStorageObject storage,
                EditorFile file
            ):
            base(
                    storage.ID,
                    file,
                    null
                ) {
            this.storage = storage;

            propertymanager = new PropertyManager(storage.GetOrMake("property-manager"), file);

            musicbrain = new MusicBrain();
            musicbrain.InsertCog(new NotePerceptualCog());
            musicbrain.InsertCog(new MeasureLayoutPerceptualCog());

            timemarkerunit = new TimeMarkerUnit(storage.GetOrMake("time-markers"), file);

            globalcaret = new GlobalCaret(timemarkerunit);
        }

        public override void Bind() {
            propertymanager.Bind();
            timemarkerunit.Bind();

            base.Bind();
        }

        public override void Unbind() {
            propertymanager.Unbind();
            timemarkerunit.Unbind();

            base.Unbind();
        }
    }
}
