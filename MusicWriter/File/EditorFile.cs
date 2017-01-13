using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicWriter.TimeSignature;

namespace MusicWriter {
    public sealed class EditorFile<View> {
        readonly Dictionary<string, ITrack> trackmap =
            new Dictionary<string, ITrack>();
        IStorageGraph storage;
        TrackSettings tracksettings;
        FileCapabilities<View> capabilities;

        public IStorageGraph Storage {
            get { return storage; }
        }

        public TrackSettings TrackSettings {
            get { return tracksettings; }
        }

        public FileCapabilities<View> Capabilities {
            get { return capabilities; }
        }

        public ObservableList<ITrack> Tracks { get; } =
            new ObservableList<ITrack>();

        public ObservableList<ITrackController<View>> Controllers { get; } =
            new ObservableList<ITrackController<View>>();

        public ObservableList<Screen<View>> Screens { get; } =
            new ObservableList<Screen<View>>();

        public ITrack this[string name] {
            get { return trackmap[name]; }
        }

        public EditorFile(
                IStorageGraph storage,
                FileCapabilities<View> capabilities) {
            this.storage = storage;
            this.capabilities = capabilities;

            Tracks.ItemAdded += Tracks_ItemAdded;
            Tracks.ItemRemoved += Tracks_ItemRemoved;

            Controllers.ItemAdded += Controllers_ItemAdded;
            Controllers.ItemRemoved += Controllers_ItemRemoved;

            Screens.ItemAdded += Screens_ItemAdded;
            Screens.ItemRemoved += Screens_ItemRemoved;

            Setup();
        }

        public ITrack CreateTrack(string type) {
            var factory = Capabilities.TrackFactories.FirstOrDefault(_ => _.Name == type);
            var storageobjectID = storage.Create();
            
            storage[storageobjectID].GetOrMake("type").WriteAllString(type);

            var unique_name = "Track";

            while (Tracks.Any(_ => _.Name.Value == unique_name))
                unique_name += "_";

            storage[storageobjectID].GetOrMake("name").WriteAllString(unique_name);

            factory.Init(storage[storageobjectID], tracksettings);
            storage[storage.Root].GetOrMake("tracks").Add("", storageobjectID);

            do {
                var track = Tracks.FirstOrDefault(_ => _.StorageObjectID == storageobjectID);

                if (track != null)
                    return track;

                Task.Delay(20).Wait();
            } while (true);
        }
        
        public ITrackController<View> CreateTrackController(string type) {
            var factory = Capabilities.ControllerFactories.FirstOrDefault(_ => _.Name == type);
            var storageobjectID = storage.Create();

            var type_obj = storage.CreateObject();
            type_obj.WriteAllString(type);

            storage[storageobjectID].Add("type", type_obj.ID);

            factory.Init(storage[storageobjectID], this);
            storage[storage.Root].GetOrMake("controllers").Add("", storageobjectID);

            do {
                var controller = Controllers.FirstOrDefault(_ => _.StorageObjectID == storageobjectID);

                if (controller != null)
                    return controller;

                Task.Delay(20).Wait();
            } while (true);
        }

        public Screen<View> CreateScreen() {
            var storageobjectID = storage.Create();
            var name_obj = storage.CreateObject();
            name_obj.WriteAllString("Screen");
            storage[storageobjectID].Add("name", name_obj.ID);

            storage[storage.Root].GetOrMake("screens").Add("", storageobjectID);

            do {
                var screen = Screens.FirstOrDefault(_ => _.StorageObjectID == storageobjectID);

                if (screen != null)
                    return screen;

                Task.Delay(20).Wait();
            } while (true);
        }

        public ITrack GetTrack(StorageObjectID storageobjectID) =>
            Tracks.FirstOrDefault(track => track.StorageObjectID == storageobjectID);

        public ITrackController<View> GetController(StorageObjectID storageobjectID) =>
            Controllers.FirstOrDefault(controller => controller.StorageObjectID == storageobjectID);

        public ITrackController<View> GetController(string name) =>
            Controllers.FirstOrDefault(controller => controller.Name.Value == name);

        public Screen<View> GetScreen(StorageObjectID storageobjectID) =>
            Screens.FirstOrDefault(screen => screen.StorageObjectID == storageobjectID);

        void Track_Rename(string old, string @new) {
            if (trackmap.ContainsKey(@new))
                throw new InvalidOperationException("Cannot overwrite track");

            storage
                [storage.Root]
                .GetOrMake("tracks")
                .Rename(old, @new);

            trackmap.Add(@new, trackmap[old]);
            trackmap.Remove(old);
        }

        void Tracks_ItemAdded(ITrack track) {
            if (trackmap.ContainsKey(track.Name.Value))
                throw new InvalidOperationException("Cannot add track that already exists");

            var nameobj = storage[track.StorageObjectID].Get("name");

            nameobj.ContentsSet += nameobjID =>
                track.Name.Value = nameobj.ReadAllString();

            track.Name.BeforeChange += Track_Rename;

            trackmap.Add(track.Name.Value, track);
        }

        void Tracks_ItemRemoved(ITrack track) {
            if (!trackmap.ContainsKey(track.Name.Value))
                throw new InvalidOperationException("Cannot delete track - doesn't exist");

            track.Name.BeforeChange -= Track_Rename;
            trackmap.Remove(track.Name.Value);

            storage[storage.Root]
                .GetOrMake("tracks")
                .Remove(track.StorageObjectID);
        }

        private void Controllers_ItemAdded(ITrackController<View> controller) {
            controller.Name.BeforeChange += Controllers_Renaming;
            controller.Name.AfterChange += Controllers_Renamed;
        }

        private void Controllers_Renaming(string oldname, string newname) {
            var same =
                Controllers.FirstOrDefault(controller => controller.Name.Value == newname);

            if (same != null) {
                var proposal = same.Name.Value;
                while (Controllers.Any(controller => controller.Name.Value == proposal))
                    proposal += "_";

                same.Name.Value = proposal;
            }
        }

        private void Controllers_Renamed(string old, string @new) {
            storage
                [storage.Root]
                .GetOrMake("controllers")
                .Rename(old, @new);
        }

        private void Controllers_ItemRemoved(ITrackController<View> controller) {
            storage[storage.Root]
                .GetOrMake("controllers")
                .Remove(controller.StorageObjectID);
        }

        private void Screens_ItemAdded(Screen<View> screen) {
            screen.Name.BeforeChange += Screen_Renaming;
        }

        private void Screen_Renaming(string oldname, string newname) {
            var same =
                Screens.FirstOrDefault(screen => screen.Name.Value == newname);

            if (same != null) {
                var proposal = same.Name.Value;
                while (Screens.Any(screen => screen.Name.Value == proposal))
                    proposal += "_";

                same.Name.Value = proposal;
            }
        }

        private void Screens_ItemRemoved(Screen<View> screen) {
            storage[storage.Root]
                .GetOrMake("screens")
                .Remove(screen.StorageObjectID);
        }
        
        void Setup() {
            tracksettings =
                new TrackSettings(
                        storage
                            [storage.Root]
                            .GetOrMake("track-settings")
                    );

            var tracks =
                storage[storage.Root].GetOrMake("tracks");

            tracks.ChildAdded += (tracksnodeID, newtrackID, key) => {
                var trackobj = storage[newtrackID];

                var type = trackobj.Get("type").ReadAllString();

                var trackfactory =
                    Capabilities
                        .TrackFactories
                        .FirstOrDefault(_ => _.Name == type);

                var track = trackfactory.Load(trackobj, tracksettings);
                track.Name.Value = trackobj.Get("name").ReadAllString();
                tracks.Rename(newtrackID, track.Name.Value);

                Tracks.Add(track);
            };

            tracks.ChildRemoved += (tracksnodeID, oldtrackID, key) => {
                var track = Tracks.FirstOrDefault(_ => _.StorageObjectID == oldtrackID);

                if (track != null)
                    Tracks.Remove(track);
            };

            var controllers =
                storage[storage.Root].GetOrMake("controllers");

            controllers.ChildAdded += (controllersnodeID, newcontrollerID, key) => {
                var controllerobj = storage[newcontrollerID];

                var type = controllerobj.Get("type").ReadAllString();

                var controllerfactory =
                    Capabilities
                        .ControllerFactories
                        .FirstOrDefault(_ => _.Name == type);

                var controller = controllerfactory.Load(controllerobj, this);
                controllers.Rename(newcontrollerID, controller.Name.Value);

                Controllers.Add(controller);
            };

            controllers.ChildRemoved += (controllersnodeID, oldcontrollerID, key) => {
                var controller = Controllers.FirstOrDefault(_ => _.StorageObjectID == oldcontrollerID);

                if (controller != null)
                    Controllers.Remove(controller);
            };

            var screens =
                storage[storage.Root].GetOrMake("screens");

            screens.ChildAdded += (screensnodeID, newscreenID, key) => {
                var screen = new Screen<View>(newscreenID, this);
                Screens.Add(screen);
            };

            screens.ChildRemoved += (screensnodeID, oldscreenID, key) => {
                var screen = Screens.FirstOrDefault(_ => _.StorageObjectID == oldscreenID);

                if (screen != null)
                    Screens.Remove(screen);
            };
        }

        public void Flush() =>
            storage.Flush();
    }
}
