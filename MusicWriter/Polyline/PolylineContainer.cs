using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class PolylineContainer : Container
    {
        readonly BoundList<PolylineData> polylines;

        public const string ItemName = "musicwriter.data.polyline.container";
        public const string ItemCodename = "polyline";

        public override IFactory<IContainer> Factory {
            get { return FactoryInstance; }
        }

        public BoundList<PolylineData> Polylines {
            get { return polylines; }
        }

        public PolylineContainer(
                StorageObjectID storageobjectID, 
                EditorFile file
            ) : 
            base(
                    storageobjectID, 
                    file, 
                    ItemName, 
                    ItemCodename
                ) {
            polylines = new BoundList<PolylineData>(storageobjectID, file);
        }

        public static IFactory<IContainer> FactoryInstance { get; } =
            new CtorFactory<IContainer, PolylineContainer>(ItemName);
    }
}
