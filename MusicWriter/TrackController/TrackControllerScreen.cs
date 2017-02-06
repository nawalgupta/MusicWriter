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
    public sealed class TrackControllerScreen : Screen {
        public const string ItemName = "musicwriter.screens.track-controller";

        readonly TrackControllerContainer container;
        readonly BoundList<ITrackController> controllers;

        public override IFactory<IScreen> Factory {
            get { return TrackControllerScreenFactory.Instance; }
        }

        public BoundList<ITrackController> Controllers {
            get { return controllers; }
        }

        public TrackControllerScreen(
                StorageObjectID storageobjectID,
                EditorFile file
            ) :
            base(
                    storageobjectID,
                    file
                ) {
            container = file[TrackControllerContainer.ItemName] as TrackControllerContainer;

            var obj =
                file.Storage[storageobjectID];

            controllers =
                new BoundList<ITrackController>(
                        obj.GetOrMake("controllers").ID,
                        file
                    );

            Controllers.ItemAdded += Controllers_ItemAdded;
            Controllers.ItemRemoved += Controllers_ItemRemoved;

            Name.AfterChange += container.Settings.GlobalCaret.RenameCaret;

            if (!file.Storage[storageobjectID].HasChild("inited")) {
                Init();

                file.Storage[storageobjectID].GetOrMake("inited");
            }
        }

        private void Controllers_ItemAdded(ITrackController obj) {
            obj.CommandCenter.SubscribeTo(CommandCenter);
        }

        private void Controllers_ItemRemoved(ITrackController obj) {
            obj.CommandCenter.DesubscribeFrom(CommandCenter);
        }
        
        void Init() {
            container
                .Settings
                .GlobalCaret
                .InitCaret(Name.Value);
        }
    }
}
