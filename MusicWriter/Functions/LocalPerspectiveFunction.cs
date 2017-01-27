using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class LocalPerspectiveFunction : IFunction, IContextualFunction
    {
        readonly IFunction context;

        public IFunction Context {
            get { return context; }
        }

        public IFunctionFactory Factory {
            get { return FactoryClass.Instance; }
        }

        public sealed class FactoryClass : IFunctionFactory
        {
            public string FriendlyName {
                get { return "Local Perspective Function"; }
            }

            public string CodeName {
                get { return "time.local"; }
            }

            public bool StoresBinaryData {
                get { return false; }
            }

            public bool AcceptsParameters {
                get { return false; }
            }

            private FactoryClass() { }

            public IFunction Create(
                    IFunction context = null,
                    IFunction[] args = null,
                    IStorageObject data = null,
                    params float[] numbers
                ) {
                throw new InvalidOperationException();
            }

            public static readonly IFunctionFactory Instance = new FactoryClass();
        }

        public LocalPerspectiveFunction(IFunction context) {
            this.context = context;
        }

        public float GetValue(FunctionCall arg) =>
            context.GetValue(new FunctionCall(arg.LocalTime, arg.LocalTime, arg.RealTime));
    }
}
