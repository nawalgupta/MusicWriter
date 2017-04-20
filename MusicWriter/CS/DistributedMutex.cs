using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class DistributedMutex
        : BoundObject<DistributedMutex>
    {
        readonly IStorageObject obj;

        readonly Guid guid;
        readonly string guidstring;
        readonly IOListener
            listener_contenetsset,
            listener_childadded;
        readonly IStorageObject flag_obj;
        readonly BackgroundWorker worker;

        public IStorageObject Obj {
            get { return obj; }
        }

        public event Action Acquired;
        public event Action Released;

        bool isacquired = false;
        public bool IsAcquired {
            get { return isacquired; }
            set {
                var wasacquired = isacquired;

                isacquired = value;

                if (isacquired && !wasacquired)
                    Acquired?.Invoke();
                else if (!isacquired && wasacquired)
                    Released?.Invoke();
            }
        }

        public int TimeoutMilliseconds { get; set; } = 1000;

        bool trytoown = false;
        public bool TryToOwn {
            get { return trytoown; }
            set {
                if (trytoown && !value)
                    if (IsAcquired)
                        IsAcquired = false;

                trytoown = value;
            }
        }

        public DistributedMutex(
                    StorageObjectID storageobjectID,
                    EditorFile file
                ) :
            base(
                    storageobjectID,
                    file
                ) {
            obj = file.Storage[storageobjectID];

            guid = Guid.NewGuid();
            guidstring = guid.ToString();
            
            flag_obj = file.Storage.CreateObject();
            flag_obj.WriteAllString(guidstring);

            //TODO: this code could be made really cool with a blockchain
            // voting system. But this will work for now.

            listener_contenetsset =
                obj
                    .CreateListen(
                            IOEvent.ObjectContentsSet,
                            () => {
                                if (IsAcquired) {
                                    if (obj.ReadAllString() != guidstring)
                                        obj.WriteAllString(guidstring);
                                }
                            }
                        );

            listener_childadded =
                obj
                    .CreateListen(
                            IOEvent.ChildAdded,
                            child_objID => {
                                if (IsAcquired)
                                    obj.Remove(child_objID);
                            }
                        );
            
            worker =
                new BackgroundWorker(() => {
                    while (true) {
                        if (TryToOwn)
                            if (obj.HasChild(flag_obj.ID))
                                IsAcquired = true;

                        Task.Delay(TimeoutMilliseconds).Wait();

                        if (TryToOwn)
                            obj.Add("", flag_obj.ID);
                    }
                });

            Acquired += DistributedMutex_Acquired;
            Released += DistributedMutex_Released;
        }

        private void DistributedMutex_Acquired() {
            obj.WriteAllString(guidstring);
            foreach (var child in obj.Children.ToArray())
                obj.Remove(child);
        }

        private void DistributedMutex_Released() {
        }

        public override void Bind() {
            worker.Start();
            obj.Graph.Listeners.Add(listener_contenetsset);
            obj.Graph.Listeners.Add(listener_childadded);

            base.Bind();
        }

        public override void Unbind() {
            IsAcquired = false;
            worker.Stop();
            obj.Graph.Listeners.Remove(listener_contenetsset);
            obj.Graph.Listeners.Remove(listener_childadded);

            base.Unbind();
        }
    }
}
