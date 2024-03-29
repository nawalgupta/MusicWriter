﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class ScreenContainer : Container
    {
        public const string ItemName = "musicwriter.screen.container";
        public const string ItemCodeName = "screen";

        readonly BoundList<IScreen> screens;

        public BoundList<IScreen> Screens {
            get { return screens; }
        }
        
        public ScreenContainer(
                StorageObjectID storageobjectID,
                EditorFile file,
                IFactory<IContainer> factory,
                FactorySet<IScreen> screens_factoryset,
                ViewerSet<IScreen> screens_viewerset
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

            screens =
                new BoundList<IScreen>(
                        obj.GetOrMake("screens").ID,
                        file,
                        screens_factoryset,
                        screens_viewerset
                    );
        }

        public override void Bind() {
            screens.Bind();

            base.Bind();
        }

        public override void Unbind() {
            screens.Unbind();

            base.Unbind();
        }

        public static IFactory<IContainer> CreateFactory(
                FactorySet<IScreen> screens_factoryset,
                ViewerSet<IScreen> screens_viewerset
            ) =>
            new CtorFactory<IContainer, ScreenContainer>(
                    ItemName,
                    screens_factoryset,
                    screens_viewerset
                );
    }
}
