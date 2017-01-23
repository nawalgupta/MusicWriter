using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed partial class FunctionEditorScreen<View> : IScreen<View>
    {
        readonly EditorFile<View> file;
        readonly FunctionContainer container;
        readonly StorageObjectID storageobjectID;

        public CommandCenter CommandCenter { get; } =
            new CommandCenter();

        public IScreenFactory<View> Factory {
            get { return FactoryClass.Instance; }
        }

        public EditorFile<View> File {
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
                EditorFile<View> file,
                StorageObjectID storageobjectID
            ) {
            this.file = file;
            this.container = null;//TODO: implement abstract containers in the file (e.g., track-controller, function-editor, etc.)
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
                    new FunctionSource(
                            activefunctionsource_objID,
                            container
                        );
            };

            activefunctionsource_vec_obj.ChildRemoved += (activefunctionsource_vec_objID, activefunctionsource_objID, key) => {
                if (ActiveFunctionSourceFile.Value == null)
                    throw new InvalidOperationException();

                ActiveFunctionSourceFile.Value = null;
            };
        }
    }
}
