﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class EditorFile {
        readonly FactorySet<IContainer> containerfactoryset;
        readonly BoundList<IContainer> containers;
        readonly IStorageGraph storage;
        
        public IStorageGraph Storage {
            get { return storage; }
        }
        
        public FactorySet<IContainer> ContainerFactorySet {
            get { return containerfactoryset; }
        }

        public IContainer this[string containername] {
            get { return containers[containername]; }
        }

        public EditorFile(
                IStorageGraph storage,
                FactorySet<IContainer> containerfactoryset,
                bool isnewfile = false
            ) {
            this.storage = storage;
            this.containerfactoryset = containerfactoryset;

            var obj = storage[storage.Root];

            containers =
                new BoundList<IContainer>(
                        storage.Root,
                        this,
                        containerfactoryset,
                        new ViewerSet<IContainer>()
                    );

            containers.Bind();

            if (isnewfile)
                containers.CreateAllObjects();
        }

        public void Flush() =>
            storage.Flush();
    }
}
