using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class IgnorantPerceptualCog<T> : IPerceptualCog<T> {
        public IDurationField<T> Knowledge { get; set; }

        public IgnorantPerceptualCog(IDurationField<T> knowledge = null) {
            Knowledge = knowledge;
        }

        public bool Analyze(Duration delta, MusicBrain brain) => false;

        public void Forget(Duration delta) {
        }
    }
}
