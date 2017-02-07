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
        
        public BoundList<PolylineData> Polylines {
            get { return polylines; }
        }

        public PolylineContainer(
                StorageObjectID storageobjectID, 
                EditorFile file,
                IFactory<IContainer> factory,

                FactorySet<PolylineData> polylines_factoryset,
                ViewerSet<PolylineData> polylines_viewerset
            ) : 
            base(
                    storageobjectID, 
                    file,
                    factory,
                    ItemName, 
                    ItemCodename
                ) {
            polylines = 
                new BoundList<PolylineData>(
                        storageobjectID,
                        file,
                        polylines_factoryset,
                        polylines_viewerset
                    );
        }

        public static IFactory<IContainer> CreateFactory(
                FactorySet<PolylineData> polylines_factoryset,
                ViewerSet<PolylineData> polylines_viewerset
            ) =>
            new CtorFactory<IContainer, PolylineContainer>(
                    ItemName,
                    polylines_factoryset,
                    polylines_viewerset
                );
    }
}
