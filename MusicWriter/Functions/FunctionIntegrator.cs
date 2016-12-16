using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public static class FunctionIntegrator
    {
        public static IFunction Integrate(this IFunction derivative) =>
            (derivative as IDirectlyIntegratableFunction)?.Integrate() ??
            new StepwiseIntegratedFunction(Time.Note_128th, derivative);
    }
}
