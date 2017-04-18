using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IJobManager
    {
        IComputeCoordinator Coordinator { get; }

        ComputeJobID JobID { get; }
        JobState State { get; }
        float? Progress { get; }

        void Start();
        void Stop();

        void Resume();
        void Pause();
    }
}
