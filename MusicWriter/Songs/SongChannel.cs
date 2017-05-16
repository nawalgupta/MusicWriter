using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class SongChannel : BoundObject<SongChannel>
    {
        public const string ItemName = "musicwriter.song.channel";

        readonly ObjectPropertyBinder<FunctionSource> functionsource;
        readonly ObjectPropertyBinder<FunctionWave> functionwave;
        readonly IJobManager jobmanager;

        public ObservableProperty<FunctionSource> FunctionSource { get; } =
            new ObservableProperty<FunctionSource>();

        public ObservableProperty<FunctionWave> FunctionWave { get; } =
            new ObservableProperty<FunctionWave>();

        public IJobManager JobManager {
            get { return jobmanager; }
        }

        public SongChannel(
                StorageObjectID storageobjectID,
                EditorFile file
            ) : 
            base(
                    storageobjectID,
                    file,
                    FactoryInstance
                ) {
            functionsource =
                FunctionSource
                    .BindObject(
                            storageobjectID,
                            "function-source",
                            file
                                [FunctionContainer.ItemName]
                                .As<IContainer, FunctionContainer>()
                                .FunctionSources
                        );

            functionwave =
                FunctionWave
                    .BindObject(
                            storageobjectID,
                            "function-wave",
                            file
                                [FunctionWaveContainer.ItemName]
                                .As<IContainer, FunctionWaveContainer>()
                                .FunctionWaves
                        );

            jobmanager =
                new EasyJobManager<FunctionWave>(
                        file
                            [ComputeContainer.ItemName]
                            .As<IContainer, ComputeContainer>()
                            .Coordinator,
                        FunctionWave,
                        FunctionWaveContainer.ItemName
                    );
        }

        public override void Bind() {
            functionsource.Bind();
            functionwave.Bind();

            FunctionSource.Set += FunctionSource_Set;
            FunctionWave.AfterChange += FunctionWave_AfterChange;
            FunctionWave.Set += FunctionWave_Set;

            base.Bind();
        }

        private void FunctionSource_Set(FunctionSource value) {
            FunctionWave.Value.FunctionSource.Value = value;
            FunctionSource.Value = value;
        }

        private void FunctionWave_Set(FunctionWave value) {
            value.FunctionSource.Set += FunctionSource_Set;
        }

        private void FunctionWave_AfterChange(FunctionWave old, FunctionWave @new) {
            old.FunctionSource.Set -= FunctionSource_Set;
        }

        public override void Unbind() {
            functionsource.Unbind();
            functionwave.Unbind();

            FunctionSource.Set -= FunctionSource_Set;
            FunctionWave.AfterChange -= FunctionWave_AfterChange;
            FunctionWave.Set -= FunctionWave_Set;
            if (FunctionWave.Value != null)
                FunctionWave.Value.FunctionSource.Set -= FunctionSource_Set;

            base.Unbind();
        }

        public static IFactory<SongChannel> FactoryInstance { get; } =
            new CtorFactory<SongChannel, SongChannel>(
                    ItemName,
                    false
                );
    }
}
