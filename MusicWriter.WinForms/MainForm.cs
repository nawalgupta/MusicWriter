using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms {
    public partial class MainForm : Form {
        MusicEditorFile file = new MusicEditorFile();

        public MainForm() {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            var track = file.Add("abc");

            track.Melody.AddNote(Tone.C5, new Duration { Start = Time.Zero, Length = Time.Note });
            track.Melody.AddNote(Tone.C5, new Duration { Start = Time.Note, Length = Time.Note_4th });

            Editor.LoadFrom(file, "abc");

            Editor.Invalidate();
        }
    }
}
