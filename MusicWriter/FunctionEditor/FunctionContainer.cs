using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FunctionContainer : Container
    {
        public const string ItemName = "musicwriter.function.container";
        public const string ItemCodeName = "func";

        readonly FunctionCodeTools functioncodetools;
        readonly BoundList<FunctionSource> functionsources;

        public static IFactory<IContainer> CreateFactory(
                FunctionCodeTools functioncodetools,
                FactorySet<FunctionSource> functionsources_factoryset,
                ViewerSet<FunctionSource> functionsources_viewerset
            ) =>
            new CtorFactory<IContainer, FunctionContainer>(
                    ItemName,
                    functioncodetools,
                    functionsources_factoryset,
                    functionsources_viewerset
                );

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

                FunctionCodeTools functioncodetools,

                FactorySet<FunctionSource> functionsources_factoryset,
                ViewerSet<FunctionSource> functionsources_viewerset
            ) :
            base(
                    storageobjectID,
                    file,
                    factory,
                    ItemName,
                    ItemCodeName
                ) {
            this.functioncodetools = functioncodetools;

            var obj =
                file.Storage[storageobjectID];

            functionsources =
                new BoundList<FunctionSource>(
                        obj.GetOrMake("function-sources").ID,
                        file,
                        functionsources_factoryset,
                        functionsources_viewerset
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
