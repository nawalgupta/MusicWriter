using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class PulseWidthModulatedFunction :
        IFunction,
        IContextualFunction,
        IParamaterizedFunction
    {
        readonly double activetime;
        readonly IFunction context;

        public double ActiveTime {
            get { return activetime; }
        }

        public IFunction Context {
            get { return context; }
        }

        public double[] Arguments {
            get { return new double[] { activetime }; }
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
                get { return "pwm"; }
            }

            public string FriendlyName {
                get { return "Pulse Width Modulated Function"; }
            }

            public bool StoresBinaryData {
                get { return false; }
            }

            private FactoryClass() {
            }

            public IFunction Create(
                    IFunction context = null,
                    IFunction[] args = null,
                    EditorFile file = null,
                    string key = null,
                    params double[] numbers
                ) =>
                new PulseWidthModulatedFunction(
                        numbers.Length == 1 ?
                            numbers[0] :
                            1,
                        context
                    );

            public static readonly IFunctionFactory Instance = new FactoryClass();
        }

        public PulseWidthModulatedFunction(
                double activetime,
                IFunction context = null
            ) {
            this.activetime = activetime;
            this.context = context;
        }

        public double GetValue(FunctionCall arg) {
            var t = arg.Time % 1.0;
            var factor = 1.0;

            if (t > activetime)
                factor = 0;

            if (context != null)
                return context.GetValue(arg) * factor;

            return factor;
        }
    }
}
