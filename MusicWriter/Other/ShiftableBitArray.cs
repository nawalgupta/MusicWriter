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
                        return false;

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

        public IEnumerable<int> AllOnes() {
            for (int i = 0; i < 64 * slots.Length; i += 64)
                foreach (var index in relative_indicies(slots[i / 64]))
                    yield return index + i;
        }

        public IEnumerable<int> AllOnes(ShiftableBitArray mask) {
            var min = Math.Min(slots.Length, mask.slots.Length);

            for (int i = 0; i < 64 * min; i += 64)
                foreach (var index in relative_indicies(slots[i / 64] & mask.slots[i / 64]))
                    yield return index + i;
        }

        static IEnumerable<int> relative_indicies(ulong x) {
            for (int i = 0; i < 64; i+=8) {
                byte m = (byte)(x & 0xFF);

                if ((m & 0xF0) != 0) {
                    if ((m & 0xC0) != 0) {
                        if ((m & 0x80) != 0) {
                            yield return i + 7;
                        }
                        if ((m & 0x40) != 0) {
                            yield return i + 6;
                        }
                    }
                    if ((m & 0x30) != 0) {
                        if ((m & 0x20) != 0) {
                            yield return i + 5;
                        }
                        if ((m & 0x10) != 0) {
                            yield return i + 4;
                        }
                    }
                }
                if ((m & 0x0F) != 0) {
                    if ((m & 0x0C) != 0) {
                        if ((m & 0x08) != 0) {
                            yield return i + 3;
                        }
                        if ((m & 0x04) != 0) {
                            yield return i + 2;
                        }
                    }
                    if ((m & 0x03) != 0) {
                        if ((m & 0x02) != 0) {
                            yield return i + 1;
                        }
                        if ((m & 0x01) != 0) {
                            yield return i + 0;
                        }
                    }
                }

                x >>= 8;
            }
        }

        public void Clear() {
            lock (locker) {
                Array.Resize(ref slots, 0);
            }
        }

        static ulong mask(int i) => 1uL << i;

        static ulong mask_after(int i) => (0xFFFFFFFFFFFFFFFFuL >> i) << i;
        
        public void Insert(int i, bool value) {
            if (i >= 64 * slots.Length)
                this[i] = value;
            else {
                lock (locker) {
                    var local_i = i % 64;

                    for (var slot_i = i / 64; slot_i < slots.Length; slot_i++) {
                        var slot = slots[slot_i];
                        var mask_a = mask_after(local_i);
                        var mask_i = mask(local_i);

                        slots[slot_i] =
                            ((slot & mask_a) << 1) |
                            (slot & ~(mask_a | mask_i)) |
                            (value ? mask_i : 0);

                        value = (slot & mask(63)) != 0;
                        local_i = 0;
                    }
                }
            }
        }

        public void Withdraw(int i) {
            if (i >= 64 * slots.Length)
                this[i] = false;
            else {
                lock (locker) {
                    var local_i = i % 64;
                    
                    for (var slot_i = i / 64; slot_i < slots.Length; slot_i++) {
                        var slot = slots[slot_i];
                        var mask_a = mask_after(local_i);
                        var mask_i = mask(local_i);
                        var next_bit = this[slot_i * 64 + 64];

                        slots[slot_i] =
                            ((slot & (mask_a & ~mask_i)) >> 1) |
                            (slot & ~(mask_a | mask_i)) |
                            (next_bit ? (1uL << 63) : 0uL);
                        
                        local_i = 0;
                    }
                }
            }
        }
    }
}
