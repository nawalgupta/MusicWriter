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
        readonly float[] arguments;

        public IFunction Context {
            get { return context; }
        }

        public float[] Arguments {
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
                    params float[] numbers
                ) =>
                new TimeScalingFunction(context, numbers);

            public static readonly IFunctionFactory Instance = new FactoryClass();
        }

        public TimeScalingFunction(
                IFunction context,
                float[] arguments
            ) {
            this.context = context;
            this.arguments = arguments;
        }

        public float GetValue(FunctionCall arg) =>
            context.GetValue(new FunctionCall(arg.WaveTime * arguments[0], arg.LocalTime, arg.RealTime));
    }
}
