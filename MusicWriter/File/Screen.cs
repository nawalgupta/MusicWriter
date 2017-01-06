using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MusicWriter {
    public sealed class Screen<View> {
        readonly StorageObjectID storageobjectID;
        readonly EditorFile<View> file;
        readonly CommandCenter commandcenter = new CommandCenter();

        public ObservableProperty<string> Name { get; } =
            new ObservableProperty<string>("");
        
        public StorageObjectID StorageObjectID {
            get { return storageobjectID; }
        }

        public CommandCenter CommandCenter {
            get { return commandcenter; }
        }

        public ObservableList<ITrackController<View>> Controllers { get; } =
            new ObservableList<ITrackController<View>>();

        public Screen(
                StorageObjectID storageobjectID,
                EditorFile<View> file
            ) {
            this.storageobjectID = storageobjectID;
            this.file = file;

            SetupStorage();

            Controllers.ItemAdded += Controllers_ItemAdded;
            Controllers.ItemRemoved += Controllers_ItemRemoved;
        }

        private void Controllers_ItemAdded(ITrackController<View> obj) {
            obj.CommandCenter.SubscribeTo(commandcenter);
        }

        private void Controllers_ItemRemoved(ITrackController<View> obj) {
            obj.CommandCenter.DesubscribeFrom(commandcenter);
        }

        void SetupStorage() {
            var obj =
                file.Storage[storageobjectID];

            var controllersobj =
                obj.GetOrMake("controllers");

            controllersobj.ChildAdded += (controllersobjID, controllerobjID) => {
                Controllers.Add(file.GetController(controllerobjID));
            };

            controllersobj.ChildRemoved += (controllersobjID, oldcontrollerobjID) => {
                var controller = Controllers.FirstOrDefault(_ => _.StorageObjectID == oldcontrollerobjID);

                if (controller != null)
                    Controllers.Remove(controller);
            };

            Controllers.ItemAdded += controller => {
                controllersobj.Add("", controller.StorageObjectID);
            };
            
            Controllers.ItemRemoved += controller => {
                controllersobj.Remove(controller.StorageObjectID);
            };

            var nameobj =
                obj.GetOrMake("name");
            
            nameobj.ContentsSet += nameobjID =>
                Name.Value = nameobj.ReadAllString();
            Name.AfterChange += (old, @new) =>
                nameobj.WriteAllString(@new);
        }
    }
}
