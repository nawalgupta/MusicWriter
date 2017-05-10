using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class EasyJobManager<T>
        : IJobManager
        where T : IBoundObject<T>
    {
        readonly IComputeCoordinator coordinator;
        readonly ObservableProperty<T> property;
        readonly string container;
        ComputeJobID jobID;
        
        public IComputeCoordinator Coordinator {
            get { return coordinator; }
        }

        public ObservableProperty<T> Property {
            get { return property; }
        }

        public string Container {
            get { return container; }
        }

        public ComputeJobID JobID {
            get { return jobID; }
        }

        public JobState State { get; private set; } = JobState.NotStarted;

        public float? Progress { get; set; } = null;

        public EasyJobManager(
                IComputeCoordinator coordinator,
                ObservableProperty<T> property,
                string container
            ) {
            this.coordinator = coordinator;
            this.property = property;
            this.container = container;
        }

        public async void Start() {
            if (State != JobState.NotStarted)
                throw new InvalidOperationException();

            jobID =
                coordinator
                    .StartJob(
                            container,
                            property
                                .Value
                                .StorageObjectID
                        );

            State = JobState.Running;

            await coordinator.WaitForJobFinishAsync(jobID);

            State = JobState.Done;
            jobID = default(ComputeJobID);
        }

        public void Stop() {
            if (State == JobState.NotStarted ||
                State == JobState.Done)
                throw new InvalidOperationException();

            coordinator.StopJob(jobID);
        }

        public void Pause() {
            if (State != JobState.Running)
                throw new InvalidOperationException();

            State = JobState.Paused;

            throw new NotImplementedException();
        }

        public void Resume() {
            if (State != JobState.Paused)
                throw new InvalidOperationException();

            State = JobState.Running;

            throw new NotImplementedException();
        }

        public void Reset() {
            if (State != JobState.NotStarted &&
                State != JobState.Done)
                Stop();
        }
    }
}
