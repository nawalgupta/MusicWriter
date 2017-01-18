using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class AssortedFilesManager
    {
        readonly IStorageObject storage;

        public IStorageObject Storage {
            get { return storage; }
        }
        
        public IStorageObject this[string file] {
            get { return storage.GetOrMake(file); }
        }

        public AssortedFilesManager(IStorageObject storage) {
            this.storage = storage;
        }

        public string GetName(StorageObjectID file) =>
            storage.GetRelation(file);
    }
}
