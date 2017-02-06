using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class PolylineFunction :
        IFunction,
        IStoredDataFunction,
        IDirectlyIntegratableFunction
    {
        readonly PolylineData data;

        public PolylineData Data {
            get { return data; }
        }

        public StorageObjectID StorageObjectID {
            get { return data.StorageObjectID; }
        }

        public IFunctionFactory Factory {
            get { return FactoryClass.Instance; }
        }

        public PolylineFunction(PolylineData data) {
            this.data = data;
        }

        public sealed class FactoryClass : IFunctionFactory
        {
            public string FriendlyName {
                get { return "Polyline Function"; }
            }

            public string CodeName {
                get { return "polyline"; }
            }

            public bool AcceptsParameters {
                get { return false; }
            }

            public bool StoresBinaryData {
                get { return true; }
            }

            public IFunction Create(
                    IFunction context = null,
                    IFunction[] args = null,
                    EditorFile file = null,
                    string key = null,
                    params float[] numbers
                ) =>
                new PolylineFunction(
                        (file[PolylineContainer.ItemName] as PolylineContainer)
                            .Polylines
                            [key]
                    );

            private FactoryClass() { }

            public static readonly IFunctionFactory Instance = new FactoryClass();
        }

        public float GetValue(FunctionCall arg) =>
            data.GetValue(arg.Time);

        public string BinaryKey(EditorFile file) =>
            data.Name.Value;

        class SingleIntegratedPolylineFunction : IFunction
        {
            readonly PolylineFunction function;

            public PolylineFunction Function {
                get { return function; }
            }

            public IFunctionFactory Factory {
                get { throw new InvalidOperationException(); }
            }

            public SingleIntegratedPolylineFunction(PolylineFunction function) {
                this.function = function;
            }

            public float GetValue(FunctionCall arg) =>
                function.data.GetIntegratedValue(arg.Time);
        }

        SingleIntegratedPolylineFunction integrated = null;
        public IFunction Integrate() =>
            integrated ?? new SingleIntegratedPolylineFunction(this);
    }
}
