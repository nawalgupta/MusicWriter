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

        public ObservableProperty<SemiTone> Tone { get; } = new ObservableProperty<SemiTone>(SemiTone.C4);
        public ObservableProperty<PerceptualTime> Length { get; } = new ObservableProperty<PerceptualTime>(new PerceptualTime(TupletClass.None, LengthClass.Quarter, 0));
        public ObservableProperty<Mode> Mode { get; } = new ObservableProperty<Mode>(MusicWriter.Mode.Major);
        public ObservableProperty<DebugVerb> Verb { get; } = new ObservableProperty<DebugVerb>(DebugVerb.Note);

        public DebugSound(
                StorageObjectID storageobjectID, 
                EditorFile file
            ) : 
            base(
                    storageobjectID, 
                    file
                ) {
            var obj = file.Storage[storageobjectID];

            var tone_obj = obj.GetOrMake("tone");
            binder_semitone = Tone.Bind(tone_obj);

            var length_obj = obj.GetOrMake("length");
            binder_length = Length.Bind(length_obj);

            var mode_obj = obj.GetOrMake("mode");
            binder_mode = Mode.Bind(mode_obj);

            var verb_obj = obj.GetOrMake("verb");
            binder_verb = 
                Verb.Bind(
                        verb_obj, 
                        ObviousExtensions.EnumParse<DebugVerb>,
                        ObviousExtensions.ToString
                    );
        }

        public override void Bind() {
            binder_semitone.Bind();
            binder_length.Bind();
            binder_mode.Bind();
            binder_verb.Bind();

            base.Bind();
        }

        public override void Unbind() {
            binder_semitone.Unbind();
            binder_length.Unbind();
            binder_mode.Unbind();
            binder_verb.Unbind();

            base.Unbind();
        }

        public const string ItemName = "musicwriter.function.debug-sound";

        public static IFactory<DebugSound> FactoryInstance { get; } =
            new CtorFactory<DebugSound, DebugSound>(
                    ItemName,
                    false
                );
    }
}
