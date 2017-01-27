using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public abstract class Container : IContainer
    {
        readonly StorageObjectID storageobjectID;
        readonly EditorFile file;

        public StorageObjectID StorageObjectID {
            get { return storageobjectID; }
        }

        public EditorFile File {
            get { return file; }
        }

        public ObservableProperty<string> Name { get; } =
            new ObservableProperty<string>();

        public abstract IFactory<IContainer> Factory { get; }

        public Container(
                StorageObjectID storageobjectID,
                EditorFile file,
                string name
            ) {
            this.storageobjectID = storageobjectID;
            this.file = file;
            Name.Value = name;

            Name.BeforeChange += Name_BeforeChange;
        }

        private void Name_BeforeChange(ObservableProperty<string>.PropertyChangingEventArgs args) {
            args.Canceled = true;
        }
    }
}
