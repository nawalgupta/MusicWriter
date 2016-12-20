using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class StepwiseIntegratedFunction : IFunction
    {
        readonly float precision;
        readonly IFunction derivative;

        public float Precision {
            get { return precision; }
        }

        public IFunction Derivative {
            get { return derivative; }
        }

        public IFunctionFactory Factory {
            get { return FactoryClass.Instance; }
        }

        public sealed class FactoryClass : IFunctionFactory
        {
            public bool AcceptsParameters {
                get { return true; }
            }

            public string CodeName {
                get { return "step-integral"; }
            }

            public string Name {
                get { return "Stepwise Function Integral"; }
            }

            public bool StoresBinaryData {
                get { return false; }
            }

            public IFunction Create() {
                throw new InvalidOperationException();
            }

            public IFunction Create(float[] args) {
                throw new InvalidOperationException();
            }

            public IFunction Create(IFunction[] args) {
                throw new InvalidOperationException();
            }

            public IFunction Create(IFunction context) =>
                Create(context, 1f / 256f);

            public IFunction Create(IFunction context, params float[] args) =>
                new StepwiseIntegratedFunction(args[0], context);

            public IFunction Deserialize(Stream stream) {
                throw new InvalidOperationException();
            }

            public void Serialize(Stream stream, IFunction function) {
                throw new InvalidOperationException();
            }

            private FactoryClass() { }

            public static readonly IFunctionFactory Instance = new FactoryClass();
        }

        public StepwiseIntegratedFunction(
                float precision,
                IFunction derivative
            ) {
            this.precision = precision;
            this.derivative = derivative;
        }

        public float GetValue(FunctionCall arg) {
            float accumulator = 0;
            float now = 0;
            
            while (now < arg.LocalTime) {
                accumulator += precision * derivative.GetValue(new FunctionCall(now));
                now += precision;
            }

            return accumulator;
        }
    }
}
