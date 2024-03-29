﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IStoredDataFunction : IFunction
    {
        string BinaryKey(EditorFile file);

        StorageObjectID StorageObjectID { get; }
    }
}
