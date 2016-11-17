using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class MusicBrain {
        readonly Dictionary<Type, IPerceptualCog<object>> cogs =
            new Dictionary<Type, IPerceptualCog<object>>();
        
        public void InsertCog<T>(IPerceptualCog<T> cog) where T : class {
            var type = typeof(T);

            cogs.Add(type, cog);
        }
        
        public void RemoveCog<T>() {
            var type = typeof(T);

            cogs.Remove(type);
        }

        public void Invalidate(
                PerceptualMemory memory,
                Duration duration
            ) {
            foreach (var cog in cogs.Values)
                memory.Forget(duration);
            
            bool flag;
            do {
                flag = false;

                foreach (var cog in cogs.Values)
                    flag |= cog.Analyze(duration, this, memory);
            }
            while (flag);
        }
    }
}
