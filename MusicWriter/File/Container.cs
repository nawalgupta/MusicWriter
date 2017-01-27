using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public abstract class Container : BoundObject<IContainer>, IContainer
    {
        public Container(
                StorageObjectID storageobjectID,
                EditorFile file,
                string name
            ) :
            base(
                    storageobjectID,
                    file
                ) {
            Name.Value = name;

            Name.BeforeChange += Name_BeforeChange;
        }

        private void Name_BeforeChange(ObservableProperty<string>.PropertyChangingEventArgs args) {
            args.Canceled = true;
        }
    }
}
