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
            var obj =
                file.Storage[storageobjectID];

            polylines = 
                new BoundList<PolylineData>(
                        obj.GetOrMake("polylines").ID,
                        file,
                        polylines_factoryset,
                        polylines_viewerset
                    );
        }

        public override void Bind() {
            polylines.Bind();

            base.Bind();
        }

        public override void Unbind() {
            polylines.Unbind();

            base.Unbind();
        }

        public static IFactory<IContainer> CreateFactory(
                FactorySet<PolylineData> polylines_factoryset,
                ViewerSet<PolylineData> polylines_viewerset
            ) =>
            new CtorFactory<IContainer, PolylineContainer>(
                    ItemName,
                    true,
                    polylines_factoryset,
                    polylines_viewerset
                );
    }
}
