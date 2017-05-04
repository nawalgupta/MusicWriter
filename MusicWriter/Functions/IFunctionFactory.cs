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
        string FriendlyName { get; }

        string CodeName { get; }

        bool StoresBinaryData { get; }

        bool AcceptsParameters { get; }
        
        IFunction Create(
                IFunction context = null,
                IFunction[] args = null,
                EditorFile file = null,
                string key = null,
                params double[] numbers
            );
    }
}
