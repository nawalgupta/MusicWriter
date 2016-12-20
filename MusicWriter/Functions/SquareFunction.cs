using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class SquareFunction : IFunction
    {
        public IFunctionFactory Factory {
            get { return FactoryClass.Instance; }
        }

        public sealed class FactoryClass : IFunctionFactory
        {
            public string Name {
                get { return "Square Wave"; }
            }

            public string CodeName {
                get { return "square"; }
            }

            public bool AcceptsParameters {
                get { return false; }
            }

            public bool StoresBinaryData {
                get { return false; }
            }

            public IFunction Create() =>
                new SquareFunction();

            public IFunction Create(params float[] args) {
                throw new InvalidOperationException();
            }

            public IFunction Create(IFunction[] args) {
                throw new InvalidOperationException();
            }

            public IFunction Create(IFunction context) {
                throw new InvalidOperationException();
            }

            public IFunction Create(IFunction[] args, params float[] numbers) {
                throw new InvalidOperationException();
            }

            public IFunction Create(IFunction context, IFunction[] args, params float[] numbers) {
                throw new InvalidOperationException();
            }

            public IFunction Deserialize(Stream stream) {
                throw new InvalidOperationException();
            }

            public void Serialize(Stream stream, IFunction function) {
                throw new InvalidOperationException();
            }

            public IFunction Create(IFunction context, params float[] args) {
                throw new InvalidOperationException();
            }

            public IFunction Deserialize(Stream stream, IFunction context) {
                throw new InvalidOperationException();
            }

            public IFunction Deserialize(Stream stream, IFunction[] arguments) {
                throw new InvalidOperationException();
            }

            public IFunction Deserialize(Stream stream, IFunction context, IFunction[] arguments) {
                throw new InvalidOperationException();
            }

            private FactoryClass() { }

            public static readonly IFunctionFactory Instance = new FactoryClass();
        }

        public float GetValue(FunctionCall arg) =>
            (arg.LocalTime < 0 || arg.LocalTime > 1) ? 0 : 1;
    }
}
