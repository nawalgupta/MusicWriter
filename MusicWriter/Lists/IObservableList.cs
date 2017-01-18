﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IObservableList<T> : IList<T>
    {
        event Action<T> ItemAdded;
        event Action<T> ItemRemoved;
    }
}