using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FunctionContainer : Container {
        public const string ItemName = "musicwriter.function.container";
        public const string ItemCodeName = "func";

        readonly IFactory<IContainer> factory;
        readonly FunctionCodeTools functioncodetools;
        readonly BoundList<FunctionSource> functionsources;
        
        public static IFactory<IContainer> MakeFactory(FunctionCodeTools functioncodetools) =>
            new FuncFactory<IContainer>(
                    ItemName,
                    (storageobjectID, file, factory) => { },
                    (storageobjectID, file, factory) =>
                        new FunctionContainer(
                                storageobjectID,
                                file,
                                factory,
                                functioncodetools
                            )
                );

        public override IFactory<IContainer> Factory {
            get { return factory; }
        }
        
        public FunctionCodeTools FunctionCodeTools {
            get { return functioncodetools; }
        }

        public BoundList<FunctionSource> FunctionSources {
            get { return functionsources; }
        }
        
        public FunctionContainer(
                StorageObjectID storageobjectID,
                EditorFile file,
                IFactory<IContainer> factory,
                FunctionCodeTools functioncodetools
            ) : 
            base(
                    storageobjectID, 
                    file,
                    ItemName,
                    ItemCodeName
                ) {
            this.factory = factory;
            this.functioncodetools = functioncodetools;

            functionsources = new BoundList<FunctionSource>(storageobjectID, file);
        }
    }
}
