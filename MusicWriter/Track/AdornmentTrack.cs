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

        public void SetStaff(Staff staff, Duration duration) {
            foreach (var oldstaff in Staffs.Intersecting(duration).ToArray()) {
                var intersection =
                    oldstaff.Duration.Intersection(duration);

                Staffs.Remove(oldstaff);

                if (intersection.Length != Time.Zero)
                    Staffs.Add(oldstaff.Value, intersection);
            }

            Staffs.Add(staff, duration);
        }

        public void SetKeySignature(KeySignature keysignature, Duration duration) {
            foreach (var oldkeysignature in KeySignatures.Intersecting(duration).ToArray()) {
                var intersection =
                    oldkeysignature.Duration.Intersection(duration);

                KeySignatures.Remove(oldkeysignature);

                if (intersection.Length != Time.Zero)
                    KeySignatures.Add(oldkeysignature.Value, intersection);
            }

            KeySignatures.Add(keysignature, duration);
        }
    }
}
