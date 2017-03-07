using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed partial class FunctionEditorScreen : Screen
    {
        readonly FunctionContainer container;
        readonly BoundList<FunctionSource> functionsources;

        public const string ItemName = "musicwriter.function.screen";

        public static IFactory<IScreen> FactoryInstance { get; } =
            new CtorFactory<IScreen, FunctionEditorScreen>(ItemName);

        public override IFactory<IScreen> Factory {
            get { return FactoryInstance; }
        }

        public FunctionContainer Container {
            get { return container; }
        }

        public ObservableProperty<FunctionSource> ActiveFunctionSourceFile { get; } =
            new ObservableProperty<FunctionSource>();

        public BoundList<FunctionSource> FunctionSources {
            get { return functionsources; }
        }

        public FunctionEditorScreen(
                StorageObjectID storageobjectID,
                EditorFile file
            ) : base(
                    storageobjectID,
                    file
                ) {
            container = file[FunctionContainer.ItemName] as FunctionContainer;

            var obj =
                file.Storage[storageobjectID];

            functionsources =
                new BoundList<FunctionSource>(
                        obj.GetOrMake("sources").ID,
                        file,
                        container.FunctionSources
                    );
        }

        public override void Bind() {
            functionsources.Bind();

            base.Bind();
        }

        public override void Unbind() {
            functionsources.Unbind();

            base.Unbind();
        }
    }
}
