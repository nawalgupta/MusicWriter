using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FactorFunction : 
        IFunction,
        IContextualFunction,
        IParamaterizedFunction
    {
        readonly IFunction context;
        readonly double factor;

        public IFunctionFactory Factory {
            get { return FactoryClass.Instance; }
        }

        public double[] Arguments {
            get { return new double[] { factor }; }
        }

        public IFunction Context {
            get { return context; }
        }

        public double Factor {
            get { return factor; }
        }

        public sealed class FactoryClass : IFunctionFactory
        {
            public bool AcceptsParameters {
                get { return true; }
            }

            public string CodeName {
                get { return "factor"; }
            }

            public string FriendlyName {
                get { return "Factor multiplying function"; }
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
                new FactorFunction(
                        context,
                        numbers.Length == 1 ?
                            numbers[0] :
                            0
                    );

            public static IFunctionFactory Instance { get; } =
                new FactoryClass();
        }

        public FactorFunction(
                IFunction context,
                double factor
            ) {
            this.context = context;
            this.factor = factor;
        }

        public double GetValue(FunctionCall arg) =>
            factor * context.GetValue(arg);
    }
}
