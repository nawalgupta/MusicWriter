using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class StepwiseIntegratedFunction : IFunction
    {
        readonly Time precision;
        readonly IFunction derivative;

        public Time Precision {
            get { return precision; }
        }

        public IFunction Derivative {
            get { return derivative; }
        }

        public StepwiseIntegratedFunction(
                Time precision,
                IFunction derivative
            ) {
            this.precision = precision;
            this.derivative = derivative;
        }

        public float GetValue(FunctionCall arg) {
            float accumulator = 0;

            Time now = Time.Zero;

            var precision_notes =
                precision.Notes;

            while (now < arg.LocalTime) {
                accumulator += precision_notes * derivative.GetValue(new FunctionCall(now));
                now += precision;
            }

            return accumulator;
        }
    }
}
