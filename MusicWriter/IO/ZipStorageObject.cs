using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class ZipStorageObject : IStorageObject
    {
        readonly ZipArchive archive;

        public ZipArchive Archive {
            get { return archive; }
        }

        public ZipStorageObject(ZipArchive archive) {
            this.archive = archive;
        }

        public Stream Read(string key) =>
            archive.GetEntry(key).Open();

        public Stream Write(string key) =>
            archive.GetEntry(key).Open();
    }
}
