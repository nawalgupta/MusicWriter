﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class ViewerSet<T>
    {
        public ObservableList<IViewer<T>> Viewers { get; } =
            new ObservableList<IViewer<T>>();

        public ViewerSet(params IViewer<T>[] viewers) {
            foreach (var viewer in viewers)
                Viewers.Add(viewer);
        }

        public object CreateView(T obj, string view) =>
            Viewers
                .First(viewer => viewer.SupportsView(view))
                .CreateView(obj, view);
    }
}
