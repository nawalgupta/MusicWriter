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
        readonly DebugSound debugsound;

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

        public DebugSound DebugSound {
            get { return debugsound; }
        }

        public FunctionEditorScreen(
                StorageObjectID storageobjectID,
                EditorFile file,
                IFactory<IScreen> factory
            ) : base(
                    storageobjectID,
                    file,
                    factory
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

            debugsound = 
                new DebugSound(
                        obj.GetOrMake("debug-sound").ID,
                        file
                    );
        }

        public override void Bind() {
            functionsources.Bind();
            debugsound.Bind();

            base.Bind();
        }

        public override void Unbind() {
            functionsources.Unbind();
            debugsound.Unbind();

            base.Unbind();
        }
    }
}
