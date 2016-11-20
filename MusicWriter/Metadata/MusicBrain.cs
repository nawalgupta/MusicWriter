using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class MusicBrain {
        readonly Dictionary<Type, IPerceptualCog<object>> cogs =
            new Dictionary<Type, IPerceptualCog<object>>();
        readonly Dictionary<Type, int> refcounts =
            new Dictionary<Type, int>();
        
        public void InsertCog<T>(IPerceptualCog<T> cog) where T : class {
            var type = typeof(T);

            if (cogs.ContainsKey(type))
                refcounts[type]++;
            else {
                cogs.Add(type, cog);
                refcounts.Add(type, 1);
            }
        }
        
        public void RemoveCog<T>() {
            var type = typeof(T);

            if (--refcounts[type] == 1) {
                cogs.Remove(type);
                refcounts.Remove(type);
            }
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
