using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class GloballyPerspectiveFunction : IFunction
    {
        readonly IFunction inner;

        public IFunction Inner {
            get { return inner; }
        }

        public GloballyPerspectiveFunction(IFunction inner) {
            this.inner = inner;
        }

        public float GetValue(FunctionCall arg) =>
            inner.GetValue(new FunctionCall(arg.LocalTime));
    }
}
