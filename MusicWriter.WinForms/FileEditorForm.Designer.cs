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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.stubMenuItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabScreens = new System.Windows.Forms.TabControl();
            this.mnuHeader = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuHeaderRenameBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuHeaderClose = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.mnuHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stubMenuItemToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(743, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // stubMenuItemToolStripMenuItem
            // 
            this.stubMenuItemToolStripMenuItem.Name = "stubMenuItemToolStripMenuItem";
            this.stubMenuItemToolStripMenuItem.Size = new System.Drawing.Size(104, 20);
            this.stubMenuItemToolStripMenuItem.Text = "Stub Menu Item";
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
            // 
            // mnuHeader
            // 
            this.mnuHeader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHeaderRenameBox,
            this.toolStripSeparator1,
            this.mnuHeaderClose});
            this.mnuHeader.Name = "mnuHeader";
            this.mnuHeader.Size = new System.Drawing.Size(161, 79);
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
            // FileEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 392);
            this.Controls.Add(this.tabScreens);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FileEditorForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.mnuHeader.ResumeLayout(false);
            this.mnuHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem stubMenuItemToolStripMenuItem;
        private System.Windows.Forms.TabControl tabScreens;
        private System.Windows.Forms.ContextMenuStrip mnuHeader;
        private System.Windows.Forms.ToolStripTextBox mnuHeaderRenameBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuHeaderClose;
    }
}

