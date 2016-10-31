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
    public partial class SheetMusicEditor : Control {
        public MelodyTrack Track { get; set; }

        public SheetMusicEditor() {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe) {
            

            base.OnPaint(pe);
        }
    }
}
