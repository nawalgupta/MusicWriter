using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter
{
    public partial class FunctionEditorScreenView : UserControl
    {
        EditorFile file;
        FunctionEditorScreen screen;

        public EditorFile File {
            get { return file; }
            set {
                if (file != null)
                    throw new InvalidOperationException();

                file = value;
            }
        }

        public FunctionEditorScreen Screen {
            get { return screen; }
            set {
                if (screen != null)
                    throw new InvalidOperationException();

                screen = value;
            }
        }

        public FunctionEditorScreenView() {
            InitializeComponent();
        }

        void Setup() {
            ContextMenus.Attatch_Tone(mnuPlayTone, screen.DebugSound.Tone);
        }
        
        private void FunctionEditorScreenView_Load(object sender, EventArgs e) {
        }
    }
}
