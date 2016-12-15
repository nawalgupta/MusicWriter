using MusicWriter.WinForms.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms {
    public partial class FileEditorForm : Form {
        EditorFile<Control> file = new EditorFile<Control>();
        KeyboardInputSource input_keyboard =
            new KeyboardInputSource();
        InputController inputcontroller;
        KeyboardMenuShortcuts shortcutter =
            new KeyboardMenuShortcuts();
        CommandCenter commandcenter =
            new CommandCenter();

        string filepath = null;

        public Screen<Control> ActiveScreen {
            get { return (tabScreens.SelectedTab as ScreenView)?.Screen; }
        }

        public FileEditorForm() {
            InitializeComponent();

            shortcutter.Menu = mnuMainMenu;
            inputcontroller = new InputController(commandcenter);
            file.Screens.CollectionChanged += Screens_CollectionChanged;
        }

        IEnumerable<T> ExpandWierdArgs<T>(IList wierdlist) where T : class {
            if (wierdlist == null)
                yield break;

            foreach (var x in wierdlist) {
                var list =
                    x as IList<T>;

                var t =
                    x as T;

                if (t != null)
                    yield return t;

                if (list != null)
                    foreach (T t2 in list)
                        yield return t2;
            }
        }

        private void Screens_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            var newitems = ExpandWierdArgs<Screen<Control>>(e.NewItems).ToArray();

            if (e.Action == NotifyCollectionChangedAction.Reset) 
                tabScreens.Controls.Clear();
            else {
                if (e.OldItems != null) {
                    for (var i = 0; i < e.OldItems.Count; i++) {
                        if (tabScreens.Controls.Count > i) {
                            tabScreens.Controls.RemoveAt(e.OldStartingIndex + i);

                            var screen = (Screen<Control>)e.OldItems[i];
                        }
                    }
                }

                for (var i = 0; i < newitems.Length; i++) {
                    var screen = newitems[i];

                    LoadScreen(screen);
                }
            }
        }

        void InitInputSources() {
            input_keyboard.Controller = inputcontroller;
        }

        void InitControllerFactories() {
            file.Capabilities.ControllerFactories.Add(SheetMusicEditor.FactoryClass.Instance);
        }

        void InitTrackFactories() {
            file.Capabilities.TrackFactories.Add(MusicTrackFactory.Instance);
        }

		void InitPorters() {
			file.Capabilities.Porters.CollectionChanged += Porters_CollectionChanged;

			file.Capabilities.Porters.Add(new MidiPorter());
		}

		private void Porters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			var filters_builder = new StringBuilder();

			bool first = true;
			foreach (var porter in file.Capabilities.Porters) {
				if (first)
					first = false;
				else {
					filters_builder.Append("|");
				}

				filters_builder.Append(porter.Name);
				filters_builder.Append(" (");
				filters_builder.Append(porter.FileExtension);
				filters_builder.Append(")|");
				filters_builder.Append(porter.FileExtension);
			}

			var filter = filters_builder.ToString();

			diagOpenImportFile.Filter = filter;
			diagSaveExportFile.Filter = filter;
		}

		List<Keys> keys_pressed = new List<Keys>();
        protected override void OnKeyDown(KeyEventArgs e) {
            if (!keys_pressed.Contains(e.KeyCode)) {
                input_keyboard.OnKeyDown(e);

                shortcutter.OnKeyDown(e);

                keys_pressed.Add(e.KeyCode);
            }
            
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            input_keyboard.OnKeyUp(e);
            shortcutter.OnKeyUp(e);

            if (keys_pressed.Contains(e.KeyCode))
                keys_pressed.Remove(e.KeyCode);

            base.OnKeyUp(e);
        }

        private void MainForm_Load(object sender, EventArgs e) {
            InitControllerFactories();
            InitTrackFactories();
            InitInputSources();
			InitPorters();
        }

        void NewScreen() {
            file.Screens.Add(new Screen<Control>(file));

            tabScreens.SelectTab(tabScreens.Controls.Count - 1);
        }

        void LoadScreen(Screen<Control> screen) {
            ScreenView view = new ScreenView();
            view.File = file;
            view.Screen = screen;

            screen.CommandCenter.SubscribeTo(commandcenter);
            screen.CommandCenter.Enabled = false;

            tabScreens.Controls.Add(view);
            if (tabScreens.Controls.Count == 1)
                tabScreens_SelectedIndexChanged(this, new EventArgs());
        }

        void CloseScreen(Screen<Control> screen) {
            ScreenView tab = (ScreenView)tabScreens.Controls[$"tabScreen_{screen.Name}"];
            tab.Screen.CommandCenter.DesubscribeFrom(commandcenter);

            tabScreens.Controls.Remove(tab);
        }

        void OpenFile(string filename) {
            filepath = filename;

            using (var stream = File.OpenRead(filepath)) {
                file.Load(stream);
            }

            if (Settings.Default.RecentFiles == null)
                Settings.Default.RecentFiles = new StringCollection();

            Settings
                .Default
                .RecentFiles
                .Add(filename);

            Settings
                .Default
                .Save();
        }

        private void mnuHeader_Opening(object sender, CancelEventArgs e) {
            mnuHeaderRenameBox.Text = ActiveScreen.Name.Value;
            ActiveScreen.Name.AfterChange += ActiveScreen_Name_AfterChange;
        }

        private void mnuHeader_Closing(object sender, ToolStripDropDownClosingEventArgs e) {
            ActiveScreen.Name.AfterChange -= ActiveScreen_Name_AfterChange;
        }

        private void ActiveScreen_Name_AfterChange(string old, string @new) {
            if (mnuHeaderRenameBox.Text != @new)
                mnuHeaderRenameBox.Text = @new;
        }

        private void mnuHeaderRenameBox_TextChanged(object sender, EventArgs e) {
            ActiveScreen.Name.Value = mnuHeaderRenameBox.Text;
        }

        private void mnuHeaderClose_Click(object sender, EventArgs e) {
            CloseScreen(ActiveScreen);
        }

        private void tabScreens_SelectedIndexChanged(object sender, EventArgs e) {
            foreach (ScreenView view in tabScreens.Controls) {
                var selected = ReferenceEquals(view, tabScreens.SelectedTab);

                view.Screen.CommandCenter.Enabled = selected;
            }
        }

        private void mnuFileNew_Click(object sender, EventArgs e) {

        }

		private void mnuFileOpen_Click(object sender, EventArgs e) =>
			diagOpenFile.ShowDialog(this);

        private void mnuFileOpenRecent_DropDownOpening(object sender, EventArgs e) {
            var files =
                Settings
                    .Default
                    .RecentFiles;

            mnuFileOpenRecent.DropDownItems.Clear();

            if (files != null) {
                foreach (var file in files) {
                    var menuitem =
                        new ToolStripMenuItem();

                    menuitem.Text = Path.GetFileName(file);
                    menuitem.ToolTipText = file;
                    menuitem.Click += mnuFileOpenRecent_FileClicked;
                    menuitem.Tag = file;

                    mnuFileOpenRecent.DropDownItems.Add(menuitem);
                }
            }
        }

        private void mnuFileOpenRecent_FileClicked(object sender, EventArgs e) {
            var menuitem = (ToolStripMenuItem)sender;
            var file = (string)menuitem.Tag;

            OpenFile(file);
        }
        
		private void mnuFileImport_Click(object sender, EventArgs e) =>
			diagOpenImportFile.ShowDialog(this);

        private void mnuFileSave_Click(object sender, EventArgs e) {
            if (filepath == null)
                mnuFileSaveAs_Click(sender, e);
            else {
                using (var stream = new FileStream(filepath, FileMode.Create)) {
                    file.Save(stream);
                }
            }
        }

        private void mnuFileSaveAs_Click(object sender, EventArgs e) =>
            diagSaveFile.ShowDialog(this);

		private void mnuFileSaveExport_Click(object sender, EventArgs e) =>
			diagSaveExportFile.ShowDialog(this);

        private void mnuFileExport_Click(object sender, EventArgs e) {

        }

        private void mnuFilePrint_Click(object sender, EventArgs e) {

        }

        private void mnuFilePrintPreview_Click(object sender, EventArgs e) {

        }

        private void mnuFileExit_Click(object sender, EventArgs e) {

        }

        private void mnuEditUndo_Click(object sender, EventArgs e) {

        }

        private void mnuEditRedo_Click(object sender, EventArgs e) {

        }

        private void mnuEditCut_Click(object sender, EventArgs e) {

        }

        private void mnuEditCopy_Click(object sender, EventArgs e) {

        }

        private void mnuEditPaste_Click(object sender, EventArgs e) {

        }

        private void mnuEditSelectAll_Click(object sender, EventArgs e) =>
            commandcenter.SelectAll();

        private void mnuEditDeselectAll_Click(object sender, EventArgs e) =>
            commandcenter.DeselectAll();

        private void mnuEditToggleSelectAll_Click(object sender, EventArgs e) =>
            commandcenter.ToggleSelectAll();

        private void mnuEditErase_Click(object sender, EventArgs e) =>
            commandcenter.EraseSelection();

        private void mnuEditDelete_Click(object sender, EventArgs e) =>
            commandcenter.DeleteSelection();

        private void mnuViewCursorDouble_Click(object sender, EventArgs e) =>
            commandcenter.MultiplyCursor(2);

        private void mnuViewCursorHalf_Click(object sender, EventArgs e) =>
            commandcenter.DivideCursor(2);

        private void mnuViewCursor3rd_Click(object sender, EventArgs e) =>
            commandcenter.DivideCursor(3);

        private void mnuViewCursor5th_Click(object sender, EventArgs e) =>
            commandcenter.DivideCursor(5);

        private void mnuViewCursor7th_Click(object sender, EventArgs e) =>
            commandcenter.DivideCursor(7);

        private void mnuViewCursorResetToOneNote_Click(object sender, EventArgs e) =>
            commandcenter.ResetCursorToOne();

        private void mnuScreenNew_Click(object sender, EventArgs e) =>
            NewScreen();

        private void mnuScreenArchive_Click(object sender, EventArgs e) {
            //TODO
        }

        private void mnuToolsCustomize_Click(object sender, EventArgs e) {

        }

        private void mnuToolsOptions_Click(object sender, EventArgs e) {

        }

        private void mnuHelpContents_Click(object sender, EventArgs e) {

        }

        private void mnuHelpIndex_Click(object sender, EventArgs e) {

        }

        private void mnuHelpSearch_Click(object sender, EventArgs e) {

        }

        private void mnuHelpAbout_Click(object sender, EventArgs e) {

        }

        private void diagSaveFile_FileOk(object sender, CancelEventArgs e) {
            filepath = diagSaveFile.FileName;

            mnuFileSave_Click(sender, e);
        }

        private void diagOpenFile_FileOk(object sender, CancelEventArgs e) =>
            OpenFile(diagOpenFile.FileName);

		private void diagSaveExportFile_FileOk(object sender, CancelEventArgs e) {
			var porter = file.Capabilities.Porters[diagSaveExportFile.FilterIndex - 1];

			porter.Export(file, diagSaveExportFile.FileName);
		}

		private void diagOpenImportFile_FileOk(object sender, CancelEventArgs e) {
			var porter = file.Capabilities.Porters[diagSaveExportFile.FilterIndex - 1];

			porter.Import(file, diagOpenImportFile.FileName);
		}
    }
}
