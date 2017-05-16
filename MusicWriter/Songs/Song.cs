using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class Song : BoundObject<Song>
    {
        public const string ItemName = "musicwriter.song";

        readonly BoundList<SongChannel> channels;
        readonly CompositeJobManager jobmanager = new CompositeJobManager();

        public BoundList<SongChannel> Channels {
            get { return channels; }
        }

        public IJobManager JobManager {
            get { return jobmanager; }
        }

        public Song(
                StorageObjectID storageobjectID,
                EditorFile file
            ) :
            base(
                    storageobjectID,
                    file,
                    FactoryInstance
                ) {
            var obj = file.Storage[storageobjectID];

            channels =
                new BoundList<SongChannel>(
                        obj.GetOrMake("channels").ID,
                        file,
                        new FactorySet<SongChannel>(SongChannel.FactoryInstance),
                        new ViewerSet<SongChannel>()
                    );

            channels.ItemAdded += Channels_ItemAdded;
            channels.ItemRemoved += Channels_ItemRemoved;
        }

        private void Channels_ItemAdded(SongChannel item) =>
            jobmanager.Jobs.Add(item.JobManager);

        private void Channels_ItemRemoved(SongChannel item) =>
            jobmanager.Jobs.Remove(item.JobManager);

        public override void Bind() {
            channels.Bind();

            base.Bind();
        }

        public override void Unbind() {
            channels.Unbind();

            base.Unbind();
        }

        public WavEncodingStream MakeWavStream() =>
            new WavEncodingStream(
                    channels
                        .Select(channel => channel.FunctionWave.Value)
                        .ToArray()
                );

        public static IFactory<Song> FactoryInstance { get; } =
            new CtorFactory<Song, Song>(
                    ItemName,
                    false
                );
    }
}
