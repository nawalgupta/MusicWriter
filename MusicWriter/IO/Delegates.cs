using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public delegate void StorageObjectChildChangedDelegate(
            StorageObjectID container,
            StorageObjectID child
        );

    public delegate void StorageObjectChangedDelegate(StorageObjectID affected);
}
