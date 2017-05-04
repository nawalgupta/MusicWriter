using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class TimeScalingFunction : 
        IFunction, 
        IContextualFunction,
        IParamaterizedFunction
    {
        readonly IFunction context;
        readonly double[] arguments;

        public IFunction Context {
            get { return context; }
        }

        public double[] Arguments {
            get { return arguments; }
        }

        public IFunctionFactory Factory {
            get { return FactoryClass.Instance; }
        }

        public sealed class FactoryClass : IFunctionFactory
        {
            public string FriendlyName {
                get { return "Time Scaling Function"; }
            }

            public string CodeName {
                get { return "time.scale"; }
            }

            public bool StoresBinaryData {
                get { return false; }
            }

            public bool AcceptsParameters {
                get { return true; }
            }

            private FactoryClass() { }

            public IFunction Create(
                    IFunction context = null,
                    IFunction[] args = null,
                    EditorFile file = null,
                    string key = null,
                    params double[] numbers
                ) =>
                new TimeScalingFunction(context, numbers);

            public static readonly IFunctionFactory Instance = new FactoryClass();
        }

        public TimeScalingFunction(
                IFunction context,
                double[] arguments
            ) {
            this.context = context;
            this.arguments = arguments;
        }

        public double GetValue(FunctionCall arg) =>
            context.GetValue(new FunctionCall(arg.WaveTime * arguments[0], arg.LocalTime, arg.RealTime));
    }
}
