﻿using MusicWriter.WinForms.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms {
    public partial class FileEditorForm : Form {
        EditorFile file;
        KeyboardInputSource input_keyboard =
            new KeyboardInputSource();
        InputController inputcontroller;
        KeyboardMenuShortcuts shortcutter =
            new KeyboardMenuShortcuts();
        CommandCenter commandcenter =
            new CommandCenter();
        readonly ObservableList<IPorter> porters =
            new ObservableList<IPorter>();
        readonly FactorySet<IContainer> containerfactoryset =
            new FactorySet<IContainer>();
        
        string filepath = null;

        public IScreen ActiveScreen {
            get { return (tabScreens.SelectedTab as TabPageInterop)?.Tag as IScreen; }
        }

        public FileEditorForm() {
            InitializeComponent();

            shortcutter.Menu = mnuMainMenu;
        }

        void InitInputSources() {
            inputcontroller = new InputController(commandcenter);

            input_keyboard.Controller = inputcontroller;
        }

        void InitContainers() {
            var controllers_factoryset =
                new FactorySet<ITrackController>(
                        SheetMusicEditor.CreateFactory()
                    );

            var controllers_viewerset =
                new ViewerSet<ITrackController>(
                        SheetMusicEditorView.Viewer.Instance
                    );

            var tracks_factoryset =
                new FactorySet<ITrack>(
                        MusicTrackFactory.Instance
                    );

            var tracks_viewerset =
                new ViewerSet<ITrack>();

            containerfactoryset.Factories.Add(
                    TrackControllerContainer.CreateFactory(
                            tracks_factoryset,
                            tracks_viewerset,
                            controllers_factoryset,
                            controllers_viewerset
                        )
                );

            containerfactoryset.Factories.Add(
                    PolylineContainer.CreateFactory(
                            new FactorySet<PolylineData>(
                                    PolylineData.FactoryInstance
                                ),
                            new ViewerSet<PolylineData>(
                                )
                        )
                );

            containerfactoryset.Factories.Add(
                    FunctionContainer.CreateFactory(
                            new FunctionCodeTools(
                                    SquareFunction.FactoryClass.Instance,
                                    PolylineFunction.FactoryClass.Instance,
                                    PolynomialFunction.FactoryClass.Instance,
                                    PulseWidthModulatedFunction.FactoryClass.Instance,

                                    LocalPerspectiveFunction.FactoryClass.Instance,
                                    GlobalPerspectiveFunction.FactoryClass.Instance
                                ),
                            new FactorySet<FunctionSource>(
                                    FunctionSource.FactoryInstance
                                ),
                            new ViewerSet<FunctionSource>(
                                )
                        )
                );

            containerfactoryset.Factories.Add(
                    ScreenContainer.CreateFactory(
                            new FactorySet<IScreen>(
                                    TrackControllerScreen.FactoryInstance
                                ),
                            new ViewerSet<IScreen>(
                                    TrackControllerScreenView.Viewer.Instance
                                )
                        )
                );
        }
        
        void InitPorters() {
            porters.ItemAdded += Porters_CollectionChanged;
            porters.ItemRemoved += Porters_CollectionChanged;

            porters.Add(new MidiPorter());
        }

        private void Porters_CollectionChanged(IPorter changed) {
            var filters_builder = new StringBuilder();

            bool first = true;
            foreach (var porter in porters) {
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

            mnuFileImport.DropDownItems.Clear();
            for (var i = 0; i < porters.Count; i++) {
                var porter = porters[i];

                var mnuFileImportPorter = new ToolStripMenuItem();
                mnuFileImportPorter.Text = $"{porter.Name} ({porter.FileExtension})";
                mnuFileImportPorter.Tag = i;
                mnuFileImportPorter.Click += mnuFileImportPorter_Click;
                mnuFileImport.DropDownItems.Add(mnuFileImportPorter);


                var mnuFileExportPorter = new ToolStripMenuItem();
                mnuFileExportPorter.Text = $"{porter.Name} ({porter.FileExtension})";
                mnuFileExportPorter.Tag = i;
                mnuFileExportPorter.Click += mnuFileExportPorter_Click;
                mnuFileExport.DropDownItems.Add(mnuFileExportPorter);
            }
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            switch (keyData) {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:

                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                    // for some reason, the arrow keys aren't fired when a controller is open
                    //if (ActiveScreen?.Controllers.Any() ?? false)
                    OnKeyDown(new KeyEventArgs(keyData));

                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
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
            SetupStatic();
            Setup();
        }

        void Setup() {
            var screencontainer =
                file[ScreenContainer.ItemName] as ScreenContainer;

            screencontainer.Screens.ItemAdded += LoadScreen;
            screencontainer.Screens.ItemRemoved += CloseScreen;
        }

        void SetupStatic() {
            InitInputSources();
            InitContainers();
            InitPorters();

            inputcontroller = new InputController(commandcenter);

            file =
                new EditorFile(
                        new MemoryStorageGraph(), 
                        containerfactoryset, 
                        isnewfile: true
                    );
        }
        
        void LoadScreen(IScreen screen) {
            var container =
                file[ScreenContainer.ItemName] as ScreenContainer;

            var view =
                container.Screens.ViewerSet.CreateView(screen, WinFormsViewer.Type) as Control;
            
            var tab = new TabPageInterop(view);
            tab.Name = $"tabScreen_{screen.Name}";
            tab.Tag = screen;

            screen.Name.AfterChange += Screen_Renamed;

            screen.CommandCenter.SubscribeTo(commandcenter);
            screen.CommandCenter.Enabled = false;

            tabScreens.Controls.Add(tab);
            if (tabScreens.Controls.Count == 1)
                tabScreens_SelectedIndexChanged(this, new EventArgs());
        }

        private void Screen_Renamed(string old, string @new) {
            var tab = tabScreens.Controls[$"tabScreen_{old}"];
            tab.Name = $"tabScreen_{@new}";
        }

        void CloseScreen(IScreen screen) {
            screen.CommandCenter.DesubscribeFrom(commandcenter);

            tabScreens.Controls.RemoveByKey($"tabScreen_{screen.Name}");
        }

        void OpenFile(string filename) {
            filepath = filename;

            IStorageGraph newgraph;
            bool isnewfile = false;

            if (string.IsNullOrEmpty(filename)) {
                newgraph = new MemoryStorageGraph();
                isnewfile = true;
            }
            else {
                switch (Path.GetExtension(filename)) {
                    case ".musicwriter":
                        var stream =
                            File.Open(
                                    path: filepath,
                                    mode: FileMode.OpenOrCreate,
                                    access: FileAccess.ReadWrite,
                                    share: FileShare.Read
                                );

                        var archive =
                            new ZipArchive(
                                    stream: stream,
                                    mode: ZipArchiveMode.Update | ZipArchiveMode.Read
                                );

                        newgraph = new ZipStorageGraph(archive);

                        break;

                    case ".musicwriter-dir":
                        throw new NotImplementedException();

                    default:
                        throw new NotSupportedException();
                }
            }

            tabScreens.Controls.Clear();

            file = new EditorFile(newgraph, containerfactoryset, isnewfile);
            Setup();

            RecentFiles.AddRecent(filepath);
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

        IScreen oldselected;
        private void tabScreens_SelectedIndexChanged(object sender, EventArgs e) {
            if (oldselected != null)
                oldselected.CommandCenter.Enabled = false;

            oldselected = ActiveScreen;

            if (oldselected != null)
                oldselected.CommandCenter.Enabled = true;
        }

        private void mnuFileNew_Click(object sender, EventArgs e) {
            OpenFile("");
        }

		private void mnuFileOpen_Click(object sender, EventArgs e) =>
			diagOpenFile.ShowDialog(this);

        private void mnuFileOpenRecent_DropDownOpening(object sender, EventArgs e) {
            var files =
                Settings
                    .Default
                    .RecentFiles;

            while (mnuFileOpenRecent.DropDownItems[0] is ToolStripMenuItem)
                mnuFileOpenRecent.DropDownItems.RemoveAt(0);

            if (files != null) {
                foreach (var file in files) {
                    var menuitem =
                        new ToolStripMenuItem();

                    menuitem.Text = Path.GetFileName(file);
                    menuitem.ToolTipText = file;
                    menuitem.Click += mnuFileOpenRecent_FileClicked;
                    menuitem.Tag = file;

                    mnuFileOpenRecent
                        .DropDownItems
                        .Insert(
                                mnuFileOpenRecent.DropDownItems.Count - 2,
                                menuitem
                            );
                }
            }
        }

        private void mnuFileOpenRecentClearRecent_Click(object sender, EventArgs e) =>
            RecentFiles.ClearRecents();

        private void mnuFileOpenRecent_FileClicked(object sender, EventArgs e) {
            var menuitem = (ToolStripMenuItem)sender;
            var file = (string)menuitem.Tag;

            OpenFile(file);
        }
        
		private void mnuFileImport_Click(object sender, EventArgs e) =>
			diagOpenImportFile.ShowDialog(this);

        private void mnuFileImportPorter_Click(object sender, EventArgs e) {
            var mnuFileImportPorter = (ToolStripMenuItem)sender;

            var index = (int)mnuFileImportPorter.Tag;

            diagOpenImportFile.FilterIndex = index;
            diagOpenImportFile.ShowDialog(this);
        }

        private void mnuFileSave_Click(object sender, EventArgs e) {
            if (filepath == null)
                mnuFileSaveAs_Click(sender, e);
            else {
                file.Flush();
                RecentFiles.AddRecent(filepath);
            }
        }

        private void mnuFileSaveAs_Click(object sender, EventArgs e) =>
            diagSaveFile.ShowDialog(this);

		private void mnuFileExport_Click(object sender, EventArgs e) =>
			diagSaveExportFile.ShowDialog(this);

        private void mnuFileExportPorter_Click(object sender, EventArgs e) {
            var mnuFileExportPorter = (ToolStripMenuItem)sender;

            var index = (int)mnuFileExportPorter.Tag;

            diagSaveExportFile.FilterIndex = index;
            diagSaveExportFile.ShowDialog(this);
        }

        private void mnuFilePrint_Click(object sender, EventArgs e) {

        }

        private void mnuFilePrintPreview_Click(object sender, EventArgs e) {

        }

        private void mnuFileExit_Click(object sender, EventArgs e) =>
            Close();

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

        private void mnuScreenNew_Click(object sender, EventArgs e) {
            var screencontainer =
                file[ScreenContainer.ItemName] as ScreenContainer;

            screencontainer.Screens.Create(screencontainer.Screens.FactorySet.Factories[0].Name);
        }
        
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
            IStorageGraph newgraph;

            switch (diagSaveFile.FilterIndex) {
                case 1:
                    // zip file
                    var stream = diagSaveFile.OpenFile();
                    var archive = new ZipArchive(stream, ZipArchiveMode.Update);
                    newgraph = new ZipStorageGraph(archive);

                    break;
                    
                default:
                    throw new NotSupportedException();
            }

            file.Storage.Transfer(newgraph);
            newgraph.Flush();

            OpenFile(diagSaveFile.FileName);
        }

        private void diagOpenFile_FileOk(object sender, CancelEventArgs e) =>
            OpenFile(diagOpenFile.FileName);

		private void diagSaveExportFile_FileOk(object sender, CancelEventArgs e) {
			var porter = porters[diagSaveExportFile.FilterIndex];

            var options =
                new PorterOptions {
                    PortTempo = true
                };

			porter.Export(file, diagSaveExportFile.FileName, options);
		}

		private void diagOpenImportFile_FileOk(object sender, CancelEventArgs e) {
			var porter = porters[diagSaveExportFile.FilterIndex];

            var options =
                new PorterOptions {
                    PortTempo = true
                };

            porter.Import(file, diagOpenImportFile.FileName, options);
		}

        private void FileEditorForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {

        }
    }
}
