using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FunctionCall
    {
        readonly float localtime;
        readonly float realtime;

        public float LocalTime {
            get { return localtime; }
        }

        public float RealTime {
            get { return realtime; }
        }

        public FunctionCall(
                float localtime,
                float realtime
            ) {
            this.localtime = localtime;
            this.realtime = realtime;
        }

        public FunctionCall(float time)
            : this(time, time) {
        }
    }
}
