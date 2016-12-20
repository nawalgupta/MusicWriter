using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IStorageObject
    {
        Stream Read(string key);

        Stream Write(string key);
    }
}
