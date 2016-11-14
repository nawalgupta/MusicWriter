namespace MusicWriter.WinForms {
    partial class MainForm {
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
            MusicWriter.WinForms.SheetMusicRenderSettings sheetMusicRenderSettings1 = new MusicWriter.WinForms.SheetMusicRenderSettings();
            this.Editor = new MusicWriter.WinForms.SheetMusicEditor();
            this.SuspendLayout();
            // 
            // Editor
            // 
            this.Editor.Brain = null;
            this.Editor.Location = new System.Drawing.Point(28, 46);
            this.Editor.Name = "Editor";
            this.Editor.ScrollX = 0F;
            sheetMusicRenderSettings1.MarginalBottomHalfLines = 3;
            sheetMusicRenderSettings1.MarginalTopHalfLines = 3;
            sheetMusicRenderSettings1.NoteHeadRadius = 4.5F;
            sheetMusicRenderSettings1.PixelsPerHalfLine = 10F;
            sheetMusicRenderSettings1.PixelsPerLine = 20F;
            sheetMusicRenderSettings1.PixelsPerX = 100F;
            sheetMusicRenderSettings1.PixelsScale = 1F;
            sheetMusicRenderSettings1.TimeSignatureFont = new System.Drawing.Font("Times New Roman", 18F);
            this.Editor.Settings = sheetMusicRenderSettings1;
            this.Editor.Size = new System.Drawing.Size(631, 218);
            this.Editor.TabIndex = 0;
            this.Editor.Track = null;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 376);
            this.Controls.Add(this.Editor);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private SheetMusicEditor Editor;
    }
}

