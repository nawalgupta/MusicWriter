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
        InputController inputcontroller =
            new InputController();

        public Screen<Control> ActiveScreen {
            get { return (tabScreens.SelectedTab as ScreenView)?.Screen; }
        }

        public FileEditorForm() {
            InitializeComponent();
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
                keys_pressed.Add(e.KeyCode);
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            input_keyboard.OnKeyUp(e);

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
            editor.Tracks.Add(track3);
            file.Tracks.Add(track3);

            screen.Controllers.Add(editor);
            screen.Name.Value = "Screen 1";
            
            LoadScreen(screen);
        }

        void LoadScreen(Screen<Control> screen) {
            ScreenView view = new ScreenView();
            view.Screen = screen;
            view.File = file;

            tabScreens.Controls.Add(view);
        }

        void CloseScreen(Screen<Control> screen) {
            tabScreens.Controls.RemoveByKey($"tabScreen_{screen.Name}");
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
    }
}
