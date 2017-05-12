using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class ComputeJob : BoundObject<ComputeJob>
    {
        readonly IStorageObject job_obj;
        readonly StorageObjectID item_objID;
        readonly IStorageObject slaves_obj;
        readonly IStorageObject slave_obj;
        readonly IOListener
            listener_contentsset,
            listener_done_added,
            listener_running_added,
            listener_running_removed;

        readonly string container;
        readonly ComputeJobID jobID;
        readonly IComputeCoordinator coordinator;
        readonly IComputeSlave slave;

        BackgroundWorker worker;

        [Flags]
        public enum WorkingState
        {
            NotWorking = 0,
            Working = 1,
            WorkingUntilComplete = 3,
            Paused = 4,
        }

        public WorkingState State { get; private set; }

        public event Action Finished;

        public bool IsStarted { get; private set; } = false;

        public bool Working {
            get { return State != WorkingState.NotWorking; }
        }

        public string Container {
            get { return container; }
        }

        public StorageObjectID WorkItemStorageObjectID {
            get { return item_objID; }
        }

        public ComputeJobID JobID {
            get { return jobID; }
        }

        public IComputeCoordinator Coordinator {
            get { return coordinator; }
        }

        public ComputeJob(
                StorageObjectID storageobjectID,
                EditorFile file,
                IComputeCoordinator coordinator
            ) :
            base(
                    storageobjectID,
                    file
                ) {
            job_obj = file.Storage[storageobjectID];
            item_objID = job_obj["item"];
            slaves_obj = job_obj.Get("slaves");
            slave_obj = file.Storage.CreateObject();

            container = job_obj.Get("container").ReadAllString();

            using (var reader = new BinaryReader(job_obj.Get("id").OpenRead())) {
                jobID = new ComputeJobID(reader.ReadUInt64());
            }

            this.coordinator = coordinator;
            slave = coordinator.GetSlaveFor(container);

            listener_contentsset =
                slave_obj
                    .CreateListen(
                            IOEvent.ObjectContentsSet,
                            async () => {
                                //TODO: fix
                                //if (State == WorkingState.Working) {
                                    worker =
                                        new BackgroundWorker(
                                                () =>
                                                    slave.Complete(
                                                            this,
                                                            slave_obj.ID
                                                        )
                                            );

                                    worker.Start();

                                    slaves_obj.Rename(slave_obj.ID, ComputeConstants.SlaveKey_Working);

                                    await worker.WaitForFinishAsync();

                                    if (State == WorkingState.Working)
                                        slaves_obj.Rename(slave_obj.ID, ComputeConstants.SlaveKey_JobRequested);
                                    else {
                                        slaves_obj.Rename(slave_obj.ID, ComputeConstants.SlaveKey_NotWillingToWork);
                                        State = WorkingState.NotWorking;
                                    }
                                //}
                            }
                        );

            listener_done_added =
                job_obj
                    .Graph
                    .CreateListen(
                        msg => {
                            Finished?.Invoke();
                            State = WorkingState.NotWorking;
                        },
                        subject: job_obj.ID,
                        verb: IOEvent.ChildAdded,
                        key: ComputeConstants.SlaveKey_WorkIsDone
                    );

            listener_running_added =
                job_obj
                    .Graph
                    .CreateListen(
                        msg => {
                            if (IsStarted)
                                Resume();
                            else
                                Start();
                        },
                        subject: job_obj.ID,
                        verb: IOEvent.ChildAdded,
                        key: ComputeConstants.SlaveKey_Working
                    );

            listener_running_removed =
                job_obj
                    .Graph
                    .CreateListen(
                        msg => {
                            if (State.HasFlag(WorkingState.Working))
                                Pause();
                        },
                        subject: job_obj.ID,
                        verb: IOEvent.ChildRemoved,
                        key: ComputeConstants.SlaveKey_Working
                    );
        }
        
        public void Start() {
            if (IsStarted)
                throw new InvalidOperationException();

            IsStarted = true;
            State = WorkingState.Working;

            slaves_obj.Add(ComputeConstants.SlaveKey_JobRequested, slave_obj.ID);
        }

        void Pause() {
            if (!IsStarted)
                throw new InvalidOperationException();

            if (!State.HasFlag(WorkingState.Working))
                throw new InvalidOperationException();

            if (State.HasFlag(WorkingState.Paused))
                throw new InvalidOperationException();

            State |= WorkingState.Paused;

            worker.Pause();
        }

        void Resume() {
            if (!IsStarted)
                throw new InvalidOperationException();

            if (!State.HasFlag(WorkingState.Working))
                throw new InvalidOperationException();

            if (!State.HasFlag(WorkingState.Paused))
                throw new InvalidOperationException();

            State &= ~WorkingState.Paused;

            worker.Resume();
        }

        public async void RequestStop() {
            if (!IsStarted)
                throw new InvalidOperationException();

            State = WorkingState.WorkingUntilComplete;

            await worker.WaitForFinishAsync();

            slaves_obj.Remove(slave_obj.ID);
        }

        public void Shutdown() {
            if (!IsStarted)
                throw new InvalidOperationException();

            State = WorkingState.NotWorking;

            worker.Stop();
        }

        public void ForceStop() {
            Shutdown();

            //TODO: does this mark this to the coordinator as cancelled?
            slaves_obj.Rename(slave_obj.ID, ComputeConstants.SlaveKey_Stopped);
            slaves_obj.Remove(slave_obj.ID);
        }

        public override void Bind() {
            File.Storage.Listeners.Add(listener_contentsset);
            File.Storage.Listeners.Add(listener_done_added);
            File.Storage.Listeners.Add(listener_running_added);
            File.Storage.Listeners.Add(listener_running_removed);

            base.Bind();
        }

        public override void Unbind() {
            File.Storage.Listeners.Remove(listener_contentsset);
            File.Storage.Listeners.Remove(listener_done_added);
            File.Storage.Listeners.Remove(listener_running_added);
            File.Storage.Listeners.Remove(listener_running_removed);

            base.Unbind();
        }
    }
}
