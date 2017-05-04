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
        readonly double precision;
        readonly IFunction derivative;

        public double Precision {
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

            public string FriendlyName {
                get { return "Stepwise Function Integral"; }
            }

            public bool StoresBinaryData {
                get { return false; }
            }

            public IFunction Create(
                    IFunction context = null,
                    IFunction[] args = null,
                    EditorFile file = null,
                    string key = null,
                    params double[] numbers
                ) =>
                new StepwiseIntegratedFunction(
                        numbers?.Length == 1 ?
                            numbers[0] : 
                            1 / 256.0,
                        context
                    );

            private FactoryClass() { }

            public static readonly IFunctionFactory Instance = new FactoryClass();
        }

        public StepwiseIntegratedFunction(
                double precision,
                IFunction derivative
            ) {
            this.precision = precision;
            this.derivative = derivative;
        }

        public double GetValue(FunctionCall arg) {
            var accumulator = 0.0;
            var now = 0.0;
            
            while (now < arg.LocalTime) {
                accumulator += precision * derivative.GetValue(new FunctionCall(now));
                now += precision;
            }

            return accumulator;
        }
    }
}
