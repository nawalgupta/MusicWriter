using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class SongScreen : Screen
    {
        public const string ItemName = "musicwriter.song.screen";

        readonly Selector<Song> songselector;

        public Selector<Song> SongSelector {
            get { return songselector; }
        }

        public SongScreen(
                StorageObjectID storageobjectID,
                EditorFile file
            ) :
            base(
                    storageobjectID,
                    file,
                    FactoryInstance
                ) {
            songselector =
                new Selector<Song>(
                        storageobjectID,
                        file,
                        file
                            [SongContainer.ItemName]
                            .As<IContainer, SongContainer>()
                            .Songs
                    );
        }

        public override void Bind() {
            songselector.Bind();

            base.Bind();
        }

        public override void Unbind() {
            songselector.Unbind();

            base.Unbind();
        }

        public static IFactory<IScreen> FactoryInstance { get; } =
            new CtorFactory<IScreen, SongScreen>(
                    ItemName,
                    false
                );
    }
}
