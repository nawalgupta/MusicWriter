using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public abstract class Screen : NamedBoundObject<IScreen>, IScreen
    {
        public CommandCenter CommandCenter { get; } =
            new CommandCenter();

        public Screen(
                StorageObjectID storageobjectID,
                EditorFile file,
                IFactory<IScreen> factory = null
            ) :
            base(
                    storageobjectID,
                    file,
                    factory
                ) {
        }
    }
}
