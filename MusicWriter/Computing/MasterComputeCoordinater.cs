using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class MasterComputeCoordinator : 
        BoundObject<IComputeCoordinator>,
        IComputeCoordinator
    {
        public const string ItemName = "musicwriter.compute.container";

        readonly IOListener listener_jobqueued;

        readonly IStorageObject obj;
        readonly IStorageObject unallocatedjobs_obj;
        readonly IStorageObject allocatedjobs_obj;

        readonly Dictionary<ComputeJobID, IOListener>
            jobs_listener_slaves_add = new Dictionary<ComputeJobID, IOListener>(),
            jobs_listener_slaves_remove = new Dictionary<ComputeJobID, IOListener>(),
            jobs_listener_slaves_keyed = new Dictionary<ComputeJobID, IOListener>();

        readonly Dictionary<string, IComputePartitioner> partitioners_map =
            new Dictionary<string, IComputePartitioner>();

        readonly Dictionary<string, IComputeSlave> slaves_map =
            new Dictionary<string, IComputeSlave>();

        public IObservableList<IComputePartitioner> Partitioners { get; } =
            new ObservableList<IComputePartitioner>();

        public IObservableList<IComputeSlave> Slaves { get; } =
            new ObservableList<IComputeSlave>();

        public IObservableList<ComputeJob> Jobs { get; } =
            new ObservableList<ComputeJob>();

        public MasterComputeCoordinator(
                StorageObjectID storageobjectID,
                EditorFile file
            ) :
            base(
                    storageobjectID,
                    file,
                    FactoryInstance
                ) {
            obj = file.Storage[storageobjectID];
            unallocatedjobs_obj = obj.GetOrMake("unallocated");
            allocatedjobs_obj = obj.GetOrMake("allocated");

            listener_jobqueued =
                unallocatedjobs_obj
                    .CreateListen(
                            IOEvent.ChildAdded,
                            job_objID => {
                                var job_obj =
                                    file.Storage[job_objID];

                                if (!job_obj.AcquireExclusiveOwnership())
                                    return;

                                var job_item_obj =
                                    job_obj.Get("item");

                                var job_container_obj =
                                    job_obj.Get("container");

                                var container =
                                    job_container_obj.ReadAllString();

                                var jobID =
                                    StartJob(
                                            container,
                                            job_item_obj.ID
                                        );

                                job_obj.Add("job", GetJobObjID(jobID));
                            }
                        );

            Partitioners.ItemAdded += Partitioners_ItemAdded;
            Partitioners.ItemRemoved += Partitioners_ItemRemoved;
            Slaves.ItemAdded += Slaves_ItemAdded;
            Slaves.ItemRemoved += Slaves_ItemRemoved;
        }

        public override void Bind() {
            File.Storage.Listeners.Add(listener_jobqueued);

            base.Bind();
        }

        public override void Unbind() {
            File.Storage.Listeners.Remove(listener_jobqueued);

            foreach (var listener in jobs_listener_slaves_add.Values)
                File.Storage.Listeners.Remove(listener);
            foreach (var listener in jobs_listener_slaves_keyed.Values)
                File.Storage.Listeners.Remove(listener);
            foreach (var listener in jobs_listener_slaves_remove.Values)
                File.Storage.Listeners.Remove(listener);

            base.Unbind();
        }
        
        private void Partitioners_ItemAdded(IComputePartitioner partitioner) {
            partitioners_map.Add(partitioner.Container, partitioner);
        }

        private void Partitioners_ItemRemoved(IComputePartitioner partitioner) {
            partitioners_map.Remove(partitioner.Container);
        }

        private void Slaves_ItemRemoved(IComputeSlave slave) {
            slaves_map.Add(slave.Container, slave);
        }

        private void Slaves_ItemAdded(IComputeSlave slave) {
            slaves_map.Remove(slave.Container);
        }

        public IComputeSlave GetSlaveFor(string container) {
            return slaves_map[container];
        }

        StorageObjectID GetJobObjID(ComputeJobID jobID) {
            return allocatedjobs_obj[jobID.ToString()];
        }

        public ComputeJobID StartJob(
                string container,
                StorageObjectID item
            ) {
            ComputeJobID jobID;

            do jobID = ComputeJobID.NewComputeJobID();
            while (allocatedjobs_obj.HasChild(jobID.ToString()));

            var job_obj =
                File.Storage.CreateObject();

            using (var writer = new BinaryWriter(job_obj.GetOrMake("id").OpenWrite())) {
                writer.Write(jobID.ID);
            }

            job_obj.GetOrMake("container").WriteAllString(container);

            var info_obj =
                job_obj.GetOrMake("info");

            job_obj.Add("item", item);

            var slaves_obj =
                job_obj.GetOrMake("slaves");

            var partitioner =
                partitioners_map[container];

            partitioner
                .SetupPartitioner(
                        File,
                        item,
                        info_obj.ID
                    );

            var listener_slaves_add =
                slaves_obj
                    .CreateListen(
                            IOEvent.ChildAdded,
                            slave_objID =>
                                File
                                    .Storage
                                    [slaves_obj.ID]
                                    .Rename(
                                            slave_objID, 
                                            ComputeConstants.SlaveKey_JobRequested
                                        )
                        );

            var listener_slaves_remove =
                slaves_obj
                    .CreateListen(
                            IOEvent.ChildRemoved,
                            (key, slave_objID) => {
                                if (key == ComputeConstants.SlaveKey_Working) {
                                    partitioner
                                        .FailChunk(
                                                File,
                                                item,
                                                info_obj.ID,
                                                slave_objID
                                            );
                                }
                            }
                        );

            var listener_slaves_keyed =
                File
                    .Storage
                    .CreateListen(
                            msg => {
                                var slave_objID = msg.Object;

                                switch (msg.NewRelation) {
                                    case ComputeConstants.SlaveKey_JobRequested:
                                        var result =
                                            partitioner
                                                .PartitionChunk(
                                                        File,
                                                        item,
                                                        info_obj.ID,
                                                        slave_objID
                                                    );

                                        if (!result) {
                                            // there are no more partitions to do
                                            slaves_obj.Rename(slave_objID, ComputeConstants.SlaveKey_WorkIsDone);
                                        }

                                        break;

                                    case ComputeConstants.SlaveKey_Stopped:
                                        partitioner
                                            .FailChunk(
                                                    File,
                                                    item,
                                                    info_obj.ID,
                                                    slave_objID
                                                );

                                        break;

                                    case ComputeConstants.SlaveKey_NotWillingToWork:
                                        break;

                                    case ComputeConstants.SlaveKey_Working:
                                        break;

                                    case ComputeConstants.SlaveKey_WorkIsDone:
                                        slaves_obj.Remove(slave_objID);

                                        if (!slaves_obj.Children.Any())
                                            StopJob(jobID);

                                        //TODO: this leaves open the vulnerability that an 
                                        // authenticated compute slave could be the first one
                                        // to join and then report WorkIsDone, which would
                                        // delete the job.

                                        break;

                                    default:
                                        throw new InvalidOperationException();
                                }
                            },
                            slaves_obj.ID,
                            IOEvent.ChildRekeyed
                        );

            File.Storage.Listeners.Add(listener_slaves_add);
            File.Storage.Listeners.Add(listener_slaves_remove);
            File.Storage.Listeners.Add(listener_slaves_keyed);

            jobs_listener_slaves_add.Add(jobID, listener_slaves_add);
            jobs_listener_slaves_remove.Add(jobID, listener_slaves_remove);
            jobs_listener_slaves_keyed.Add(jobID, listener_slaves_keyed);

            allocatedjobs_obj.Add(jobID.ToString(), job_obj.ID);

            return jobID;
        }

        public void StopJob(ComputeJobID jobID) {
            File.Storage.Listeners.Remove(jobs_listener_slaves_add[jobID]);
            File.Storage.Listeners.Remove(jobs_listener_slaves_remove[jobID]);
            File.Storage.Listeners.Remove(jobs_listener_slaves_keyed[jobID]);

            jobs_listener_slaves_add.Remove(jobID);
            jobs_listener_slaves_remove.Remove(jobID);
            jobs_listener_slaves_keyed.Remove(jobID);

            allocatedjobs_obj.Get(jobID.ToString()).Delete();
        }

        public static IFactory<IComputeCoordinator> FactoryInstance { get; } =
            new CtorFactory<IComputeCoordinator, MasterComputeCoordinator>(
                    ItemName,
                    false
                );
    }
}
