using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IComputeCoordinator : IBoundObject<IComputeCoordinator>
    {
        IObservableList<ComputeJob> Jobs { get; }

        IComputeSlave GetSlaveFor(string container);

        ComputeJobID StartJob(
                string container,
                StorageObjectID item
            );

        void StopJob(ComputeJobID jobID);

        Task WaitForJobFinishAsync(ComputeJobID jobID);
    }
}
