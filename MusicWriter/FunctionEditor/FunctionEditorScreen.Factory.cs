using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    partial class FunctionEditorScreen<View>
    {
        public sealed class FactoryClass : IScreenFactory<View>
        {
            public string Name {
                get { return "Function Editor"; }
            }

            private FactoryClass() {
            }

            public void Init(
                    StorageObjectID storageobjectID, 
                    EditorFile<View> file
                ) {
                throw new NotImplementedException();
            }

            public IScreen<View> Load(
                    StorageObjectID storageobjectID, 
                    EditorFile<View> file
                ) {
                throw new NotImplementedException();
            }

            public static readonly IScreenFactory<View> Instance = new FactoryClass();
        }
    }
}
