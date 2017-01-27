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
    public sealed class TrackControllerScreen : Screen {
        public const string ItemName = "musicwriter.screens.track-controller";

        readonly TrackControllerContainer container;

        public override IFactory<IScreen> Factory {
            get { return TrackControllerScreenFactory.Instance; }
        }

        public ObservableList<ITrackController> Controllers { get; } =
            new ObservableList<ITrackController>();

        public TrackControllerScreen(
                StorageObjectID storageobjectID,
                EditorFile file
            ) : base(
                    storageobjectID,
                    file
                ) {
            SetupStorage();

            container = file[TrackControllerContainer.ItemName] as TrackControllerContainer;

            Controllers.ItemAdded += Controllers_ItemAdded;
            Controllers.ItemRemoved += Controllers_ItemRemoved;

            Name.AfterChange += Name_AfterChange;

            if (!file.Storage[storageobjectID].HasChild("inited")) {
                Init();

                file.Storage[storageobjectID].GetOrMake("inited");
            }
        }

        private void Name_AfterChange(string old, string @new) {
            container
                .Settings
                .GlobalCaret
                .RenameCaret(old, @new);
        }

        private void Controllers_ItemAdded(ITrackController obj) {
            obj.CommandCenter.SubscribeTo(CommandCenter);
        }

        private void Controllers_ItemRemoved(ITrackController obj) {
            obj.CommandCenter.DesubscribeFrom(CommandCenter);
        }

        void SetupStorage() {
            var obj =
                File.Storage[StorageObjectID];

            var controllersobj =
                obj.GetOrMake("controllers");

            controllersobj.ChildAdded += (controllersobjID, controllerobjID, key) => {
                var controller = file.GetController(controllerobjID);

                if (!Controllers.Contains(controller)) {
                    Controllers.Add(controller);
                    controller.Name.AfterChange += controllersobj.Rename;
                }
            };

            controllersobj.ChildRemoved += (controllersobjID, oldcontrollerobjID, key) => {
                var controller = Controllers.FirstOrDefault(_ => _.StorageObjectID == oldcontrollerobjID);

                if (controller != null) {
                    Controllers.Remove(controller);
                    controller.Name.AfterChange -= controllersobj.Rename;
                }
            };

            Controllers.ItemAdded += controller => {
                if (!controllersobj.HasChild(controller.Name.Value))
                    controllersobj.Add(controller.Name.Value, controller.StorageObjectID);
            };
            
            Controllers.ItemRemoved += controller => {
                controllersobj.Remove(controller.Name.Value);
            };

            var nameobj =
                obj.GetOrMake("name");
            
            nameobj.ContentsSet += nameobjID =>
                Name.Value = nameobj.ReadAllString();
            Name.AfterChange += (old, @new) =>
                nameobj.WriteAllString(@new);
        }
        
        void Init() {
            container
                .Settings
                .GlobalCaret
                .InitCaret(Name.Value);
        }
    }
}
