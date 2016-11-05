using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public struct Time : 
        IComparable<Time>,
        IEquatable<Time> {
        int ticks;
        
        public int Ticks {
            get { return ticks; }
        }
        
        public float Notes {
            get { return (float)ticks / TicksPerNote; }
            set { ticks = (int)(value * TicksPerNote); }
        }

        public Time? Half {
            get {
                const int div = 2;

                if ((ticks % div) != 0)
                    return null;

                return new Time(ticks / div);
            }
        }

        public Time? Third {
            get {
                const int div = 3;

                if ((ticks % div) != 0)
                    return null;

                return new Time(ticks / div);
            }
        }

        public Time? Fifth {
            get {
                const int div = 5;

                if ((ticks % div) != 0)
                    return null;

                return new Time(ticks / div);
            }
        }

        public Time? Seventh {
            get {
                const int div = 7;

                if ((ticks % div) != 0)
                    return null;

                return new Time(ticks / div);
            }
        }
        
        private Time(int ticks) {
            this.ticks = ticks;
        }

        public Time(Time copy) {
            ticks = copy.ticks;
        }

        private const int TicksPerNote_2nd = TicksPerNote / 2;
        private const int TicksPerNote_4th = TicksPerNote / 4;
        private const int TicksPerNote_8th = TicksPerNote / 8;
        private const int TicksPerNote_16th = TicksPerNote / 16;
        private const int TicksPerNote_32nd = TicksPerNote / 32;
        private const int TicksPerNote_64th = TicksPerNote / 64;
        private const int TicksPerNote_128th = TicksPerNote / 128;
        private const int TicksPerNote_3rd = TicksPerNote / 3;
        private const int TicksPerNote_5th = TicksPerNote / 5;
        private const int TicksPerNote_7th = TicksPerNote / 7;

        private const int TicksPerNote = 2 * 2 * 2 * 2 * 2 * 2 * 2 * 3 * 5 * 7; // 128th notes, 3rd notes, 5th notes, 7th notes

        public static readonly Time Zero = new Time(0);
        public static readonly Time Note = new Time(TicksPerNote);
        public static readonly Time Note128th_3rd_5th_7th = new Time(TicksPerNote_128th / (3 * 5 * 7));

        public static Time Fraction(int numerator, int denominator) =>
            new Time(TicksPerNote * numerator / denominator);

        public override bool Equals(object obj) =>
            obj is Time &&
            Equals((Time)obj);

        public bool Equals(Time that) =>
            this == that;

        public override int GetHashCode() =>
            ticks;

        public static Time operator +(Time a, Time b) =>
            new Time(a.ticks + b.ticks);

        public static Time operator -(Time a, Time b) =>
            new Time(a.ticks - b.ticks);

        public static Time operator *(int a, Time b) =>
            new Time(a * b.ticks);

        public static Time operator *(Time a, int b) =>
            new Time(a.ticks * b);

        public static int operator /(Time a, Time b) =>
            a.ticks / b.ticks;

        public static Time operator /(Time a, int b) =>
            new Time(a.ticks / b);

        public static Time operator %(Time a, Time b) =>
            new Time(a.ticks % b.ticks);

        public static bool operator <(Time a, Time b) =>
            a.ticks < b.ticks;

        public static bool operator >(Time a, Time b) =>
            a.ticks > b.ticks;

        public static bool operator <=(Time a, Time b) =>
            a.ticks <= b.ticks;

        public static bool operator >=(Time a, Time b) =>
            a.ticks >= b.ticks;

        public static bool operator ==(Time a, Time b) =>
            a.ticks == b.ticks;

        public static bool operator !=(Time a, Time b) =>
            a.ticks != b.ticks;

        public int CompareTo(Time other) =>
            ticks.CompareTo(other.ticks);
    }
}
