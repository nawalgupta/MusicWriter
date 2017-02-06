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
        readonly IOListener
            listener_activefunctionsourcefile_add,
            listener_activefunctionsourcefile_remove;

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

            var activefunctionsource_vec_obj =
                obj.GetOrMake("active-function-source-vec");

            listener_activefunctionsourcefile_add =
                activefunctionsource_vec_obj.Listen(IOEvent.ChildAdded, (key, activefunctionsource_objID) => {
                    if (ActiveFunctionSourceFile.Value != null)
                        throw new InvalidOperationException();

                    ActiveFunctionSourceFile.Value =
                        container.FunctionSources[activefunctionsource_objID];
                });

            listener_activefunctionsourcefile_remove =
                activefunctionsource_vec_obj.Listen(IOEvent.ChildRemoved, (key, activefunctionsource_objID) => {
                    if (ActiveFunctionSourceFile.Value == null)
                        throw new InvalidOperationException();

                    ActiveFunctionSourceFile.Value = null;
                });
        }

        public override void Unbind() {
            File.Storage.Listeners.Remove(listener_activefunctionsourcefile_add);
            File.Storage.Listeners.Remove(listener_activefunctionsourcefile_remove);

            base.Unbind();
        }
    }
}
