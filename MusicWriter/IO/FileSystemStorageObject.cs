using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FileSystemStorageObject : IStorageObject
    {
        readonly string path;

        public string Path {
            get { return path; }
        }

        public FileSystemStorageObject(string path) {
            this.path = path;
        }

        public Stream Read(string key) =>
            File.OpenRead(System.IO.Path.Combine(path, key));

        public Stream Write(string key) =>
            File.OpenWrite(System.IO.Path.Combine(path, key));
    }
}
