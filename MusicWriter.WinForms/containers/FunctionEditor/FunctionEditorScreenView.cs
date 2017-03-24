using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms
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

            screen.FunctionSources.ItemInserted += FunctionSources_ItemInserted;
            screen.FunctionSources.ItemWithdrawn -= FunctionSources_ItemWithdrawn;
            screen.FunctionSources.ItemMoved -= FunctionSources_ItemMoved;
        }

        private void FunctionSources_ItemInserted(
                FunctionSource item, 
                int index
            ) {
            var control =
                new FunctionSourceEditorControl();

            control.Setup(item);

            var tab = 
                new TabPage();

            tab.Controls.Add(control);
            control.Dock = DockStyle.Fill;

            ObservableProperty<string>.PropertyChangeHandler namechanged =
                (old, @new) => tab.Text = @new;

            item.Name.AfterChange += namechanged;
            
            tab.Disposed += delegate {
                control.UnSetup();
                item.Name.AfterChange -= namechanged;
            };

            tabFunctionSources.Controls.Add(tab);
        }
        
        private void FunctionSources_ItemWithdrawn(
                FunctionSource item,
                int index
            ) =>
            tabFunctionSources
                .Controls
                .RemoveAt(index);

        private void FunctionSources_ItemMoved(
                FunctionSource item, 
                int oldindex,
                int newindex
            ) =>
            tabFunctionSources
                .Controls
                .SetChildIndex(
                        tabFunctionSources.Controls[oldindex],
                        newindex
                    );
        
        private void FunctionEditorScreenView_Load(object sender, EventArgs e) {
        }

        private void tabFunctionSources_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void tabFunctionSources_ControlAdded(object sender, ControlEventArgs e) =>
            tabFunctionSources.SelectedIndex = tabFunctionSources.Controls.Count - 1;
    }
}
