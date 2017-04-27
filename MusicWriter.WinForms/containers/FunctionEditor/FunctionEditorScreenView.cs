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
        DebugSoundPlayer soundplayer;

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

        FunctionSource ActiveFunctionSource {
            get { return (FunctionSource)tabFunctionSources.SelectedTab.Tag; }
        }

        public FunctionEditorScreenView() {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e) {
            Setup();

            base.OnLoad(e);
        }

        void Setup() {
            ContextMenus.Attach_Tone(mnuPlayTone, screen.DebugSound.Tone);
            ContextMenus.Attach_PerceptualTime(mnuPlayLength, screen.DebugSound.Length);

            screen.Container.FunctionSources.ItemInserted += FunctionSources_ItemInserted;
            screen.Container.FunctionSources.ItemWithdrawn += FunctionSources_ItemWithdrawn;
            screen.Container.FunctionSources.ItemMoved += FunctionSources_ItemMoved;

            screen.FunctionSources.ItemInserted += Screen_FunctionSources_ItemInserted;
            screen.FunctionSources.ItemWithdrawn += Screen_FunctionSources_ItemWithdrawn;
            screen.FunctionSources.ItemMoved += Screen_FunctionSources_ItemMoved;

            soundplayer = new DebugSoundPlayer(screen.DebugSound);
            soundplayer.PlayingStarted += Soundplayer_PlayingStarted;
            soundplayer.PlayingFinished += Soundplayer_PlayingFinished;
        }

        private void Soundplayer_PlayingStarted() {
            btnPlay.Text = "Stop";
        }

        private void Soundplayer_PlayingFinished() {
            btnPlay.Text = "Play";
        }

        private void FunctionSources_ItemInserted(
                FunctionSource item,
                int index
            ) {
            Invoke(new Action(() => {
                var lsvItem = new ListViewItem(item.Name.Value);
                
                item.Name.AfterChange += FunctionSource_Name_AfterChange;

                lsvItem.Name = $"lsvFunctionSources_{item.Name}";
                lsvItem.Text = item.Name.Value;

                lsvFunctionSources.Items.Insert(index, lsvItem);

                lsvItem.Tag = item;
            }));
        }

        private void FunctionSource_Name_AfterChange(string old, string @new) {
            Invoke(new Action(() => {
                var lsvItem =
                    lsvFunctionSources.Items[$"lsvFunctionSources_{old}"];

                lsvItem.Name = $"lsvFunctionSources_{@new}";
                lsvItem.Text = @new;
            }));
        }

        private void FunctionSources_ItemWithdrawn(
                FunctionSource item,
                int index
            ) {
            Invoke(new Action(() => {
                lsvFunctionSources.Items.RemoveByKey($"lsvFunctionSources_{item.Name}");
            }));
        }

        private void FunctionSources_ItemMoved(
                FunctionSource item,
                int oldindex,
                int newindex
            ) {
            Invoke(new Action(() => {
                var lsvItem = lsvFunctionSources.Items[$"lsvFunctionSources_{item.Name}"];
                lsvFunctionSources.Items.Remove(lsvItem);
                lsvFunctionSources.Items.Insert(newindex, lsvItem);
            }));
        }

        private void Screen_FunctionSources_ItemInserted(
                FunctionSource item,
                int index
            ) {
            Invoke(new Action(() => {
                var control =
                    new FunctionSourceEditorControl();

                control.Setup(item);

                var tab =
                    new TabPage();

                tab.Tag = item;
                tab.Text = item.Name.Value;
                tab.Name = $"tabFunctionSources_{item.Name}";
                tab.Controls.Add(control);
                control.Dock = DockStyle.Fill;

                ObservableProperty<string>.PropertyChangeHandler namechanged =
                    (old, @new) => {
                        tab.Text = @new;
                        tab.Name = $"tabFunctionSources_{item.Name}";
                    };

                item.Name.AfterChange += namechanged;

                tab.Disposed += delegate {
                    control.UnSetup();
                    item.Name.AfterChange -= namechanged;
                };

                tabFunctionSources.Controls.Add(tab);
                tabFunctionSources.Controls.SetChildIndex(tab, index);

                var lsvFunctionSources_item =
                    lsvFunctionSources.Items[$"lsvFunctionSources_{item.Name}"];

                if (!lsvFunctionSources_item.Checked)
                    lsvFunctionSources_item.Checked = true;
            }));
        }

        private void Screen_FunctionSources_ItemWithdrawn(
                FunctionSource item,
                int index
            ) {
            Invoke(new Action(() => {
                tabFunctionSources
                    .Controls
                    .RemoveAt(index);

                var lsvFunctionSources_item =
                    lsvFunctionSources.Items[$"lsvFunctionSources_{item.Name}"];

                if (lsvFunctionSources_item.Checked)
                    lsvFunctionSources_item.Checked = false;
            }));
        }

        private void Screen_FunctionSources_ItemMoved(
                FunctionSource item,
                int oldindex,
                int newindex
            ) {
            Invoke(new Action(() => {
                tabFunctionSources
                    .Controls
                    .SetChildIndex(
                            tabFunctionSources.Controls[$"lsvFunctionSources_{item.Name}"],
                            newindex
                        );

                var lsvFunctionSources_item =
                    lsvFunctionSources.Items[$"lsvFunctionSources_{item.Name}"];

                if (lsvFunctionSources_item.Index != newindex)
                    throw new NotImplementedException();
            }));
        }
        
        private void FunctionEditorScreenView_Load(object sender, EventArgs e) {
        }

        private void tabFunctionSources_SelectedIndexChanged(object sender, EventArgs e) {
            var index = lsvFunctionSources.Items.IndexOfKey($"lsvFunctionSources_{ActiveFunctionSource.Name}");
            lsvFunctionSources.SelectedIndices.Clear();
            lsvFunctionSources.SelectedIndices.Add(index);

            var functionsource = tabFunctionSources.SelectedTab.Tag as FunctionSource;
            screen.DebugSound.FunctionSource.Value = functionsource;
        }

        private void tabFunctionSources_ControlAdded(object sender, ControlEventArgs e) {
            tabFunctionSources.SelectTab(tabFunctionSources.Controls.Count - 1);

            if (tabFunctionSources.Controls.Count == 1) {
                var functionsource = tabFunctionSources.SelectedTab.Tag as FunctionSource;
                screen.DebugSound.FunctionSource.Value = functionsource;
            }
        }

        private void lsvFunctionSources_SelectedIndexChanged(object sender, EventArgs e) {
            if (lsvFunctionSources.SelectedIndices.Count == 0)
                return;

            tabFunctionSources.SelectedIndex = lsvFunctionSources.SelectedIndices[0];
        }

        private void lsvPolylines_SelectedIndexChanged(object sender, EventArgs e) {
            
        }

        private void btnAddFunctionSource_Click(object sender, EventArgs e) {
            screen
                .FunctionSources
                .Create(FunctionSource.ItemName);
        }

        private void btnDeleteFunctionSource_Click(object sender, EventArgs e) {
            if (lsvFunctionSources.SelectedItems.Count == 0)
                MessageBox.Show(this, "Select a function source to delete it.", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else {
                var activeItem =
                    lsvFunctionSources.SelectedItems[0];

                var functionsource =
                    (FunctionSource)activeItem.Tag;

                functionsource.Delete();
            }
        }

        private void btnAddPolyline_Click(object sender, EventArgs e) {

        }

        private void btnDeletePolyline_Click(object sender, EventArgs e) {

        }

        private void mnuFunctionSourcesEnabled_CheckedChanged(object sender, EventArgs e) {
            foreach (ListViewItem item in lsvFunctionSources.SelectedItems)
                item.Checked = mnuFunctionSourcesEnabled.Checked;
        }

        private void mnuFunctionSourcesCreate_Click(object sender, EventArgs e) =>
            btnAddFunctionSource.PerformClick();

        private void mnuFunctionSourcesDelete_Click(object sender, EventArgs e) =>
            btnDeleteFunctionSource.PerformClick();

        private void mnuFunctionSources_Opening(object sender, CancelEventArgs e) {
            if (lsvFunctionSources.SelectedItems.Count == 0)
                return;

            var activeItem =
                lsvFunctionSources.SelectedItems[0];

            var functionsource =
                (FunctionSource)activeItem.Tag;

            mnuFunctionSourcesDelete.Enabled = lsvFunctionSources.SelectedIndices.Count != 0;
            mnuFunctionSourcesEnabled.Checked = screen.FunctionSources.Contains(functionsource);
        }

        private void lsvFunctionSources_ItemChecked(object sender, ItemCheckedEventArgs e) {
            var functionsource =
                (FunctionSource)e.Item.Tag;

            if (e.Item.Checked) {
                if (!screen.FunctionSources.Contains(functionsource))
                    screen.FunctionSources.Add(functionsource);
            }
            else {
                if (screen.FunctionSources.Contains(functionsource))
                    screen.FunctionSources.Remove(functionsource);
            }
        }

        private void btnPlay_Click(object sender, EventArgs e) {
            if (soundplayer.IsPlaying) {
                soundplayer.Stop();
            }
            else {
                soundplayer.Play();
            }
        }

        private void lsvFunctionSources_AfterLabelEdit(object sender, LabelEditEventArgs e) {
            var functionsource =
                (FunctionSource)lsvFunctionSources.Items[e.Item].Tag;
            
            functionsource.Name.Value = e.Label;
        }
    }
}
