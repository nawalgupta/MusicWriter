using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public interface IPerceptualCog<out T> {
        void Analyze(
                Duration delta,
                MusicBrain brain
            );

        void Forget(Duration delta);

        IDurationField<T> Knowledge { get; }
    }
}
