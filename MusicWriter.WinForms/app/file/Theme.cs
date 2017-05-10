using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms
{
    public sealed class Theme
    {
        readonly string fullpath;

        public string FullPath {
            get { return fullpath; }
        }

        public sealed class Line
        {
            readonly Color color;
            readonly float width;

            public Color Color {
                get { return color; }
            }

            public float Width {
                get { return width; }
            }

            public Line(
                    Color color,
                    float width
                ) {
                this.color = color;
                this.width = width;
            }
        }

        public sealed class Paint
        {
            readonly Color forecolor;
            readonly Color backcolor;

            public Color ForeColor {
                get { return forecolor; }
            }

            public Color BackColor {
                get { return backcolor; }
            }

            public Paint(
                    Color forecolor,
                    Color backcolor
                ) {
                this.forecolor = forecolor;
                this.backcolor = backcolor;
            }
        }

        private Theme(string fullpath) {
            this.fullpath = fullpath;

            if (!Directory.Exists(fullpath))
                Directory.CreateDirectory(fullpath);
        }

        public void MakeCurrent() {
            var current = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MusicWriter", "themes", "current");

            Directory.Move(current, $"{current}.old");
            Directory.Delete(current, true);
            DirectoryCopy(fullpath, current, true);
            //System.IO.Directory.Delete($"{current}.old");
        }

        //Credit: MSDN
        //https://msdn.microsoft.com/en-us/library/bb762914(v=vs.110).aspx
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs) {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists) {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName)) {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files) {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs) {
                foreach (DirectoryInfo subdir in dirs) {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static Theme Current { get; private set; } =
            new Theme(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MusicWriter", "themes", "current"));

        readonly static Dictionary<string, Themelet> themelets =
            new Dictionary<string, Themelet>();

        class Themelet
        {
            readonly FileSystemWatcher watcher;
            readonly string name;
            readonly List<Action<string>> responders =
                new List<Action<string>>();

            public Themelet(string name) {
                this.name = name;

                var path = Path.Combine(Current.FullPath, name);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                watcher = new FileSystemWatcher(path);
                watcher.IncludeSubdirectories = true;
                watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
                watcher.Created += Watcher_Event;
                watcher.Deleted += Watcher_Event;
                watcher.Changed += Watcher_Event;
                watcher.Renamed += Watcher_Renamed;
                watcher.EnableRaisingEvents = true;
            }

            public void AddResponder(Action<string> responder) {
                responders.Add(responder);
                responder(watcher.Path);
            }

            private void Watcher_Renamed(object sender, RenamedEventArgs e) {
                TellResponders();
            }

            private void Watcher_Event(object sender, FileSystemEventArgs e) {
                TellResponders();
            }

            void TellResponders() {
                foreach (var responder in responders)
                    responder(watcher.Path);
            }
        }

        public static void RegisterThemelet(string name, Action<string> loadsettings) =>
            themelets
                .Lookup(name, () => new Themelet(name))
                .AddResponder(loadsettings);

        public static void SerializeFont(string file, Font font) {
            var pieces = new List<string>();

            pieces.Add(font.Name);
            pieces.Add(font.Size.ToString());
            pieces.Add(((int)font.Style).ToString());

            File.WriteAllLines(file, pieces);
        }

        public static Font DeserializeFont(string file) {
            if (!File.Exists(file))
                return null;

            var pieces = File.ReadAllLines(file);

            return
                new Font(
                        pieces[0],
                        float.Parse(pieces[1]),
                        (FontStyle)int.Parse(pieces[2])
                    );
        }

        public static void SerializePaint(string file, Paint paint) {
            var pieces = new List<string>();

            pieces.Add(paint.ForeColor.R.ToString());
            pieces.Add(paint.ForeColor.G.ToString());
            pieces.Add(paint.ForeColor.B.ToString());
            pieces.Add(paint.BackColor.R.ToString());
            pieces.Add(paint.BackColor.G.ToString());
            pieces.Add(paint.BackColor.B.ToString());

            File.WriteAllLines(file, pieces);
        }

        public static Paint DeserializePaint(string file) {
            if (!File.Exists(file))
                return null;

            var pieces = File.ReadAllLines(file);
                
            return
                new Paint(
                        Color.FromArgb(
                                int.Parse(pieces[0]),
                                int.Parse(pieces[1]),
                                int.Parse(pieces[2])
                            ),
                        Color.FromArgb(
                                int.Parse(pieces[3]),
                                int.Parse(pieces[4]),
                                int.Parse(pieces[5])
                            )
                );
        }
    }
}
