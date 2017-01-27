using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed partial class FunctionEditorScreen : IScreen
    {
        readonly EditorFile file;
        readonly FunctionContainer container;
        readonly StorageObjectID storageobjectID;

        public const string ItemName = "musicwriter.function.screen";

        public static IFactory<IScreen> FactoryInstance { get; } =
            new FuncFactory<IScreen>(
                    ItemName,
                    (storageobjectID, file) => { },
                    (storageobjectID, file) => 
                        new FunctionEditorScreen(
                                storageobjectID,
                                file
                            )
                );

        public CommandCenter CommandCenter { get; } =
            new CommandCenter();

        public IFactory<IScreen> Factory {
            get { return FactoryInstance; }
        }

        public EditorFile File {
            get { return file; }
        }

        public FunctionContainer Container {
            get { return container; }
        }

        public ObservableProperty<string> Name { get; } =
            new ObservableProperty<string>();
        
        public StorageObjectID StorageObjectID {
            get { return storageobjectID; }
        }

        public ObservableProperty<FunctionSource> ActiveFunctionSourceFile { get; } =
            new ObservableProperty<FunctionSource>();

        public FunctionEditorScreen(
                StorageObjectID storageobjectID,
                EditorFile file
            ) {
            this.file = file;
            container = file[FunctionContainer.ItemName] as FunctionContainer;
            this.storageobjectID = storageobjectID;

            Setup();
        }

        void Setup() {
            var obj =
                file.Storage[storageobjectID];
            
            var activefunctionsource_vec_obj =
                obj.GetOrMake("active-function-source-vec");
            
            activefunctionsource_vec_obj.ChildAdded += (activefunctionsource_vec_objID, activefunctionsource_objID, key) => {
                if (ActiveFunctionSourceFile.Value != null)
                    throw new InvalidOperationException();

                ActiveFunctionSourceFile.Value =
                    container.FunctionSources[activefunctionsource_objID];
            };

            activefunctionsource_vec_obj.ChildRemoved += (activefunctionsource_vec_objID, activefunctionsource_objID, key) => {
                if (ActiveFunctionSourceFile.Value == null)
                    throw new InvalidOperationException();

                ActiveFunctionSourceFile.Value = null;
            };
        }
    }
}
