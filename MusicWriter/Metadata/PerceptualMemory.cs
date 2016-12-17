using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class PerceptualMemory {
        readonly Dictionary<Type, IMemoryModule<object>> memorymodules =
            new Dictionary<Type, IMemoryModule<object>>();
        readonly Dictionary<Type, int> refcounts =
            new Dictionary<Type, int>();

        public void InsertMemoryModule<T>(IMemoryModule<T> module) where T : class {
            var type =
                typeof(T);

            if (memorymodules.ContainsKey(type))
                refcounts[type]++;
            else {
                refcounts.Add(type, 1);
                memorymodules.Add(type, module);
            }
        }

        public void RemoveMemoryModule<T>() {
            var type =
                typeof(T);

            if (--refcounts[type] == 0) {
                memorymodules.Remove(type);
                refcounts.Remove(type);
            }
        }

        public IMemoryModule<T> MemoryModule<T>() {
            var type =
                typeof(T);

            IMemoryModule<object> module;

            if (!memorymodules.TryGetValue(type, out module))
                return null;

            var typedmodule =
                module as IMemoryModule<T>;

            return typedmodule;
        }

        public IEnumerable<IDuratedItem<T>> Analyses<T>(Duration duration) {
            var module =
                MemoryModule<T>();

            if (module == null)
                return Enumerable.Empty<IDuratedItem<T>>();

            return module.Knowledge.Intersecting(duration);
        }

        public IEnumerable<IDuratedItem<T>> Analyses<T>(Time point) {
            var module =
                MemoryModule<T>();

            if (module == null)
                return Enumerable.Empty<IDuratedItem<T>>();

            return module.Knowledge.Intersecting(point);
        }

        public void Forget(Duration duration) {
            foreach (var module in memorymodules.Values)
                module.Forget(duration);
        }

        public void Forget<T>(Duration duration) =>
            MemoryModule<T>().Forget(duration);
    }
}
