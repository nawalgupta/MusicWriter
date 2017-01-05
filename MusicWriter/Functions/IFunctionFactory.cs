using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IFunctionFactory
    {
        string Name { get; }

        string CodeName { get; }

        bool StoresBinaryData { get; }

        bool AcceptsParameters { get; }
        
        IFunction Create(
                IFunction context = null,
                IFunction[] args = null,
                IStorageObject data = null,
                params float[] numbers
            );
    }
}
