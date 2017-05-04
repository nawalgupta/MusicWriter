using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IFunction
    {
        IFunctionFactory Factory { get; }

        double GetValue(FunctionCall arg);
    }
}
