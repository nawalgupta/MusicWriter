using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms.Screens.FunctionEditor
{
    public partial class FunctionEditorScreenView : UserControl
    {
        FunctionEditorScreen screen;

        public FunctionEditorScreenView() {
            InitializeComponent();
        }

        private void FunctionEditorScreenView_Load(object sender, EventArgs e) {
            ContextMenus.Attatch_Tone(mnuPlayTone, screen.DebugSound.Tone);
        }
    }
}
