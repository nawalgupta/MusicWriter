using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class GainFunction : 
        IFunction,
        IContextualFunction,
        IParamaterizedFunction
    {
        readonly IFunction context;
        readonly double factor;
        readonly double gain;

        public IFunctionFactory Factory {
            get { return FactoryClass.Instance; }
        }

        public double[] Arguments {
            get { return new double[] { gain     }; }
        }

        public IFunction Context {
            get { return context; }
        }

        public double Factor {
            get { return factor; }
        }

        public double Gain {
            get { return gain; }
        }

        public sealed class FactoryClass : IFunctionFactory
        {
            public bool AcceptsParameters {
                get { return true; }
            }

            public string CodeName {
                get { return "gain"; }
            }

            public string FriendlyName {
                get { return "Decibel gain"; }
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
                new GainFunction(
                        context,
                        numbers.Length == 1 ?
                            numbers[0] :
                            0
                    );

            public static IFunctionFactory Instance { get; } =
                new FactoryClass();
        }

        public GainFunction(
                IFunction context,
                double gain
            ) {
            this.context = context;
            this.gain = gain;

            factor = Math.Pow(10, gain / 10.0);
        }

        public double GetValue(FunctionCall arg) =>
            factor * context.GetValue(arg);
    }
}
