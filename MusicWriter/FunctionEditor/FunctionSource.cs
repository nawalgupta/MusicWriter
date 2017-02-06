using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed partial class FunctionSource : BoundObject<FunctionSource>
    {
        readonly StorageObjectID storageobjectID;
        readonly FunctionContainer container;

        public const string ItemName = "musicwriter.function.source";
        
        public ObservableProperty<string> Code { get; } =
            new ObservableProperty<string>();

        public ObservableProperty<IFunction> Function { get; } =
            new ObservableProperty<IFunction>();

        public FunctionContainer Container {
            get { return container; }
        }

        public override IFactory<FunctionSource> Factory {
            get { return FactoryInstance; }
        }

        public FunctionSource(
                StorageObjectID storageobjectID,
                EditorFile file
            ) :
            base(
                    storageobjectID,
                    file
                )
            {
            this.storageobjectID = storageobjectID;
            container = file[FunctionContainer.ItemName] as FunctionContainer;

            Setup();
        }

        void Setup() {
            var obj = File.Storage[storageobjectID];

            Code.AfterChange += Code_AfterChange;
            Function.AfterChange += Function_AfterChange;

            Code.Bind(obj.GetOrMake("code"));
        }

        private void Function_AfterChange(IFunction old, IFunction @new) {
            var code = new StringBuilder();
            container
                .FunctionCodeTools
                .Render(
                        code,
                        @new,
                        File
                    );

            if (code != null)
                Code.Value = code.ToString();
        }

        private void Code_AfterChange(string old, string @new) {
            var errors =
                new List<KeyValuePair<Tuple<int, int>, string>>();

            Function.Value =
                    container
                        .FunctionCodeTools
                        .Parse(
                                ref @new,
                                File,
                                errors
                            );
        }

        public static IFactory<FunctionSource> FactoryInstance { get; } =
            new CtorFactory<FunctionSource, FunctionSource>(ItemName);
    }
}
