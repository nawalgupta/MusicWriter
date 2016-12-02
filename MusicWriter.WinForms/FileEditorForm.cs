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
    public partial class FileEditorForm : Form {
        EditorFile file = new EditorFile();
        FileCapabilities<Control> capabilities =
            new FileCapabilities<Control>();
        KeyboardInputSource input_keyboard =
            new KeyboardInputSource();
        InputController inputcontroller;
        KeyboardMenuShortcuts shortcutter =
            new KeyboardMenuShortcuts();
        CommandCenter commandcenter =
            new CommandCenter();

        public Screen<Control> ActiveScreen {
            get { return (tabScreens.SelectedTab as ScreenView)?.Screen; }
        }

        public FileEditorForm() {
            InitializeComponent();

            shortcutter.Menu = mnuMainMenu;
            inputcontroller = new InputController(commandcenter);
        }

        void InitInputSources() {
            input_keyboard.Controller = inputcontroller;
        }

        void InitControllerFactories() {
            capabilities.ControllerFactories.Add(new SheetMusicEditor.Factory());
        }

        void InitTrackFactories() {
            capabilities.TrackFactories.Add(new MusicTrackFactory());
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

            NewScreen();
        }

        void NewScreen() {
            var screen =
                new Screen<Control>(
                        capabilities,
                        file,
                        inputcontroller
                    );

            var editor =
                new SheetMusicEditor();

            editor.File = file;

            var track1 =
                (MusicTrack)capabilities.TrackFactories[0].Create();

            track1.Melody.AddNote(SemiTone.C5, new Duration { Start = Time.Zero, Length = Time.Note_4th + Time.Note_4th / 2 });
            track1.Melody.AddNote(SemiTone.C5, new Duration { Start = Time.Note_4th + Time.Note_4th / 2, Length = Time.Note_4th / 2 });
            track1.Melody.AddNote(SemiTone.C5, new Duration { Start = Time.Note_2nd, Length = Time.Note_4th });
            track1.Melody.AddNote(SemiTone.C5, new Duration { Start = Time.Note_2nd + Time.Note_4th, Length = Time.Note_4th });

            track1.Name.Value = "Track 1";
            editor.Tracks.Add(track1);
            file.Tracks.Add(track1);

            var track2 =
                (MusicTrack)capabilities.TrackFactories[0].Create();

            track2.Melody.AddNote(SemiTone.C5, new Duration { Start = Time.Zero, Length = Time.Note_2nd });
            track2.Melody.AddNote(SemiTone.C5, new Duration { Start = Time.Note_2nd, Length = Time.Note_2nd });

            track2.Name.Value = "Track 2";
            //editor.Tracks.Add(track2);
            file.Tracks.Add(track2);

            var track3 =
                (MusicTrack)capabilities.TrackFactories[0].Create();

            track3.Melody.AddNote(SemiTone.C4, new Duration { Start = Time.Note_8th * 0, Length = Time.Note_8th });
            track3.Melody.AddNote(SemiTone.C4, new Duration { Start = Time.Note_8th * 1, Length = Time.Note_8th });
            track3.Melody.AddNote(SemiTone.C4 + 2, new Duration { Start = Time.Note_8th * 2, Length = Time.Note_8th });
            track3.Melody.AddNote(SemiTone.C4, new Duration { Start = Time.Note_8th * 3, Length = Time.Note_8th });
            track3.Melody.AddNote(SemiTone.C4 + 4, new Duration { Start = Time.Note_8th * 4, Length = Time.Note_8th });
            track3.Melody.AddNote(SemiTone.C4 + 2, new Duration { Start = Time.Note_8th * 5, Length = Time.Note_8th });
            track3.Melody.AddNote(SemiTone.C4 + 5, new Duration { Start = Time.Note_8th * 6, Length = Time.Note_8th });
            track3.Melody.AddNote(SemiTone.C4 + 16, new Duration { Start = Time.Note_8th * 7, Length = Time.Note_8th });

            track3.Name.Value = "Track 3";
            //editor.Tracks.Add(track3);
            file.Tracks.Add(track3);

            screen.Controllers.Add(editor);
            screen.Name.Value = "Screen 1";
            
            LoadScreen(screen);
        }

        void LoadScreen(Screen<Control> screen) {
            ScreenView view = new ScreenView();
            view.Screen = screen;
            view.File = file;

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
                view.Screen.CommandCenter.Enabled = ReferenceEquals(view, tabScreens.SelectedTab);
            }
        }

        private void mnuFileNew_Click(object sender, EventArgs e) {

        }

        private void mnuFileOpen_Click(object sender, EventArgs e) {

        }

        private void mnuFileSave_Click(object sender, EventArgs e) {

        }

        private void mnuFileSaveAs_Click(object sender, EventArgs e) {

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
    }
}
