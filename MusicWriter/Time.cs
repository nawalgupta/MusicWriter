using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public struct Time {
        int ticks;
        
        public double Notes {
            get { return (double)ticks / TicksPerNote; }
        }

        public Time Double {
            get { return new Time(ticks * 2); }
        }

        public Time Triple {
            get { return new Time(ticks * 3); }
        }

        public Time Fifth {
            get { return new Time(ticks * 5); }
        }

        public Time Seventh {
            get { return new Time(ticks * 7); }
        }

        public Time(int ticks = 0) {
            this.ticks = ticks;
        }

        public const int TicksPerNote_2nd = TicksPerNote / 2;
        public const int TicksPerNote_4th = TicksPerNote / 4;
        public const int TicksPerNote_8th = TicksPerNote / 8;
        public const int TicksPerNote_16th = TicksPerNote / 16;
        public const int TicksPerNote_32nd = TicksPerNote / 32;
        public const int TicksPerNote_64th = TicksPerNote / 64;
        public const int TicksPerNote_128th = TicksPerNote / 128;
        public const int TicksPerNote_3rd = TicksPerNote / 3;
        public const int TicksPerNote_5th = TicksPerNote / 5;
        public const int TicksPerNote_7th = TicksPerNote / 7;

        public const int TicksPerNote = 2 * 2 * 2 * 2 * 2 * 2 * 2 * 3 * 5 * 7; // 128th notes, 3rd notes, 5th notes, 7th notes
    }
}
