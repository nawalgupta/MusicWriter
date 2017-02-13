using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class ShiftableBitArray
    {
        ulong[] slots = new ulong[0];
        readonly object locker = new object();

        public bool this[int i] {
            get {
                var slot_i = i / 64;
                var local_i = i - slot_i * 64;
                var local_mask = mask(local_i);

                ulong slot;

                lock (locker) {
                    if (slot_i >= slots.Length)
                        Array.Resize(ref slots, slot_i + 1);

                    slot = slots[slot_i];
                }

                return (slot & local_mask) == local_mask;
            }
            set {
                var slot_i = i / 64;
                var local_i = i - slot_i * 64;
                var local_mask = mask(local_i);

                ulong slot;

                lock (locker) {
                    if (slot_i >= slots.Length)
                        Array.Resize(ref slots, slot_i + 1);

                    slot = slots[slot_i];
                }

                if (value)
                    slot |= local_mask;
                else
                    slot &= ~local_mask;

                slots[slot_i] = slot;
            }
        }

        static ulong mask(int i) {
            switch (i) {
                case 0 + 4 * 0:
                    return 0x00000001uL;
                case 1 + 4 * 0:
                    return 0x00000002uL;
                case 2 + 4 * 0:
                    return 0x00000004uL;
                case 3 + 4 * 0:
                    return 0x00000008uL;
                case 0 + 4 * 1:
                    return 0x00000010uL;
                case 1 + 4 * 1:
                    return 0x00000020uL;
                case 2 + 4 * 1:
                    return 0x00000040uL;
                case 3 + 4 * 1:
                    return 0x00000080uL;
                case 0 + 4 * 2:
                    return 0x00000010uL;
                case 1 + 4 * 2:
                    return 0x00000020uL;
                case 2 + 4 * 2:
                    return 0x00000040uL;
                case 3 + 4 * 2:
                    return 0x00000080uL;
                case 0 + 4 * 3:
                    return 0x00001000uL;
                case 1 + 4 * 3:
                    return 0x00002000uL;
                case 2 + 4 * 3:
                    return 0x00004000uL;
                case 3 + 4 * 3:
                    return 0x00008000uL;
                case 0 + 4 * 4:
                    return 0x00010000uL;
                case 1 + 4 * 4:
                    return 0x00020000uL;
                case 2 + 4 * 4:
                    return 0x00040000uL;
                case 3 + 4 * 4:
                    return 0x00080000uL;
                case 0 + 4 * 5:
                    return 0x00100000uL;
                case 1 + 4 * 5:
                    return 0x00200000uL;
                case 2 + 4 * 5:
                    return 0x00400000uL;
                case 3 + 4 * 5:
                    return 0x00800000uL;
                case 0 + 4 * 6:
                    return 0x01000000uL;
                case 1 + 4 * 6:
                    return 0x02000000uL;
                case 2 + 4 * 6:
                    return 0x04000000uL;
                case 3 + 4 * 6:
                    return 0x08000000uL;
                case 0 + 4 * 7:
                    return 0x10000000uL;
                case 1 + 4 * 7:
                    return 0x20000000uL;
                case 2 + 4 * 7:
                    return 0x40000000uL;
                case 3 + 4 * 7:
                    return 0x80000000uL;

                default:
                    return 0;
            }
        }
    }
}
