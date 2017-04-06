using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IComputeSlave
    {
        string Container { get; }

        void Complete(
                ComputeJob job,
                StorageObjectID partition_objID
            );
    }
}
