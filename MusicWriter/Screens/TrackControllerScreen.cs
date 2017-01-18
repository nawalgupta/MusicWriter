﻿using System;
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
    public sealed class TrackControllerScreen<View> : IScreen<View> {
        readonly StorageObjectID storageobjectID;
        readonly EditorFile<View> file;
        readonly CommandCenter commandcenter = new CommandCenter();

        public ObservableProperty<string> Name { get; } =
            new ObservableProperty<string>("Screen");
        
        public StorageObjectID StorageObjectID {
            get { return storageobjectID; }
        }

        public EditorFile<View> File {
            get { return file; }
        }

        public CommandCenter CommandCenter {
            get { return commandcenter; }
        }

        public IScreenFactory<View> Factory {
            get { return TrackControllerScreenFactory<View>.Instance; }
        }

        public ObservableList<ITrackController<View>> Controllers { get; } =
            new ObservableList<ITrackController<View>>();

        public TrackControllerScreen(
                StorageObjectID storageobjectID,
                EditorFile<View> file
            ) {
            this.storageobjectID = storageobjectID;
            this.file = file;

            SetupStorage();

            Controllers.ItemAdded += Controllers_ItemAdded;
            Controllers.ItemRemoved += Controllers_ItemRemoved;

            Name.AfterChange += Name_AfterChange;

            if (!file.Storage[storageobjectID].HasChild("inited")) {
                Init();

                file.Storage[storageobjectID].GetOrMake("inited");
            }
        }

        private void Name_AfterChange(string old, string @new) {
            file
                .TrackSettings
                .GlobalCaret
                .RenameCaret(old, @new);
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
            file
                .TrackSettings
                .GlobalCaret
                .InitCaret(Name.Value);
        }
    }
}