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
        
        public BoundList<ITrackController> Controllers {
            get { return controllers; }
        }

        public TrackControllerScreen(
                StorageObjectID storageobjectID,
                EditorFile file,
                IFactory<IScreen> factory
            ) :
            base(
                    storageobjectID,
                    file,
                    factory
                ) {
            container = file[TrackControllerContainer.ItemName] as TrackControllerContainer;

            var obj =
                file.Storage[storageobjectID];

            controllers =
                new BoundList<ITrackController>(
                        obj.GetOrMake("controllers").ID,
                        file,
                        container.Controllers
                    );
        }

        private void Controllers_ItemAdded(ITrackController obj) {
            obj.CommandCenter.SubscribeTo(CommandCenter);
        }

        private void Controllers_ItemRemoved(ITrackController obj) {
            obj.CommandCenter.DesubscribeFrom(CommandCenter);
        }

        public override void Bind() {
            controllers.Bind();

            controllers.ItemAdded += Controllers_ItemAdded;
            controllers.ItemRemoved += Controllers_ItemRemoved;

            Name.AfterChange += container.Settings.GlobalCaret.RenameCaret;

            if (!this.Object<TrackControllerScreen, IScreen>().HasChild("inited")) {
                Init();

                this.Object<TrackControllerScreen, IScreen>().GetOrMake("inited");
            }

            base.Bind();
        }

        public override void Unbind() {
            controllers.Unbind();

            base.Unbind();
        }

        void Init() {
            container
                .Settings
                .GlobalCaret
                .InitCaret(Name.Value);
        }

        public static IFactory<IScreen> FactoryInstance { get; } =
            new CtorFactory<IScreen, TrackControllerScreen>(ItemName);
    }
}
