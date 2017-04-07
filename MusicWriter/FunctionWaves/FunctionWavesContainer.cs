using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FunctionWavesContainer : Container
    {
        readonly BoundList<FunctionWave> functionwaves;
        readonly IStorageObject obj;

        public const string ItemName = "musicwriter.function-waves.container";
        public const string ItemCodeName = "funcwave";

        public BoundList<FunctionWave> FunctionWaves {
            get { return functionwaves; }
        }

        public FunctionWavesContainer(
                StorageObjectID storageobjectID, 
                EditorFile file
            ) :
            base(
                    storageobjectID, 
                    file,
                    FactoryInstance,
                    ItemName,
                    ItemCodeName
                ) {
            obj = file.Storage[storageobjectID];

            functionwaves =
                new BoundList<FunctionWave>(
                        obj.GetOrMake("waves").ID,
                        file,
                        new FactorySet<FunctionWave>(FunctionWave.FactoryInstance),
                        new ViewerSet<FunctionWave>()
                    );
        }

        public override void Bind() {
            FunctionWaves.Bind();

            base.Bind();
        }

        public override void Unbind() {
            FunctionWaves.Unbind();

            base.Unbind();
        }

        public static IFactory<IContainer> FactoryInstance { get; } =
            new CtorFactory<IContainer, FunctionWavesContainer>(
                    ItemName,
                    false
                );
    }
}
