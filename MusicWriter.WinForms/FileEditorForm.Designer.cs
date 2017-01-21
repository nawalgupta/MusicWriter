namespace MusicWriter.WinForms {
    partial class FileEditorForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileEditorForm));
            System.Windows.Forms.ToolStripSeparator mnuFileOpenRecentSeparator;
            this.mnuMainMenu = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileOpenRecent = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileImport = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileExport = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFilePrint = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFilePrintPreview = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditCut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditDeselectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditToggleSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditErase = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewCursorDouble = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewCursorHalf = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewCursor3rd = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewCursor5th = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewCursor7th = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuViewCursorResetToOneNote = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuScreen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuScreenNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuScreenArchive = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsCustomize = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpContents = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tabScreens = new System.Windows.Forms.TabControl();
            this.mnuHeader = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuHeaderRenameBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuHeaderClose = new System.Windows.Forms.ToolStripMenuItem();
            this.diagSaveFile = new System.Windows.Forms.SaveFileDialog();
            this.diagOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.diagOpenImportFile = new System.Windows.Forms.OpenFileDialog();
            this.diagSaveExportFile = new System.Windows.Forms.SaveFileDialog();
            this.mnuFileOpenRecentClearRecent = new System.Windows.Forms.ToolStripMenuItem();
            mnuFileOpenRecentSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.mnuMainMenu.SuspendLayout();
            this.mnuHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuMainMenu
            // 
            this.mnuMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuView,
            this.mnuScreen,
            this.mnuTools,
            this.mnuHelp});
            this.mnuMainMenu.Location = new System.Drawing.Point(0, 0);
            this.mnuMainMenu.Name = "mnuMainMenu";
            this.mnuMainMenu.Size = new System.Drawing.Size(743, 24);
            this.mnuMainMenu.TabIndex = 0;
            this.mnuMainMenu.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileNew,
            this.mnuFileOpen,
            this.mnuFileOpenRecent,
            this.mnuFileImport,
            this.mnuFileSeparator1,
            this.mnuFileSave,
            this.mnuFileSaveAs,
            this.mnuFileExport,
            this.mnuFileSeparator2,
            this.mnuFilePrint,
            this.mnuFilePrintPreview,
            this.mnuFileSeparator3,
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(37, 20);
            this.mnuFile.Text = "&File";
            // 
            // mnuFileNew
            // 
            this.mnuFileNew.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileNew.Image")));
            this.mnuFileNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuFileNew.Name = "mnuFileNew";
            this.mnuFileNew.ShortcutKeyDisplayString = "Ctrl+N";
            this.mnuFileNew.Size = new System.Drawing.Size(185, 22);
            this.mnuFileNew.Text = "&New";
            this.mnuFileNew.Click += new System.EventHandler(this.mnuFileNew_Click);
            // 
            // mnuFileOpen
            // 
            this.mnuFileOpen.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileOpen.Image")));
            this.mnuFileOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuFileOpen.Name = "mnuFileOpen";
            this.mnuFileOpen.ShortcutKeyDisplayString = "Ctrl+O";
            this.mnuFileOpen.Size = new System.Drawing.Size(185, 22);
            this.mnuFileOpen.Text = "&Open";
            this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpen_Click);
            // 
            // mnuFileOpenRecent
            // 
            this.mnuFileOpenRecent.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            mnuFileOpenRecentSeparator,
            this.mnuFileOpenRecentClearRecent});
            this.mnuFileOpenRecent.Name = "mnuFileOpenRecent";
            this.mnuFileOpenRecent.Size = new System.Drawing.Size(185, 22);
            this.mnuFileOpenRecent.Text = "Open &Recent";
            this.mnuFileOpenRecent.DropDownOpening += new System.EventHandler(this.mnuFileOpenRecent_DropDownOpening);
            // 
            // mnuFileImport
            // 
            this.mnuFileImport.Name = "mnuFileImport";
            this.mnuFileImport.ShortcutKeyDisplayString = "Ctrl+Alt+O";
            this.mnuFileImport.Size = new System.Drawing.Size(185, 22);
            this.mnuFileImport.Text = "I&mport...";
            this.mnuFileImport.Click += new System.EventHandler(this.mnuFileImport_Click);
            // 
            // mnuFileSeparator1
            // 
            this.mnuFileSeparator1.Name = "mnuFileSeparator1";
            this.mnuFileSeparator1.Size = new System.Drawing.Size(182, 6);
            // 
            // mnuFileSave
            // 
            this.mnuFileSave.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileSave.Image")));
            this.mnuFileSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuFileSave.Name = "mnuFileSave";
            this.mnuFileSave.ShortcutKeyDisplayString = "Ctrl+S";
            this.mnuFileSave.Size = new System.Drawing.Size(185, 22);
            this.mnuFileSave.Text = "&Save";
            this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
            // 
            // mnuFileSaveAs
            // 
            this.mnuFileSaveAs.Name = "mnuFileSaveAs";
            this.mnuFileSaveAs.Size = new System.Drawing.Size(185, 22);
            this.mnuFileSaveAs.Text = "Save &As";
            this.mnuFileSaveAs.Click += new System.EventHandler(this.mnuFileSaveAs_Click);
            // 
            // mnuFileExport
            // 
            this.mnuFileExport.Name = "mnuFileExport";
            this.mnuFileExport.ShortcutKeyDisplayString = "Ctrl+Alt+S";
            this.mnuFileExport.Size = new System.Drawing.Size(185, 22);
            this.mnuFileExport.Text = "E&xport...";
            this.mnuFileExport.Click += new System.EventHandler(this.mnuFileExport_Click);
            // 
            // mnuFileSeparator2
            // 
            this.mnuFileSeparator2.Name = "mnuFileSeparator2";
            this.mnuFileSeparator2.Size = new System.Drawing.Size(182, 6);
            // 
            // mnuFilePrint
            // 
            this.mnuFilePrint.Image = ((System.Drawing.Image)(resources.GetObject("mnuFilePrint.Image")));
            this.mnuFilePrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuFilePrint.Name = "mnuFilePrint";
            this.mnuFilePrint.ShortcutKeyDisplayString = "Ctrl+P";
            this.mnuFilePrint.Size = new System.Drawing.Size(185, 22);
            this.mnuFilePrint.Text = "&Print";
            this.mnuFilePrint.Click += new System.EventHandler(this.mnuFilePrint_Click);
            // 
            // mnuFilePrintPreview
            // 
            this.mnuFilePrintPreview.Image = ((System.Drawing.Image)(resources.GetObject("mnuFilePrintPreview.Image")));
            this.mnuFilePrintPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuFilePrintPreview.Name = "mnuFilePrintPreview";
            this.mnuFilePrintPreview.Size = new System.Drawing.Size(185, 22);
            this.mnuFilePrintPreview.Text = "Print Pre&view";
            this.mnuFilePrintPreview.Click += new System.EventHandler(this.mnuFilePrintPreview_Click);
            // 
            // mnuFileSeparator3
            // 
            this.mnuFileSeparator3.Name = "mnuFileSeparator3";
            this.mnuFileSeparator3.Size = new System.Drawing.Size(182, 6);
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.ShortcutKeyDisplayString = "Alt+F4";
            this.mnuFileExit.Size = new System.Drawing.Size(185, 22);
            this.mnuFileExit.Text = "E&xit";
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
            // 
            // mnuEdit
            // 
            this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEditUndo,
            this.mnuEditRedo,
            this.mnuEditSeparator1,
            this.mnuEditCut,
            this.mnuEditCopy,
            this.mnuEditPaste,
            this.mnuEditSeparator2,
            this.mnuEditSelectAll,
            this.mnuEditDeselectAll,
            this.mnuEditToggleSelectAll,
            this.mnuEditSeparator3,
            this.mnuEditErase,
            this.mnuEditDelete});
            this.mnuEdit.Name = "mnuEdit";
            this.mnuEdit.Size = new System.Drawing.Size(39, 20);
            this.mnuEdit.Text = "&Edit";
            // 
            // mnuEditUndo
            // 
            this.mnuEditUndo.Name = "mnuEditUndo";
            this.mnuEditUndo.ShortcutKeyDisplayString = "Ctrl+Z";
            this.mnuEditUndo.Size = new System.Drawing.Size(209, 22);
            this.mnuEditUndo.Text = "&Undo";
            this.mnuEditUndo.Click += new System.EventHandler(this.mnuEditUndo_Click);
            // 
            // mnuEditRedo
            // 
            this.mnuEditRedo.Name = "mnuEditRedo";
            this.mnuEditRedo.ShortcutKeyDisplayString = "Ctrl+Y";
            this.mnuEditRedo.Size = new System.Drawing.Size(209, 22);
            this.mnuEditRedo.Text = "&Redo";
            this.mnuEditRedo.Click += new System.EventHandler(this.mnuEditRedo_Click);
            // 
            // mnuEditSeparator1
            // 
            this.mnuEditSeparator1.Name = "mnuEditSeparator1";
            this.mnuEditSeparator1.Size = new System.Drawing.Size(206, 6);
            // 
            // mnuEditCut
            // 
            this.mnuEditCut.Image = ((System.Drawing.Image)(resources.GetObject("mnuEditCut.Image")));
            this.mnuEditCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuEditCut.Name = "mnuEditCut";
            this.mnuEditCut.ShortcutKeyDisplayString = "Ctrl+X";
            this.mnuEditCut.Size = new System.Drawing.Size(209, 22);
            this.mnuEditCut.Text = "Cu&t";
            this.mnuEditCut.Click += new System.EventHandler(this.mnuEditCut_Click);
            // 
            // mnuEditCopy
            // 
            this.mnuEditCopy.Image = ((System.Drawing.Image)(resources.GetObject("mnuEditCopy.Image")));
            this.mnuEditCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuEditCopy.Name = "mnuEditCopy";
            this.mnuEditCopy.ShortcutKeyDisplayString = "Ctrl+C";
            this.mnuEditCopy.Size = new System.Drawing.Size(209, 22);
            this.mnuEditCopy.Text = "&Copy";
            this.mnuEditCopy.Click += new System.EventHandler(this.mnuEditCopy_Click);
            // 
            // mnuEditPaste
            // 
            this.mnuEditPaste.Image = ((System.Drawing.Image)(resources.GetObject("mnuEditPaste.Image")));
            this.mnuEditPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuEditPaste.Name = "mnuEditPaste";
            this.mnuEditPaste.ShortcutKeyDisplayString = "Ctrl+V";
            this.mnuEditPaste.Size = new System.Drawing.Size(209, 22);
            this.mnuEditPaste.Text = "&Paste";
            this.mnuEditPaste.Click += new System.EventHandler(this.mnuEditPaste_Click);
            // 
            // mnuEditSeparator2
            // 
            this.mnuEditSeparator2.Name = "mnuEditSeparator2";
            this.mnuEditSeparator2.Size = new System.Drawing.Size(206, 6);
            // 
            // mnuEditSelectAll
            // 
            this.mnuEditSelectAll.Name = "mnuEditSelectAll";
            this.mnuEditSelectAll.ShortcutKeyDisplayString = "Ctrl+A";
            this.mnuEditSelectAll.Size = new System.Drawing.Size(209, 22);
            this.mnuEditSelectAll.Text = "Select &All";
            this.mnuEditSelectAll.Click += new System.EventHandler(this.mnuEditSelectAll_Click);
            // 
            // mnuEditDeselectAll
            // 
            this.mnuEditDeselectAll.Name = "mnuEditDeselectAll";
            this.mnuEditDeselectAll.ShortcutKeyDisplayString = "Ctrl+Shift+A";
            this.mnuEditDeselectAll.Size = new System.Drawing.Size(209, 22);
            this.mnuEditDeselectAll.Text = "&Deselect All";
            this.mnuEditDeselectAll.Click += new System.EventHandler(this.mnuEditDeselectAll_Click);
            // 
            // mnuEditToggleSelectAll
            // 
            this.mnuEditToggleSelectAll.Name = "mnuEditToggleSelectAll";
            this.mnuEditToggleSelectAll.ShortcutKeyDisplayString = "A";
            this.mnuEditToggleSelectAll.Size = new System.Drawing.Size(209, 22);
            this.mnuEditToggleSelectAll.Text = "&Toggle Select All";
            this.mnuEditToggleSelectAll.Click += new System.EventHandler(this.mnuEditToggleSelectAll_Click);
            // 
            // mnuEditSeparator3
            // 
            this.mnuEditSeparator3.Name = "mnuEditSeparator3";
            this.mnuEditSeparator3.Size = new System.Drawing.Size(206, 6);
            // 
            // mnuEditErase
            // 
            this.mnuEditErase.Name = "mnuEditErase";
            this.mnuEditErase.ShortcutKeyDisplayString = "Shift+Del";
            this.mnuEditErase.Size = new System.Drawing.Size(209, 22);
            this.mnuEditErase.Text = "Era&se";
            this.mnuEditErase.Click += new System.EventHandler(this.mnuEditErase_Click);
            // 
            // mnuEditDelete
            // 
            this.mnuEditDelete.Name = "mnuEditDelete";
            this.mnuEditDelete.ShortcutKeyDisplayString = "Del";
            this.mnuEditDelete.Size = new System.Drawing.Size(209, 22);
            this.mnuEditDelete.Text = "D&elete";
            this.mnuEditDelete.Click += new System.EventHandler(this.mnuEditDelete_Click);
            // 
            // mnuView
            // 
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewCursorDouble,
            this.mnuViewCursorHalf,
            this.mnuViewCursor3rd,
            this.mnuViewCursor5th,
            this.mnuViewCursor7th,
            this.mnuViewSeparator1,
            this.mnuViewCursorResetToOneNote});
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(44, 20);
            this.mnuView.Text = "&View";
            // 
            // mnuViewCursorDouble
            // 
            this.mnuViewCursorDouble.Name = "mnuViewCursorDouble";
            this.mnuViewCursorDouble.ShortcutKeyDisplayString = "+";
            this.mnuViewCursorDouble.Size = new System.Drawing.Size(171, 22);
            this.mnuViewCursorDouble.Text = "&Double Cursor";
            this.mnuViewCursorDouble.Click += new System.EventHandler(this.mnuViewCursorDouble_Click);
            // 
            // mnuViewCursorHalf
            // 
            this.mnuViewCursorHalf.Name = "mnuViewCursorHalf";
            this.mnuViewCursorHalf.ShortcutKeyDisplayString = "-";
            this.mnuViewCursorHalf.Size = new System.Drawing.Size(171, 22);
            this.mnuViewCursorHalf.Text = "&Half Cursor";
            this.mnuViewCursorHalf.Click += new System.EventHandler(this.mnuViewCursorHalf_Click);
            // 
            // mnuViewCursor3rd
            // 
            this.mnuViewCursor3rd.Name = "mnuViewCursor3rd";
            this.mnuViewCursor3rd.ShortcutKeyDisplayString = "3";
            this.mnuViewCursor3rd.Size = new System.Drawing.Size(171, 22);
            this.mnuViewCursor3rd.Text = "&3rd Cursor";
            this.mnuViewCursor3rd.Click += new System.EventHandler(this.mnuViewCursor3rd_Click);
            // 
            // mnuViewCursor5th
            // 
            this.mnuViewCursor5th.Name = "mnuViewCursor5th";
            this.mnuViewCursor5th.ShortcutKeyDisplayString = "5";
            this.mnuViewCursor5th.Size = new System.Drawing.Size(171, 22);
            this.mnuViewCursor5th.Text = "&5th Cursor";
            this.mnuViewCursor5th.Click += new System.EventHandler(this.mnuViewCursor5th_Click);
            // 
            // mnuViewCursor7th
            // 
            this.mnuViewCursor7th.Name = "mnuViewCursor7th";
            this.mnuViewCursor7th.ShortcutKeyDisplayString = "7";
            this.mnuViewCursor7th.Size = new System.Drawing.Size(171, 22);
            this.mnuViewCursor7th.Text = "&7th Cursor";
            this.mnuViewCursor7th.Click += new System.EventHandler(this.mnuViewCursor7th_Click);
            // 
            // mnuViewSeparator1
            // 
            this.mnuViewSeparator1.Name = "mnuViewSeparator1";
            this.mnuViewSeparator1.Size = new System.Drawing.Size(168, 6);
            // 
            // mnuViewCursorResetToOneNote
            // 
            this.mnuViewCursorResetToOneNote.Name = "mnuViewCursorResetToOneNote";
            this.mnuViewCursorResetToOneNote.ShortcutKeyDisplayString = "1";
            this.mnuViewCursorResetToOneNote.Size = new System.Drawing.Size(171, 22);
            this.mnuViewCursorResetToOneNote.Text = "Cursor = &1 Note";
            this.mnuViewCursorResetToOneNote.Click += new System.EventHandler(this.mnuViewCursorResetToOneNote_Click);
            // 
            // mnuScreen
            // 
            this.mnuScreen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuScreenNew,
            this.mnuScreenArchive});
            this.mnuScreen.Name = "mnuScreen";
            this.mnuScreen.Size = new System.Drawing.Size(54, 20);
            this.mnuScreen.Text = "&Screen";
            // 
            // mnuScreenNew
            // 
            this.mnuScreenNew.Name = "mnuScreenNew";
            this.mnuScreenNew.ShortcutKeyDisplayString = "Ctrl+T";
            this.mnuScreenNew.Size = new System.Drawing.Size(139, 22);
            this.mnuScreenNew.Text = "&New";
            this.mnuScreenNew.Click += new System.EventHandler(this.mnuScreenNew_Click);
            // 
            // mnuScreenArchive
            // 
            this.mnuScreenArchive.Name = "mnuScreenArchive";
            this.mnuScreenArchive.Size = new System.Drawing.Size(139, 22);
            this.mnuScreenArchive.Text = "A&rchive...";
            this.mnuScreenArchive.Click += new System.EventHandler(this.mnuScreenArchive_Click);
            // 
            // mnuTools
            // 
            this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuToolsCustomize,
            this.mnuToolsOptions});
            this.mnuTools.Name = "mnuTools";
            this.mnuTools.Size = new System.Drawing.Size(47, 20);
            this.mnuTools.Text = "&Tools";
            // 
            // mnuToolsCustomize
            // 
            this.mnuToolsCustomize.Name = "mnuToolsCustomize";
            this.mnuToolsCustomize.Size = new System.Drawing.Size(130, 22);
            this.mnuToolsCustomize.Text = "&Customize";
            this.mnuToolsCustomize.Click += new System.EventHandler(this.mnuToolsCustomize_Click);
            // 
            // mnuToolsOptions
            // 
            this.mnuToolsOptions.Name = "mnuToolsOptions";
            this.mnuToolsOptions.Size = new System.Drawing.Size(130, 22);
            this.mnuToolsOptions.Text = "&Options";
            this.mnuToolsOptions.Click += new System.EventHandler(this.mnuToolsOptions_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpContents,
            this.mnuHelpIndex,
            this.mnuHelpSearch,
            this.mnuHelpSeparator1,
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(44, 20);
            this.mnuHelp.Text = "&Help";
            // 
            // mnuHelpContents
            // 
            this.mnuHelpContents.Name = "mnuHelpContents";
            this.mnuHelpContents.Size = new System.Drawing.Size(122, 22);
            this.mnuHelpContents.Text = "&Contents";
            this.mnuHelpContents.Click += new System.EventHandler(this.mnuHelpContents_Click);
            // 
            // mnuHelpIndex
            // 
            this.mnuHelpIndex.Name = "mnuHelpIndex";
            this.mnuHelpIndex.Size = new System.Drawing.Size(122, 22);
            this.mnuHelpIndex.Text = "&Index";
            this.mnuHelpIndex.Click += new System.EventHandler(this.mnuHelpIndex_Click);
            // 
            // mnuHelpSearch
            // 
            this.mnuHelpSearch.Name = "mnuHelpSearch";
            this.mnuHelpSearch.Size = new System.Drawing.Size(122, 22);
            this.mnuHelpSearch.Text = "&Search";
            this.mnuHelpSearch.Click += new System.EventHandler(this.mnuHelpSearch_Click);
            // 
            // mnuHelpSeparator1
            // 
            this.mnuHelpSeparator1.Name = "mnuHelpSeparator1";
            this.mnuHelpSeparator1.Size = new System.Drawing.Size(119, 6);
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(122, 22);
            this.mnuHelpAbout.Text = "&About...";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // tabScreens
            // 
            this.tabScreens.ContextMenuStrip = this.mnuHeader;
            this.tabScreens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabScreens.Location = new System.Drawing.Point(0, 24);
            this.tabScreens.Name = "tabScreens";
            this.tabScreens.SelectedIndex = 0;
            this.tabScreens.Size = new System.Drawing.Size(743, 368);
            this.tabScreens.TabIndex = 1;
            this.tabScreens.SelectedIndexChanged += new System.EventHandler(this.tabScreens_SelectedIndexChanged);
            // 
            // mnuHeader
            // 
            this.mnuHeader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHeaderRenameBox,
            this.toolStripSeparator1,
            this.mnuHeaderClose});
            this.mnuHeader.Name = "mnuHeader";
            this.mnuHeader.Size = new System.Drawing.Size(161, 57);
            this.mnuHeader.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.mnuHeader_Closing);
            this.mnuHeader.Opening += new System.ComponentModel.CancelEventHandler(this.mnuHeader_Opening);
            // 
            // mnuHeaderRenameBox
            // 
            this.mnuHeaderRenameBox.Name = "mnuHeaderRenameBox";
            this.mnuHeaderRenameBox.Size = new System.Drawing.Size(100, 23);
            this.mnuHeaderRenameBox.TextChanged += new System.EventHandler(this.mnuHeaderRenameBox_TextChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // mnuHeaderClose
            // 
            this.mnuHeaderClose.Name = "mnuHeaderClose";
            this.mnuHeaderClose.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this.mnuHeaderClose.Size = new System.Drawing.Size(160, 22);
            this.mnuHeaderClose.Text = "Close";
            this.mnuHeaderClose.Click += new System.EventHandler(this.mnuHeaderClose_Click);
            // 
            // diagSaveFile
            // 
            this.diagSaveFile.Filter = "Music Writer Files (*.musicwriter)|*.musicwriter";
            this.diagSaveFile.FilterIndex = 0;
            this.diagSaveFile.Title = "Save File";
            this.diagSaveFile.FileOk += new System.ComponentModel.CancelEventHandler(this.diagSaveFile_FileOk);
            // 
            // diagOpenFile
            // 
            this.diagOpenFile.Filter = "Music Writer Files (*.musicwriter)|*.musicwriter|Directory (*.musicwriter)|*.musi" +
    "cwriter";
            this.diagOpenFile.FilterIndex = 0;
            this.diagOpenFile.Title = "Open File";
            this.diagOpenFile.FileOk += new System.ComponentModel.CancelEventHandler(this.diagOpenFile_FileOk);
            // 
            // diagOpenImportFile
            // 
            this.diagOpenImportFile.FilterIndex = 0;
            this.diagOpenImportFile.FileOk += new System.ComponentModel.CancelEventHandler(this.diagOpenImportFile_FileOk);
            // 
            // diagSaveExportFile
            // 
            this.diagSaveExportFile.FilterIndex = 0;
            this.diagSaveExportFile.FileOk += new System.ComponentModel.CancelEventHandler(this.diagSaveExportFile_FileOk);
            // 
            // mnuFileOpenRecentSeparator
            // 
            mnuFileOpenRecentSeparator.Name = "mnuFileOpenRecentSeparator";
            mnuFileOpenRecentSeparator.Size = new System.Drawing.Size(149, 6);
            // 
            // mnuFileOpenRecentClearRecent
            // 
            this.mnuFileOpenRecentClearRecent.Name = "mnuFileOpenRecentClearRecent";
            this.mnuFileOpenRecentClearRecent.Size = new System.Drawing.Size(152, 22);
            this.mnuFileOpenRecentClearRecent.Text = "&Clear Recents";
            this.mnuFileOpenRecentClearRecent.Click += new System.EventHandler(this.mnuFileOpenRecentClearRecent_Click);
            // 
            // FileEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 392);
            this.Controls.Add(this.tabScreens);
            this.Controls.Add(this.mnuMainMenu);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.mnuMainMenu;
            this.Name = "FileEditorForm";
            this.Text = "Music Writer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.FileEditorForm_PreviewKeyDown);
            this.mnuMainMenu.ResumeLayout(false);
            this.mnuMainMenu.PerformLayout();
            this.mnuHeader.ResumeLayout(false);
            this.mnuHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuMainMenu;
        private System.Windows.Forms.TabControl tabScreens;
        private System.Windows.Forms.ContextMenuStrip mnuHeader;
        private System.Windows.Forms.ToolStripTextBox mnuHeaderRenameBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuHeaderClose;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileNew;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpenRecent;
        private System.Windows.Forms.ToolStripMenuItem mnuFileImport;
        private System.Windows.Forms.ToolStripSeparator mnuFileSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSave;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSaveAs;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExport;
        private System.Windows.Forms.ToolStripSeparator mnuFileSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuFilePrint;
        private System.Windows.Forms.ToolStripMenuItem mnuFilePrintPreview;
        private System.Windows.Forms.ToolStripSeparator mnuFileSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.ToolStripMenuItem mnuEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuEditUndo;
        private System.Windows.Forms.ToolStripMenuItem mnuEditRedo;
        private System.Windows.Forms.ToolStripSeparator mnuEditSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuEditCut;
        private System.Windows.Forms.ToolStripMenuItem mnuEditCopy;
        private System.Windows.Forms.ToolStripMenuItem mnuEditPaste;
        private System.Windows.Forms.ToolStripSeparator mnuEditSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuEditSelectAll;
        private System.Windows.Forms.ToolStripMenuItem mnuEditDeselectAll;
        private System.Windows.Forms.ToolStripMenuItem mnuEditToggleSelectAll;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuViewCursorDouble;
        private System.Windows.Forms.ToolStripMenuItem mnuViewCursorHalf;
        private System.Windows.Forms.ToolStripMenuItem mnuViewCursor3rd;
        private System.Windows.Forms.ToolStripMenuItem mnuViewCursor5th;
        private System.Windows.Forms.ToolStripMenuItem mnuViewCursor7th;
        private System.Windows.Forms.ToolStripMenuItem mnuViewCursorResetToOneNote;
        private System.Windows.Forms.ToolStripMenuItem mnuTools;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsCustomize;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsOptions;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpContents;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpIndex;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpSearch;
        private System.Windows.Forms.ToolStripSeparator mnuHelpSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ToolStripSeparator mnuViewSeparator1;
        private System.Windows.Forms.ToolStripSeparator mnuEditSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mnuEditErase;
        private System.Windows.Forms.ToolStripMenuItem mnuEditDelete;
        private System.Windows.Forms.SaveFileDialog diagSaveFile;
        private System.Windows.Forms.OpenFileDialog diagOpenFile;
        private System.Windows.Forms.ToolStripMenuItem mnuScreen;
        private System.Windows.Forms.ToolStripMenuItem mnuScreenNew;
        private System.Windows.Forms.ToolStripMenuItem mnuScreenArchive;
		private System.Windows.Forms.OpenFileDialog diagOpenImportFile;
		private System.Windows.Forms.SaveFileDialog diagSaveExportFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpenRecentClearRecent;
    }
}

