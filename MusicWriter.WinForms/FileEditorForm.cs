﻿using System;
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

        public Screen<Control> ActiveScreen {
            get { return (tabScreens.SelectedTab as ScreenView)?.Screen; }
        }

        public FileEditorForm() {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            capabilities.ControllerFactories.Add(new NewControllerFactory<SheetMusicEditor, Control> { Name = "Sheet Music Editor" });
            capabilities.TrackFactories.Add(new MusicTrackFactory());

            NewScreen();
        }

        void NewScreen() =>
            LoadScreen(
                    new Screen<Control>(
                            capabilities
                        )
                );

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
