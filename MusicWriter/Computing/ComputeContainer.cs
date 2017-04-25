using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class ComputeContainer : Container
    {
        public const string ItemName = "musicwriter.computing.container";
        public const string ItemCodeName = "compute";

        readonly DistributedMutex masterallocator;
        readonly IComputeCoordinator coordinator;
        readonly bool volunteeroverseer;

        public IComputeCoordinator Coordinator {
            get { return coordinator; }
        }

        public bool VolunteerOverseer {
            get { return volunteeroverseer; }
        }

        public IObservableList<IComputeSlave> Slaves { get; } =
            new ObservableList<IComputeSlave>();

        public IObservableList<IComputePartitioner> Partitioners { get; } =
            new ObservableList<IComputePartitioner>();

        public ComputeContainer(
                StorageObjectID storageobjectID, 
                EditorFile file,
                IFactory<IContainer> factory,
                bool volunteeroverseer,
                IComputeSlave[] slaves,
                IComputePartitioner[] partitioners
            ) :
            base(
                    storageobjectID, 
                    file,
                    factory,
                    ItemName,
                    ItemCodeName
                ) {
            var obj =
                file.Storage[storageobjectID];

            var coordinator_obj =
                obj.GetOrMake("coordinator");

            this.volunteeroverseer = volunteeroverseer;

            Slaves.AddRange(slaves);
            Partitioners.AddRange(partitioners);

            var master =
                new MasterComputeCoordinator(
                        coordinator_obj.ID,
                        file
                    );

            Partitioners.Bind(master.Partitioners);

            masterallocator =
                new DistributedMutex(
                        obj.GetOrMake("master-mutex").ID,
                        file
                    );

            masterallocator.TryToOwn = volunteeroverseer;
            masterallocator.Acquired += master.Bind;
            masterallocator.Released += master.Unbind;

            coordinator = 
                new ProxyComputeCoordinator(
                        coordinator_obj.ID,
                        file
                    );

            Slaves.Bind((coordinator as ProxyComputeCoordinator).Slaves);
        }

        public override void Bind() {
            masterallocator.Bind();
            coordinator.Bind();

            base.Bind();
        }

        public override void Unbind() {
            masterallocator.Unbind();
            coordinator.Unbind();

            base.Unbind();
        }

        public static IFactory<IContainer> CreateFactory(
                bool volunteeroverseer,
                IComputeSlave[] slaves,
                IComputePartitioner[] partitioners
            ) =>
            new CtorFactory<IContainer, ComputeContainer>(
                    ItemName,
                    true,
                    volunteeroverseer,
                    slaves,
                    partitioners
                );
    }
}
