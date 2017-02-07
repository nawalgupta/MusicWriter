using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public abstract class Container : BoundObject<IContainer>, IContainer
    {
        readonly string codename;

        public string Codename {
            get { return codename; }
        }
        
        public Container(
                StorageObjectID storageobjectID,
                EditorFile file,
                IFactory<IContainer> factory,
                string name,
                string codename
            ) :
            base(
                    storageobjectID,
                    file,
                    factory
                ) {
            Name.Value = name;

            Name.BeforeChange += Name_BeforeChange;

            this.codename = codename;
        }

        private void Name_BeforeChange(ObservableProperty<string>.PropertyChangingEventArgs args) {
            args.Canceled = true;
        }
    }
}
