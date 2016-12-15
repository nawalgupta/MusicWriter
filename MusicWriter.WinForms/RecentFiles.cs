using MusicWriter.WinForms.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms
{
    public static class RecentFiles
    {
        public static void AddRecent(string file) {
            if (Settings.Default.RecentFiles == null)
                Settings.Default.RecentFiles = new StringCollection();

            int i =
                Settings.Default.RecentFiles.IndexOf(file);
            
            if (i == -1)
                Settings.Default.RecentFiles.Add(file);
            else if (i > 0) {
                Settings.Default.RecentFiles.RemoveAt(i--);

                Settings.Default.RecentFiles.Insert(i, file);
            }

            Settings.Default.Save();
        }
    }
}
