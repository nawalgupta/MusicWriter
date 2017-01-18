using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public ObservableList<IScreen<View>> Screens { get; } =
            new ObservableList<IScreen<View>>();

        public ITrack this[string name] {
            get { return trackmap[name]; }
        }

        public EditorFile(
                IStorageGraph storage,
                FileCapabilities<View> capabilities
            ) {
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
            storage[storage.Root].GetOrMake("tracks").Add(unique_name, storageobjectID);

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

            var unique_name = "Controller";
            while (Controllers.Any(_ => _.Name.Value == unique_name))
                unique_name += "_";
            storage[storageobjectID].GetOrMake("name").WriteAllString(unique_name);

            factory.Init(storage[storageobjectID], this);
            storage[storage.Root].GetOrMake("controllers").Add(unique_name, storageobjectID);

            do {
                var controller = Controllers.FirstOrDefault(_ => _.StorageObjectID == storageobjectID);

                if (controller != null)
                    return controller;

                Task.Delay(20).Wait();
            } while (true);
        }

        public IScreen<View> CreateScreen(string type) {
            var factory = Capabilities.ScreenFactories.FirstOrDefault(_ => _.Name == type);
            var storageobjectID = storage.Create();
            
            var unique_name = "Screen";
            while (Screens.Any(_ => _.Name.Value == unique_name))
                unique_name += "_";
            storage[storageobjectID].GetOrMake("name").WriteAllString(unique_name);
            storage[storageobjectID].GetOrMake("type").WriteAllString(type);

            factory.Init(storageobjectID, this);

            storage[storage.Root].GetOrMake("screens").Add(unique_name, storageobjectID);

            do {
                var screen = Screens.FirstOrDefault(_ => _.StorageObjectID == storageobjectID);

                if (screen != null)
                    return screen;

                Task.Delay(20).Wait();
            } while (true);
        }

        public ITrack GetTrack(StorageObjectID storageobjectID) =>
            Tracks.FirstOrDefault(track => track.StorageObjectID == storageobjectID);

        ITrackController<View> GetControllerByTracksObjID(StorageObjectID storageobjectID) =>
            Controllers.FirstOrDefault(controller => storage.HasChild(controller.StorageObjectID, storageobjectID));

        public ITrackController<View> GetController(StorageObjectID storageobjectID) =>
            Controllers.FirstOrDefault(controller => controller.StorageObjectID == storageobjectID);

        public ITrackController<View> GetController(string name) =>
            Controllers.FirstOrDefault(controller => controller.Name.Value == name);

        public IScreen<View> GetScreen(StorageObjectID storageobjectID) =>
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

            var controller_obj =
                storage[controller.StorageObjectID];

            var controller_tracks_obj =
                controller_obj.GetOrMake("tracks");

            controller_tracks_obj.ChildAdded += Controller_tracks_obj_ChildAdded;
            controller_tracks_obj.ChildRemoved += Controller_tracks_obj_ChildRemoved;

            Action<ITrack> track_added =
                track => controller_tracks_obj.Add(track.Name.Value, track.StorageObjectID);

            Action<ITrack> track_removed =
                track => controller_tracks_obj.Remove(track.StorageObjectID);

            controller.Tracks.ItemAdded += track_added;
            controller.Tracks.ItemRemoved += track_removed;
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
                .Get("controllers")
                .Rename(old, @new);
        }

        private void Controllers_ItemRemoved(ITrackController<View> controller) {
            var controller_obj =
                storage[controller.StorageObjectID];

            var controller_tracks_obj =
                controller_obj.Get("tracks");

            controller.Tracks.Clear();

            controller_tracks_obj.ChildAdded -= Controller_tracks_obj_ChildAdded;
            controller_tracks_obj.ChildRemoved -= Controller_tracks_obj_ChildRemoved;

            storage[storage.Root]
                .Get("controllers")
                .Remove(controller.StorageObjectID);
            
            //TODO: clear the controller.Tracks.ItemAdded and ItemRemoved delegates
        }

        private void Screens_ItemAdded(IScreen<View> screen) {
            screen.Name.BeforeChange += Screen_Renaming;
            screen.Name.AfterChange += Screen_Renamed;
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

        private void Screen_Renamed(string oldname, string newname) {
            storage
                [storage.Root]
                .Get("screens")
                .Rename(oldname, newname);
        }

        private void Screens_ItemRemoved(IScreen<View> screen) {
            screen.Name.BeforeChange -= Screen_Renaming;
            screen.Name.AfterChange -= Screen_Renamed;

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

            var tracks_obj =
                storage[storage.Root].GetOrMake("tracks");

            tracks_obj.ChildAdded += (tracksnodeID, newtrackID, key) => {
                var trackobj = storage[newtrackID];

                var type = trackobj.Get("type").ReadAllString();

                var trackfactory =
                    Capabilities
                        .TrackFactories
                        .FirstOrDefault(_ => _.Name == type);

                var track = trackfactory.Load(trackobj, tracksettings);
                track.Name.Value = trackobj.Get("name").ReadAllString();
                tracks_obj.Rename(newtrackID, track.Name.Value);

                if (key != track.Name.Value)
                    throw new InvalidOperationException();

                Tracks.Add(track);
            };

            tracks_obj.ChildRemoved += (tracksnodeID, oldtrackID, key) => {
                var track = Tracks.FirstOrDefault(_ => _.StorageObjectID == oldtrackID);

                if (key != track.Name.Value)
                    throw new InvalidOperationException();

                if (track != null)
                    Tracks.Remove(track);
            };

            var controllers_obj =
                storage[storage.Root].GetOrMake("controllers");

            controllers_obj.ChildAdded += (controllersnodeID, newcontrollerID, key) => {
                var controller_obj = storage[newcontrollerID];

                var type = controller_obj.Get("type").ReadAllString();

                var controllerfactory =
                    Capabilities
                        .ControllerFactories
                        .FirstOrDefault(_ => _.Name == type);

                var controller = controllerfactory.Load(controller_obj, this);
                controller.Name.Value = controller_obj.Get("name").ReadAllString();
                controllers_obj.Rename(newcontrollerID, controller.Name.Value);

                if (controller.Name.Value != key)
                    throw new InvalidOperationException();

                Controllers.Add(controller);

                var controller_tracks_obj =
                    controller_obj.GetOrMake("tracks");
            };

            controllers_obj.ChildRemoved += (controllersnodeID, oldcontrollerID, key) => {
                var controller = Controllers.FirstOrDefault(_ => _.StorageObjectID == oldcontrollerID);
                
                if (controller.Name.Value != key)
                    throw new InvalidOperationException();

                if (controller != null)
                    Controllers.Remove(controller);
            };

            var screens_obj =
                storage[storage.Root].GetOrMake("screens");

            screens_obj.ChildAdded += (screensnodeID, newscreenID, key) => {
                var type = storage[newscreenID].Get("type").ReadAllString();
                var factory = capabilities.ScreenFactories.FirstOrDefault(_ => _.Name == type);
                
                var screen = factory.Load(newscreenID, this);
                Screens.Add(screen);
            };

            screens_obj.ChildRemoved += (screensnodeID, oldscreenID, key) => {
                var screen = Screens.FirstOrDefault(_ => _.StorageObjectID == oldscreenID);

                if (screen.Name.Value != key)
                    throw new InvalidOperationException();

                if (screen != null)
                    Screens.Remove(screen);
            };
        }

        private void Controller_tracks_obj_ChildAdded(
                StorageObjectID container,
                StorageObjectID child,
                string key
            ) {
            var controller =
                GetControllerByTracksObjID(container);

            var track =
                GetTrack(child);

            if (key != track.Name.Value)
                throw new InvalidOperationException();

            if (!controller.Tracks.Contains(track))
                controller.Tracks.Add(track);
        }

        private void Controller_tracks_obj_ChildRemoved(
                StorageObjectID container,
                StorageObjectID child,
                string key
            ) {
            var controller =
                GetControllerByTracksObjID(container);

            var track =
                GetTrack(child);

            if (key != track.Name.Value)
                throw new InvalidOperationException();

            if (controller.Tracks.Contains(track))
                controller.Tracks.Remove(track);
        }

        public void Flush() =>
            storage.Flush();
    }
}
