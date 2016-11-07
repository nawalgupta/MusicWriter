using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class AdornmentTrack {
        readonly DurationField<KeySignature> keysignatures;

        public DurationField<KeySignature> KeySignatures {
            get { return keysignatures; }
        }

        public AdornmentTrack(
                DurationField<KeySignature> keysignatures
            ) {
            this.keysignatures = keysignatures;
        }
    }
}
