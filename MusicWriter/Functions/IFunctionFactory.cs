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

        IFunction Create(IFunction[] args, params float[] numbers);

        IFunction Create(params float[] args);

        IFunction Create(IFunction context, params float[] args);

        IFunction Create(IFunction context, IFunction[] args, params float[] numbers);

        IFunction Deserialize(Stream stream);

        IFunction Deserialize(
                Stream stream,
                IFunction context
            );

        IFunction Deserialize(
                Stream stream,
                IFunction[] arguments
            );

        IFunction Deserialize(
                Stream stream,
                IFunction context,
                IFunction[] arguments
            );

        void Serialize(
                Stream stream,
                IFunction function
            );
    }
}
