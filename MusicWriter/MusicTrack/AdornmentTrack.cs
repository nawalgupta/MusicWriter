using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class AdornmentTrack {
        public DurationField<KeySignature> KeySignatures { get; } =
            new DurationField<KeySignature>();

        public DurationField<Staff> Staffs { get; } =
            new DurationField<Staff>();
    }
}
