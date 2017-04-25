using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class DebugSound : BoundObject<DebugSound>
    {
        readonly PropertyBinder<SemiTone> binder_semitone;
        readonly PropertyBinder<PerceptualTime> binder_length;
        readonly PropertyBinder<Mode> binder_mode;
        readonly PropertyBinder<DebugVerb> binder_verb;
        readonly PropertyBinder<int> binder_count;
        readonly PropertyBinder<float> binder_notelength;
        readonly PropertyBinder<DebugStrategy> binder_renderingstrategy;
        readonly ObjectPropertyBinder<FunctionWave> binder_renderedwave;
        readonly ObjectPropertyBinder<FunctionSource> binder_functionsource;
        readonly IJobManager jobmanager;

        public ObservableProperty<SemiTone> Tone { get; } = new ObservableProperty<SemiTone>(SemiTone.C4);
        public ObservableProperty<PerceptualTime> Length { get; } = new ObservableProperty<PerceptualTime>(new PerceptualTime(TupletClass.None, LengthClass.Quarter, 0));
        public ObservableProperty<Mode> Mode { get; } = new ObservableProperty<Mode>(MusicWriter.Mode.Major);
        public ObservableProperty<DebugVerb> Verb { get; } = new ObservableProperty<DebugVerb>(DebugVerb.Note);
        public ObservableProperty<int> Count { get; } = new ObservableProperty<int>(1);
        public ObservableProperty<float> NoteLength { get; } = new ObservableProperty<float>(2);
        public ObservableProperty<DebugStrategy> RenderingStrategy { get; } = new ObservableProperty<DebugStrategy>();
        public ObservableProperty<FunctionWave> RenderedWave { get; } = new ObservableProperty<FunctionWave>();
        public ObservableProperty<FunctionSource> FunctionSource { get; } = new ObservableProperty<FunctionSource>();
        public IJobManager JobManager {
            get { return jobmanager; }
        }

        public DebugSound(
                StorageObjectID storageobjectID,
                EditorFile file
            ) :
            base(
                    storageobjectID,
                    file
                ) {
            var obj = file.Storage[storageobjectID];

            binder_semitone = Tone.Bind(obj.GetOrMake("tone"));
            binder_length = Length.Bind(obj.GetOrMake("length"));
            binder_mode = Mode.BindEnum(obj.GetOrMake("mode"));
            binder_verb = Verb.BindEnum(obj.GetOrMake("verb"));
            binder_count = Count.Bind(obj.GetOrMake("count"));
            binder_notelength = NoteLength.Bind(obj.GetOrMake("tempo"));
            binder_renderingstrategy = RenderingStrategy.BindEnum(obj.GetOrMake("rendering-strategy"));
            binder_renderedwave = RenderedWave.BindObject(storageobjectID, "wave", (file[FunctionWaveContainer.ItemName] as FunctionWaveContainer).FunctionWaves);
            binder_functionsource = FunctionSource.BindObject(storageobjectID, "func", (file[FunctionContainer.ItemName] as FunctionContainer).FunctionSources);

            Length.AfterChange += Length_AfterChange;
            Count.AfterChange += Count_AfterChange;
            NoteLength.AfterChange += NoteLength_AfterChange;

            //TODO: insert facade function source so that verb can be applied to the user's function source.

            jobmanager =
                new EasyJobManager<FunctionWave>(
                        file
                            [ComputeContainer.ItemName]
                            .As<IContainer, ComputeContainer>()
                            .Coordinator,
                        RenderedWave,
                        FunctionWaveContainer.ItemName
                    );
        }

        private void Length_AfterChange(PerceptualTime old, PerceptualTime @new) {
            Recalc_Length();
        }

        private void Count_AfterChange(int old, int @new) {
            Recalc_Length();
        }

        private void NoteLength_AfterChange(float old, float @new) {
            Recalc_Length();
        }

        void Recalc_Length() {
            RenderedWave
                .Value
                .Length
                .Value =
                Count.Value * 
                NoteLength.Value *
                Length.Value.TimeLength().Notes;
        }

        public override void Bind() {
            FunctionSource.AfterChange += FunctionSource_AfterChange;
            RenderedWave.AfterChange += RenderedWave_AfterChange;
            binder_semitone.Bind();
            binder_length.Bind();
            binder_mode.Bind();
            binder_verb.Bind();
            binder_count.Bind();
            binder_notelength.Bind();
            binder_renderingstrategy.Bind();
            binder_renderedwave.Bind();
            binder_functionsource.Bind();

            Recalc_Length();

            base.Bind();
        }

        public override void Unbind() {
            FunctionSource.AfterChange -= FunctionSource_AfterChange;
            RenderedWave.AfterChange -= RenderedWave_AfterChange;
            binder_semitone.Unbind();
            binder_length.Unbind();
            binder_mode.Unbind();
            binder_verb.Unbind();
            binder_count.Unbind();
            binder_notelength.Unbind();
            binder_renderingstrategy.Unbind();
            binder_renderedwave.Unbind();
            binder_functionsource.Unbind();

            base.Unbind();
        }

        private void RenderedWave_AfterChange(FunctionWave old, FunctionWave @new) {
            RenderedWave.Value.FunctionSource.Value = FunctionSource.Value;
        }

        private void FunctionSource_AfterChange(FunctionSource old, FunctionSource @new) {
            RenderedWave.Value.FunctionSource.Value = FunctionSource.Value;
        }

        public void StopRender() {
            jobmanager.Stop();
        }

        public void StartRender() {
            jobmanager.Start();
        }

        public const string ItemName = "musicwriter.function.debug-sound";

        public static IFactory<DebugSound> FactoryInstance { get; } =
            new CtorFactory<DebugSound, DebugSound>(
                    ItemName,
                    false
                );
    }
}
