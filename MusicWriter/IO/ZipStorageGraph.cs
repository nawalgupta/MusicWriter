using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class ZipStorageGraph : MemoryStorageGraph
    {
        readonly ZipArchive zipfile;

        public ZipArchive ZipFile {
            get { return zipfile; }
        }
        
        public ZipStorageGraph(ZipArchive zipfile) {
            this.zipfile = zipfile;

            Reload();
        }
        
        void Reload() {
            foreach (var entry in zipfile.Entries) {
                var id = StorageObjectID.Parse(entry.FullName.Split('/')[0]);

                if (!Contains(id)) {
                    DeserializeNode(id);
                    DeserializeArrows(id);
                }
            }
        }

        protected override void AddArrow(
                StorageObjectID source,
                StorageObjectID sink,
                string key
            ) {
            SerializeArrows(source);

            base.AddArrow(source, sink, key);
        }

        protected override void RenameArrow(
                StorageObjectID source,
                StorageObjectID sink,
                string newkey
            ) {
            SerializeArrows(source);

            base.RenameArrow(source, sink, newkey);
        }

        protected override void RemoveArrow(
                StorageObjectID source, 
                StorageObjectID sink
            ) {
            SerializeArrows(source);

            base.RemoveArrow(source, sink);
        }

        protected override void SetContents(StorageObjectID id) {
            SerializeNode(id);

            base.SetContents(id);
        }

        protected override void Archive(StorageObjectID id) {
            SerializeNode(id);

            base.Archive(id);
        }

        protected override void Unarchive(StorageObjectID id) {
            var storageobject =
                GetSpecialStorageObject(id);

            var path = $"{id}/dat";
            var entry =
                zipfile.GetEntry(path) ??
                zipfile.CreateEntry(path);
            var data = new byte[entry.Length];

            using (var stream = entry.Open()) {
                stream.Read(data, 0, data.Length);
            }

            storageobject.SetData(data);

            base.Unarchive(id);
        }

        void SerializeArrows(StorageObjectID source) {
            var path = $"{source}/rel";
            var entry =
                zipfile.GetEntry(path) ??
                zipfile.CreateEntry(path);

            using (var stream = entry.Open()) {
                using (var bw = new BinaryWriter(stream)) {
                    foreach (var outgoing in Outgoing(source)) {
                        bw.Write(outgoing.Key);
                        bw.Write(outgoing.Value.ID.ToByteArray());
                    }
                }
            }
        }

        void DeserializeArrows(StorageObjectID source) {
            var path = $"{source}/rel";
            var entry =
                zipfile.GetEntry(path) ??
                zipfile.CreateEntry(path);

            using (var stream = entry.Open()) {
                using (var br = new BinaryReader(stream)) {
                    while (br.BaseStream.Position != br.BaseStream.Length) {
                        var relation = br.ReadString();
                        var child = new StorageObjectID(new Guid(br.ReadBytes(16)));

                        AddArrow(source, child, relation);
                    }
                }
            }
        }

        void SerializeNode(StorageObjectID id) {
            var storageobject =
                GetSpecialStorageObject(id);

            var data = storageobject.GetData();
            var path = $"{id}/dat";
            var entry =
                zipfile.GetEntry(path) ??
                zipfile.CreateEntry(path);

            using (var stream = entry.Open()) {
                stream.Write(data, 0, data.Length);
            }
        }

        void DeserializeNode(StorageObjectID file) {
            Load(file, true);
        }
    }
}
