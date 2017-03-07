using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class Selector<T> :
        BoundObject<Selector<T>>
        where T : IBoundObject<T>
    {
        readonly BoundList<T> list;
        readonly bool ispersistent;
        readonly bool allownull;
        readonly PropertyBinder<int> index_binder;
        readonly IndexCorrectionBehavoir indexcorrectionbehavoir;

        public ObservableProperty<T> Active { get; } =
            new ObservableProperty<T>();

        public ObservableProperty<int> ActiveIndex { get; } =
            new ObservableProperty<int>(-1);

        public BoundList<T> List {
            get { return list; }
        }

        public bool IsPersistent {
            get { return ispersistent; }
        }

        public bool AllowNull {
            get { return allownull; }
        }

        public IndexCorrectionBehavoir IndexCorrectionBehavoir {
            get { return indexcorrectionbehavoir; }
        }

        public Selector(
                StorageObjectID storageobjectID,
                EditorFile file,
                BoundList<T> list,
                bool ispersistent = true,
                bool allownull = true,
                IndexCorrectionBehavoir indexcorrectionbehavoir = IndexCorrectionBehavoir.CancelChange
            ) :
            base(
                    storageobjectID,
                    file
                ) {
            this.list = list;
            this.ispersistent = ispersistent;
            this.allownull = allownull;
            this.indexcorrectionbehavoir = indexcorrectionbehavoir;

            ActiveIndex.BeforeChange += ActiveIndex_BeforeChange;
            ActiveIndex.AfterChange += ActiveIndex_AfterChange;
            Active.BeforeChange += Active_BeforeChange;
            Active.AfterChange += Active_AfterChange;

            if (ispersistent) {
                var obj = file.Storage[storageobjectID];

                index_binder = ActiveIndex.Bind(obj);
            }
        }

        public override void Bind() {
            index_binder.Bind();

            base.Bind();
        }

        public override void Unbind() {
            index_binder.Unbind();

            base.Unbind();
        }

        private void ActiveIndex_BeforeChange(ObservableProperty<int>.PropertyChangingEventArgs args) {
            var min =
                allownull ? -1 : 0;

            if (args.NewValue >= list.Count || args.NewValue < min) {
                switch (indexcorrectionbehavoir) {
                    case IndexCorrectionBehavoir.CancelChange:
                        args.Canceled = true;
                        break;

                    case IndexCorrectionBehavoir.ThrowException:
                        if (args.NewValue < -1)
                            throw new IndexOutOfRangeException($"New index {args.NewValue} is less than {min}.");
                        else throw new IndexOutOfRangeException($"New index {args.NewValue} past last index of list ({list.Count - 1}).");

                    case IndexCorrectionBehavoir.AlterChange:
                        if (args.NewValue < min)
                            args.NewValue = min;
                        else args.NewValue = list.Count - 1;
                        args.Altered = true;

                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private void ActiveIndex_AfterChange(int old, int @new) {
            if (@new == -1)
                Active.Value = default(T);
            else {
                Active.Value = list[@new];
            }
        }

        private void Active_BeforeChange(ObservableProperty<T>.PropertyChangingEventArgs args) {
            if (!list.Contains(args.NewValue))
                throw new InvalidOperationException("The new value being selected isn't in the list.");
        }

        private void Active_AfterChange(T old, T @new) {
            ActiveIndex.Value = list.IndexOf(@new);
        }
    }
}
