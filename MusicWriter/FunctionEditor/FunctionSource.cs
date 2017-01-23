using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FunctionSource
    {
        readonly StorageObjectID storageobjectID;
        readonly FunctionContainer container;

        public ObservableProperty<string> Code { get; } =
            new ObservableProperty<string>();

        public ObservableProperty<IFunction> Function { get; } =
            new ObservableProperty<IFunction>();

        public StorageObjectID StorageObjectID {
            get { return storageobjectID; }
        }

        public FunctionContainer Container {
            get { return container; }
        }

        public FunctionSource(
                StorageObjectID storageobjectID,
                FunctionContainer container
            ) {
            this.storageobjectID = storageobjectID;
            this.container = container;

            Setup();
        }

        void Setup() {
        }
    }
}
