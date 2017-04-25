﻿using System;
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
        readonly IOListener listener_contentsset;

        readonly string container;
        readonly ComputeJobID jobID;
        readonly IComputeCoordinator coordinator;
        readonly IComputeSlave slave;

        BackgroundWorker worker;

        public enum WorkingState
        {
            NotWorking,
            Working,
            WorkingUntilComplete,
        }

        public WorkingState State { get; private set; }

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
                            async (key, slave_objID) => {
                                if (State == WorkingState.Working) {
                                    worker =
                                        new BackgroundWorker(
                                                () =>
                                                    slave.Complete(
                                                            this,
                                                            slave_objID
                                                        )
                                            );

                                    worker.Start();

                                    slaves_obj.Rename(slave_objID, ComputeConstants.SlaveKey_Working);

                                    await worker.WaitForFinishAsync();

                                    if (State == WorkingState.Working)
                                        slaves_obj.Rename(slave_objID, ComputeConstants.SlaveKey_JobRequested);
                                    else {
                                        slaves_obj.Rename(slave_objID, ComputeConstants.SlaveKey_NotWillingToWork);
                                        State = WorkingState.NotWorking;
                                    }
                                }
                            }
                        );
        }

        public void Start() {
            slaves_obj.Add(ComputeConstants.SlaveKey_JobRequested, slave_obj.ID);

            State = WorkingState.Working;
        }

        public async void RequestStop() {
            State = WorkingState.WorkingUntilComplete;

            await worker.WaitForFinishAsync();

            slaves_obj.Remove(slave_obj.ID);
        }

        public void ForceStop() {
            State = WorkingState.NotWorking;

            worker.Stop();

            //TODO: does this mark this to the coordinator as cancelled?
            slaves_obj.Rename(slave_obj.ID, ComputeConstants.SlaveKey_Stopped);
            slaves_obj.Remove(slave_obj.ID);
        }

        public override void Bind() {
            File.Storage.Listeners.Add(listener_contentsset);
            
            base.Bind();
        }

        public override void Unbind() {
            File.Storage.Listeners.Remove(listener_contentsset);
            
            base.Unbind();
        }
    }
}