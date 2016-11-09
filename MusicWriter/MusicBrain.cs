using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class MusicBrain {
        readonly List<IPerceptualCog<object>> cogs =
            new List<IPerceptualCog<object>>();
        readonly IPropertyGraphlet<Note> noteproperties;

        public List<IPerceptualCog<object>> Cogs {
            get { return cogs; }
        }

        public IPropertyGraphlet<Note> NoteProperties {
            get { return noteproperties; }
        }

        public MusicBrain(
                IPropertyGraphlet<Note> noteproperties
            ) {
            this.noteproperties = noteproperties;
        }

        public IEnumerable<T> Anlyses<T>(Duration duration) {
            foreach (var cog in cogs) {
                var typedcog =
                    cog as IPerceptualCog<T>;

                if (typedcog != null)
                    foreach (var analysis in typedcog.Knowledge.Intersecting(duration))
                        yield return analysis;
            }
        }

        public IEnumerable<T> Anlyses<T>(Time point) {
            foreach (var cog in cogs) {
                var typedcog =
                    cog as IPerceptualCog<T>;

                if (typedcog != null)
                    foreach (var analysis in typedcog.Knowledge.Intersecting(point))
                        yield return analysis;
            }
        }
    }
}
