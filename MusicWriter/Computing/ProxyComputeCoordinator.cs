﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class ProxyComputeCoordinator :
        BoundObject<IComputeCoordinator>,
        IComputeCoordinator
    {
        IStorageObject
            obj,
            unallocated_obj,
            allocated_obj;

        IOListener
            listener_allocatedjobs_added,
            listener_allocatedjobs_removed;

        readonly Dictionary<StorageObjectID, ComputeJobID> jobID_item_lookup =
            new Dictionary<StorageObjectID, ComputeJobID>();
        readonly Dictionary<StorageObjectID, BackgroundWorker> jobID_allocated_waithandle =
            new Dictionary<StorageObjectID, BackgroundWorker>();

        readonly Dictionary<string, IComputeSlave> slaves_map =
            new Dictionary<string, IComputeSlave>();

        public IObservableList<ComputeJob> Jobs { get; } =
            new ObservableList<ComputeJob>();

        public IObservableList<IComputeSlave> Slaves { get; } =
            new ObservableList<IComputeSlave>();

        public ProxyComputeCoordinator(
                StorageObjectID storageobjectID,
                EditorFile file
            ) :
            base(
                    storageobjectID,
                    file
                ) {
            obj = file.Storage[storageobjectID];

            unallocated_obj = obj.GetOrMake("unallocated");
            allocated_obj = obj.GetOrMake("allocated");

            listener_allocatedjobs_added =
                allocated_obj
                    .CreateListen(
                            IOEvent.ChildAdded,
                            job_objID => {
                                var job =
                                    new ComputeJob(
                                            job_objID,
                                            File,
                                            this
                                        );

                                Jobs.Add(job);

                                jobID_item_lookup.Add(job.WorkItemStorageObjectID, job.JobID);
                                jobID_allocated_waithandle[job.StorageObjectID].Stop();
                            }
                        );

            listener_allocatedjobs_removed =
                allocated_obj
                    .CreateListen(
                            IOEvent.ChildRemoved,
                            job_objID => {
                                var job =
                                    Jobs.FirstOrDefault(_ => _.StorageObjectID == job_objID);

                                if (job.Working)
                                    job.ForceStop();

                                Jobs.Remove(job);
                            }
                        );

            Slaves.ItemAdded += Slaves_ItemAdded;
            Slaves.ItemRemoved += Slaves_ItemRemoved;
        }

        public override void Bind() {
            File.Storage.Listeners.Add(listener_allocatedjobs_added);
            File.Storage.Listeners.Add(listener_allocatedjobs_removed);

            base.Bind();
        }

        public override void Unbind() {
            File.Storage.Listeners.Remove(listener_allocatedjobs_added);
            File.Storage.Listeners.Remove(listener_allocatedjobs_removed);

            base.Unbind();
        }

        private void Slaves_ItemRemoved(IComputeSlave slave) {
            slaves_map.Add(slave.Container, slave);
        }

        private void Slaves_ItemAdded(IComputeSlave slave) {
            slaves_map.Remove(slave.Container);
        }

        public ComputeJobID StartJob(
                string container,
                StorageObjectID item
            ) {
            var jobrequest_obj =
                File.Storage.CreateObject();

            jobrequest_obj.Add("item", item);
            jobrequest_obj.GetOrMake("container").WriteAllString(container);

            unallocated_obj.Add("", jobrequest_obj.ID);

            var waithandle =
                BackgroundWorker.MakeWaitHandle();
            
            jobID_allocated_waithandle.Add(item, waithandle);

            waithandle.WaitForFinish();

            var jobID = jobID_item_lookup[item];

            jobID_allocated_waithandle.Remove(item);
            jobID_item_lookup.Remove(item);

            unallocated_obj.Remove(jobrequest_obj.ID);

            return jobID;
        }

        public void StopJob(ComputeJobID jobID) {
            throw new NotImplementedException();
        }

        public IComputeSlave GetSlaveFor(string container) =>
            slaves_map[container];
    }
}