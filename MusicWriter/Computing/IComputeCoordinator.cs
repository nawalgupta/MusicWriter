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

        ComputeJobID SetupJob(
                string container,
                StorageObjectID item
            );

        bool IsJobRunning(ComputeJobID jobID);

        void StartJob(ComputeJobID jobID);

        void PauseJob(ComputeJobID jobID);

        void StopJob(ComputeJobID jobID);

        Task WaitForJobFinishAsync(ComputeJobID jobID);
    }
}
