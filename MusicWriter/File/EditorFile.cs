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
        readonly MusicBrain brain = new MusicBrain();
        readonly Dictionary<string, ITrack> trackmap =
            new Dictionary<string, ITrack>();
        IStorageGraph storage = new MemoryStorageGraph();

        public IStorageGraph Storage {
            get { return storage; }
        }

        public ObservableList<ITrack> Tracks { get; } =
            new ObservableList<ITrack>();

        public  ObservableList<ITrackController<View>> Controllers { get; } =
            new ObservableList<ITrackController<View>>();

        public ObservableList<Screen<View>> Screens { get; } =
            new ObservableList<Screen<View>>();

        public FileCapabilities<View> Capabilities =
            new FileCapabilities<View>();
        
        public ITrack this[string name] {
            get { return trackmap[name]; }
        }

        public MusicBrain Brain {
            get { return brain; }
        }

        public EditorFile() {
            InitializeBrain();

            Tracks.ItemAdded += Tracks_ItemAdded;
            Tracks.ItemRemoved += Tracks_ItemRemoved;

            Controllers.ItemAdded += Controllers_ItemAdded;
            Controllers.ItemRemoved += Controllers_ItemRemoved;

            Screens.ItemAdded += Screens_ItemAdded;
            Screens.ItemRemoved += Screens_ItemRemoved;
        }

        public async Task<ITrack> CreateTrackAsync(string type) {
            var factory = Capabilities.TrackFactories.FirstOrDefault(_ => _.Name == type);
            var storageobjectID = storage.Create();

            factory.Init(storage[storageobjectID]);

            return await Task.Run(async () => {
                do {
                    var track = Tracks.FirstOrDefault(_ => _.StorageObjectID == storageobjectID);

                    if (track != null)
                        return track;

                    await Task.Delay(100);
                } while (true);
            });
        }

        public ITrack CreateTrack(string type) =>
            CreateTrackAsync(type)
                .GetAwaiter()
                .GetResult();

        public async Task<ITrackController<View>> CreateTrackControllerAsync(string type) {
            var factory = Capabilities.ControllerFactories.FirstOrDefault(_ => _.Name == type);
            var storageobjectID = storage.Create();

            factory.Init(storage[storageobjectID], this);

            return await Task.Run(async () => {
                do {
                    var controller = Controllers.FirstOrDefault(_ => _.StorageObjectID == storageobjectID);

                    if (controller != null)
                        return controller;

                    await Task.Delay(100);
                } while (true);
            });
        }

        public ITrackController<View> CreateTrackController(string type) =>
            CreateTrackControllerAsync(type)
                .GetAwaiter()
                .GetResult();

        public Screen<View> CreateScreen() {
            var storageobjectID = storage.Create();
            var screen = new Screen<View>(storageobjectID, this);

            return screen;
        }

        public ITrack GetTrack(StorageObjectID storageobjectID) =>
            Tracks.FirstOrDefault(track => track.StorageObjectID == storageobjectID);

        public ITrackController<View> GetController(StorageObjectID storageobjectID) =>
            Controllers.FirstOrDefault(controller => controller.StorageObjectID == storageobjectID);

        public Screen<View> GetScreen(StorageObjectID storageobjectID) =>
            Screens.FirstOrDefault(screen => screen.StorageObjectID == storageobjectID);

        void Track_Rename(string old, string @new) {
            if (trackmap.ContainsKey(@new))
                throw new InvalidOperationException("Cannot overwrite track");

            trackmap.Add(@new, trackmap[old]);
            trackmap.Remove(old);
        }

        void Tracks_ItemAdded(ITrack track) {
            if (trackmap.ContainsKey(track.Name.Value))
                throw new InvalidOperationException("Cannot add track that already exists");

            var nameobj = storage[track.StorageObjectID].GetOrMake("name");

            track.Name.Value = nameobj.ReadAllString();
            nameobj.ContentsChanged += nameobjID =>
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
        }

        private void Controllers_ItemRemoved(ITrackController<View> controller) {
            storage[storage.Root]
                .GetOrMake("controllers")
                .Remove(controller.StorageObjectID);
        }

        private void Screens_ItemAdded(Screen<View> screen) {
        }

        private void Screens_ItemRemoved(Screen<View> screen) {
            storage[storage.Root]
                .GetOrMake("screens")
                .Remove(screen.StorageObjectID);
        }
        
        public void Clear() {
            trackmap.Clear();
            Tracks.Clear();
            Controllers.Clear();
            Screens.Clear();
        }

        public void Transfer(IStorageGraph newgraph) {
            var translationIDmap =
                new Dictionary<StorageObjectID, StorageObjectID>();

            foreach (var oldobjID in storage.Objects)
                translationIDmap.Add(oldobjID, newgraph.Create());

            foreach (var oldobjID in storage.Objects) {
                var newobjID = translationIDmap[oldobjID];
                var newobj = newgraph[newobjID];

                foreach (var outgoing in storage.Outgoing(oldobjID))
                    newobj.Add(outgoing.Key, translationIDmap[outgoing.Value]);
            }

            //TOOD: how do I reset everything?

            Reload();
        }
        
        void Reload() {
            var tracks =
                storage[storage.Root].GetOrMake("tracks");

            tracks.ChildAdded += (tracksnodeID, newtrackID) => {
                var trackobj = storage[newtrackID];
                
                var type = storage[trackobj["type"]].ReadAllString();

                var trackfactory =
                    Capabilities
                        .TrackFactories
                        .FirstOrDefault(_ => _.Name == type);

                var track = trackfactory.Load(trackobj);
                
                Tracks.Add(track);
            };

            tracks.ChildRemoved += (tracksnodeID, oldtrackID) => {
                var track = Tracks.FirstOrDefault(_ => _.StorageObjectID == oldtrackID);

                if (track != null)
                    Tracks.Remove(track);
            };

            var controllers =
                storage[storage.Root].GetOrMake("controllers");

            controllers.ChildAdded += (controllersnodeID, newcontrollerID) => {
                var controllerobj = storage[newcontrollerID];

                var type = storage[controllerobj["type"]].ReadAllString();

                var controllerfactory =
                    Capabilities
                        .ControllerFactories
                        .FirstOrDefault(_ => _.Name == type);

                var controller = controllerfactory.Load(controllerobj, this);

                Controllers.Add(controller);
            };

            controllers.ChildRemoved += (controllersnodeID, oldcontrollerID) => {
                var controller = Controllers.FirstOrDefault(_ => _.StorageObjectID == oldcontrollerID);

                if (controller != null)
                    Controllers.Remove(controller);
            };

            var screens =
                storage[storage.Root].GetOrMake("screens");

            screens.ChildAdded += (screensnodeID, newscreenID) => {
                var screen = new Screen<View>(newscreenID, this);
                Screens.Add(screen);
            };

            screens.ChildRemoved += (screensnodeID, oldscreenID) => {
                var screen = Screens.FirstOrDefault(_ => _.StorageObjectID == oldscreenID);

                if (screen != null)
                    Screens.Remove(screen);
            };
        }
        
        void InitializeBrain() {
            brain.InsertCog(new NotePerceptualCog());
            //brain.InsertCog(new NoteLayoutPerceptualCog());
            //brain.InsertCog(new ChordLayoutPerceptualCog());
            brain.InsertCog(new MeasureLayoutPerceptualCog());
        }
    }
}
