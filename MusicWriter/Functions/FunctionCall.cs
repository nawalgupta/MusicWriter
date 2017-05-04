using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FunctionCall
    {
        readonly double wavetime;
        readonly double localtime;
        readonly double realtime;

        public double Time {
            get { return wavetime; }
        }

        public double WaveTime {
            get { return wavetime; }
        }

        public double LocalTime {
            get { return localtime; }
        }

        public double RealTime {
            get { return realtime; }
        }
        
        public FunctionCall(
                double wavetime,
                double localtime,
                double realtime
            ) {
            this.wavetime = wavetime;
            this.localtime = localtime;
            this.realtime = realtime;
        }

        public FunctionCall(double time)
            : this(time, time, time) {
        }
    }
}
