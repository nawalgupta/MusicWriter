using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class CompositeJobManager : IJobManager
    {
        public ObservableList<IJobManager> Jobs { get; } =
            new ObservableList<IJobManager>();
        
        public float? Progress {
            get { return Jobs.Average(job => job.Progress); }
        }

        public JobState State {
            get {
                if (Jobs.All(job => job.State == JobState.Done))
                    return JobState.Done;

                if (Jobs.All(job => job.State == JobState.NotStarted))
                    return JobState.NotStarted;

                if (Jobs.All(job => job.State == JobState.Paused))
                    return JobState.Paused;

                return JobState.Running;
            }
        }

        public void Pause() {
            foreach (var job in Jobs)
                job.Pause();
        }

        public void Reset() {
            foreach (var job in Jobs)
                job.Reset();
        }

        public void Resume() {
            foreach (var job in Jobs)
                job.Reset();
        }

        public void Start() {
            foreach (var job in Jobs)
                job.Start();
        }

        public void Stop() {
            foreach (var job in Jobs)
                job.Stop();
        }
    }
}
