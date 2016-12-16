using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FunctionCall
    {
        readonly Time localtime;
        readonly Time realtime;

        public Time LocalTime {
            get { return localtime; }
        }

        public Time RealTime {
            get { return realtime; }
        }

        public FunctionCall(
                Time localtime,
                Time realtime
            ) {
            this.localtime = localtime;
            this.realtime = realtime;
        }

        public FunctionCall(Time time)
            : this(time, time) {
        }
    }
}
