using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class MusicBrain {
        readonly Dictionary<Type, IPerceptualCog<object>> cogs =
            new Dictionary<Type, IPerceptualCog<object>>();
        readonly IPropertyGraphlet<NoteID> noteproperties;
        readonly Track track;

        public IPropertyGraphlet<NoteID> NoteProperties {
            get { return noteproperties; }
        }

        public Track Track {
            get { return track; }
        }

        public MusicBrain(
                IPropertyGraphlet<NoteID> noteproperties,
                Track track
            ) {
            this.noteproperties = noteproperties;
            this.track = track;
        }

        public IEnumerable<IDuratedItem<T>> Anlyses<T>(Duration duration) {
            IPerceptualCog<object> cog;

            if (!cogs.TryGetValue(typeof(T), out cog))
                return Enumerable.Empty<IDuratedItem<T>>();

            var typedcog =
                (IPerceptualCog<T>)cog;

            return typedcog.Knowledge.Intersecting(duration);
        }

        public void InsertCog<T>(IPerceptualCog<T> cog) where T : class {
            var type = typeof(T);

            cogs.Add(type, cog);
        }

        public IEnumerable<IDuratedItem<T>> Anlyses<T>(Time point) {
            IPerceptualCog<object> cog;

            if (!cogs.TryGetValue(typeof(T), out cog))
                return Enumerable.Empty<IDuratedItem<T>>();

            var typedcog =
                (IPerceptualCog<T>)cog;

            return typedcog.Knowledge.Intersecting(point);
        }

        public void Invalidate(Duration duration) {
            foreach (var cog in cogs.Values)
                cog.Forget(duration);
            
            bool flag;
            do {
                flag = false;

                foreach (var cog in cogs.Values)
                    flag |= cog.Analyze(duration, this);
            }
            while (flag);
        }
    }
}
