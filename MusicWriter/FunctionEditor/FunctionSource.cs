using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed partial class FunctionSource : IBoundObject<FunctionSource>
    {
        readonly StorageObjectID storageobjectID;
        readonly FunctionContainer container;

        public const string ItemName = "musicwriter.function.source";

        public ObservableProperty<string> Name { get; } =
            new ObservableProperty<string>(ItemName);

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

        public EditorFile File {
            get { return container.File; }
        }

        public IFactory<FunctionSource> Factory {
            get { return FactoryInstance; }
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
            var obj = File.Storage[storageobjectID];

            Code.AfterChange += Code_AfterChange;

            Code.Bind(obj.GetOrMake("code"));
        }

        private void Code_AfterChange(string old, string @new) {
            throw new NotImplementedException();

            //var errors =
            //    new List<KeyValuePair<Tuple<int, int>, string>>();

            //Function.Value =
            //        container
            //            .FunctionCodeTools
            //            .Parse(
            //                    ref @new,
            //                    container.File.AssortedFilesManager,
            //                    errors
            //                );
        }

        public static readonly IFactory<FunctionSource> FactoryInstance =
            new FuncFactory<FunctionSource>(
                    ItemName,
                    (storageobjectID, file) => { },
                    (storageobjectID, file) => 
                        new FunctionSource(
                                storageobjectID,
                                file[FunctionContainer.ItemName] as FunctionContainer
                            )
                );
    }
}
