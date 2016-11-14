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
            MusicWriter.WinForms.SheetMusicRenderSettings sheetMusicRenderSettings1 = new MusicWriter.WinForms.SheetMusicRenderSettings();
            this.Editor = new MusicWriter.WinForms.SheetMusicEditor();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Editor
            // 
            this.Editor.Brain = null;
            this.Editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Editor.Location = new System.Drawing.Point(0, 0);
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
            this.Editor.Size = new System.Drawing.Size(492, 392);
            this.Editor.TabIndex = 0;
            this.Editor.Track = null;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.Editor);
            this.splitContainer1.Size = new System.Drawing.Size(743, 392);
            this.splitContainer1.SplitterDistance = 247;
            this.splitContainer1.TabIndex = 1;
            // 
            // FileEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 392);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FileEditorForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SheetMusicEditor Editor;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}

