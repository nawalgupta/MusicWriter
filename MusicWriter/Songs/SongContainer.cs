using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    class SongContainer : Container
    {
        public const string ItemName = "musicwriter.song.container";
        public const string ItemCodeName = "song";

        readonly BoundList<Song> songs;

        public BoundList<Song> Songs {
            get { return songs; }
        }

        public SongContainer(
                StorageObjectID storageobjectID,
                EditorFile file,
                IFactory<IContainer> factory,

                FactorySet<Song> songs_factoryset,
                ViewerSet<Song> songs_viewerset
            ) : 
            base(
                    storageobjectID,
                    file, 
                    factory,
                    ItemName,
                    ItemCodeName
                ) {
            var obj =
                file.Storage[storageobjectID];
            
            songs =
                new BoundList<Song>(
                        obj.GetOrMake("songs").ID,
                        file,
                        songs_factoryset,
                        songs_viewerset
                    );
        }

        public static IFactory<IContainer> CreateFactory(
                FactorySet<Song> songs_factoryset,
                ViewerSet<Song> songs_viewerset
            ) =>
            new CtorFactory<IContainer, SongContainer>(
                    ItemName,
                    true,
                    songs_factoryset,
                    songs_viewerset
                );
    }
}
