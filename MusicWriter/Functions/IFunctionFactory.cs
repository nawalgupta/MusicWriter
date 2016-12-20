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
        
        IFunction Create();

        IFunction Create(IFunction context);

        IFunction Create(IFunction[] args);

        IFunction Create(params float[] args);

        IFunction Create(IFunction context, params float[] args);

        IFunction Deserialize(Stream stream);

        void Serialize(
                Stream stream,
                IFunction function
            );
    }
}
