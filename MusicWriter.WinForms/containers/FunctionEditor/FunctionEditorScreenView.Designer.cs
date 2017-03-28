namespace MusicWriter.WinForms
{
    partial class FunctionEditorScreenView
    {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripSeparator mnuPlaySeparator1;
            System.Windows.Forms.Panel panel1;
            System.Windows.Forms.Panel panel2;
            System.Windows.Forms.ToolStripSeparator mnuFunctionSourcesSeparator1;
            this.txtSearchFunctionSources = new System.Windows.Forms.TextBox();
            this.btnDeleteFunctionSource = new System.Windows.Forms.Button();
            this.btnAddFunctionSource = new System.Windows.Forms.Button();
            this.txtPolylines = new System.Windows.Forms.TextBox();
            this.btnDeletePolyline = new System.Windows.Forms.Button();
            this.btnAddPolyline = new System.Windows.Forms.Button();
            this.mnuPlay = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuPlayTone = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPlayLength = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPlayTempo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPlayVerb = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPlayRepeat = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPlayPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPlayStop = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lsvFunctionSources = new System.Windows.Forms.ListView();
            this.clmFunctionSourceName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mnuFunctionSources = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuFunctionSourcesEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFunctionSourcesCreate = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFunctionSourcesDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.lsvPolylines = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabFunctionSources = new System.Windows.Forms.TabControl();
            mnuPlaySeparator1 = new System.Windows.Forms.ToolStripSeparator();
            panel1 = new System.Windows.Forms.Panel();
            panel2 = new System.Windows.Forms.Panel();
            mnuFunctionSourcesSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            this.mnuPlay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.mnuFunctionSources.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuPlaySeparator1
            // 
            mnuPlaySeparator1.Name = "mnuPlaySeparator1";
            mnuPlaySeparator1.Size = new System.Drawing.Size(108, 6);
            // 
            // panel1
            // 
            panel1.Controls.Add(this.txtSearchFunctionSources);
            panel1.Controls.Add(this.btnDeleteFunctionSource);
            panel1.Controls.Add(this.btnAddFunctionSource);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(200, 22);
            panel1.TabIndex = 2;
            // 
            // txtSearchFunctionSources
            // 
            this.txtSearchFunctionSources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearchFunctionSources.Location = new System.Drawing.Point(0, 0);
            this.txtSearchFunctionSources.Name = "txtSearchFunctionSources";
            this.txtSearchFunctionSources.Size = new System.Drawing.Size(154, 20);
            this.txtSearchFunctionSources.TabIndex = 3;
            this.txtSearchFunctionSources.Text = "Function Sources";
            // 
            // btnDeleteFunctionSource
            // 
            this.btnDeleteFunctionSource.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnDeleteFunctionSource.Location = new System.Drawing.Point(154, 0);
            this.btnDeleteFunctionSource.Name = "btnDeleteFunctionSource";
            this.btnDeleteFunctionSource.Size = new System.Drawing.Size(23, 22);
            this.btnDeleteFunctionSource.TabIndex = 2;
            this.btnDeleteFunctionSource.Text = "-";
            this.btnDeleteFunctionSource.UseVisualStyleBackColor = true;
            this.btnDeleteFunctionSource.Click += new System.EventHandler(this.btnDeleteFunctionSource_Click);
            // 
            // btnAddFunctionSource
            // 
            this.btnAddFunctionSource.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAddFunctionSource.Location = new System.Drawing.Point(177, 0);
            this.btnAddFunctionSource.Name = "btnAddFunctionSource";
            this.btnAddFunctionSource.Size = new System.Drawing.Size(23, 22);
            this.btnAddFunctionSource.TabIndex = 1;
            this.btnAddFunctionSource.Text = "+";
            this.btnAddFunctionSource.UseVisualStyleBackColor = true;
            this.btnAddFunctionSource.Click += new System.EventHandler(this.btnAddFunctionSource_Click);
            // 
            // panel2
            // 
            panel2.Controls.Add(this.txtPolylines);
            panel2.Controls.Add(this.btnDeletePolyline);
            panel2.Controls.Add(this.btnAddPolyline);
            panel2.Dock = System.Windows.Forms.DockStyle.Top;
            panel2.Location = new System.Drawing.Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(200, 22);
            panel2.TabIndex = 4;
            // 
            // txtPolylines
            // 
            this.txtPolylines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPolylines.Location = new System.Drawing.Point(0, 0);
            this.txtPolylines.Name = "txtPolylines";
            this.txtPolylines.Size = new System.Drawing.Size(154, 20);
            this.txtPolylines.TabIndex = 3;
            this.txtPolylines.Text = "Polylines";
            // 
            // btnDeletePolyline
            // 
            this.btnDeletePolyline.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnDeletePolyline.Location = new System.Drawing.Point(154, 0);
            this.btnDeletePolyline.Name = "btnDeletePolyline";
            this.btnDeletePolyline.Size = new System.Drawing.Size(23, 22);
            this.btnDeletePolyline.TabIndex = 2;
            this.btnDeletePolyline.Text = "-";
            this.btnDeletePolyline.UseVisualStyleBackColor = true;
            this.btnDeletePolyline.Click += new System.EventHandler(this.btnDeletePolyline_Click);
            // 
            // btnAddPolyline
            // 
            this.btnAddPolyline.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAddPolyline.Location = new System.Drawing.Point(177, 0);
            this.btnAddPolyline.Name = "btnAddPolyline";
            this.btnAddPolyline.Size = new System.Drawing.Size(23, 22);
            this.btnAddPolyline.TabIndex = 1;
            this.btnAddPolyline.Text = "+";
            this.btnAddPolyline.UseVisualStyleBackColor = true;
            this.btnAddPolyline.Click += new System.EventHandler(this.btnAddPolyline_Click);
            // 
            // mnuFunctionSourcesSeparator1
            // 
            mnuFunctionSourcesSeparator1.Name = "mnuFunctionSourcesSeparator1";
            mnuFunctionSourcesSeparator1.Size = new System.Drawing.Size(113, 6);
            // 
            // mnuPlay
            // 
            this.mnuPlay.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuPlayTone,
            this.mnuPlayLength,
            this.mnuPlayTempo,
            this.mnuPlayVerb,
            this.mnuPlayRepeat,
            mnuPlaySeparator1,
            this.mnuPlayPlay,
            this.mnuPlayStop});
            this.mnuPlay.Name = "mnuPlay";
            this.mnuPlay.Size = new System.Drawing.Size(112, 164);
            // 
            // mnuPlayTone
            // 
            this.mnuPlayTone.Name = "mnuPlayTone";
            this.mnuPlayTone.Size = new System.Drawing.Size(111, 22);
            this.mnuPlayTone.Text = "&Tone";
            // 
            // mnuPlayLength
            // 
            this.mnuPlayLength.Name = "mnuPlayLength";
            this.mnuPlayLength.Size = new System.Drawing.Size(111, 22);
            this.mnuPlayLength.Text = "&Length";
            // 
            // mnuPlayTempo
            // 
            this.mnuPlayTempo.Name = "mnuPlayTempo";
            this.mnuPlayTempo.Size = new System.Drawing.Size(111, 22);
            this.mnuPlayTempo.Text = "Te&mpo";
            // 
            // mnuPlayVerb
            // 
            this.mnuPlayVerb.Name = "mnuPlayVerb";
            this.mnuPlayVerb.Size = new System.Drawing.Size(111, 22);
            this.mnuPlayVerb.Text = "&Verb";
            // 
            // mnuPlayRepeat
            // 
            this.mnuPlayRepeat.Name = "mnuPlayRepeat";
            this.mnuPlayRepeat.Size = new System.Drawing.Size(111, 22);
            this.mnuPlayRepeat.Text = "&Repeat";
            // 
            // mnuPlayPlay
            // 
            this.mnuPlayPlay.Name = "mnuPlayPlay";
            this.mnuPlayPlay.Size = new System.Drawing.Size(111, 22);
            this.mnuPlayPlay.Text = "&Play";
            // 
            // mnuPlayStop
            // 
            this.mnuPlayStop.Name = "mnuPlayStop";
            this.mnuPlayStop.Size = new System.Drawing.Size(111, 22);
            this.mnuPlayStop.Text = "S&top";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabFunctionSources);
            this.splitContainer1.Size = new System.Drawing.Size(600, 396);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.lsvFunctionSources);
            this.splitContainer2.Panel1.Controls.Add(panel1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lsvPolylines);
            this.splitContainer2.Panel2.Controls.Add(panel2);
            this.splitContainer2.Size = new System.Drawing.Size(200, 396);
            this.splitContainer2.SplitterDistance = 186;
            this.splitContainer2.TabIndex = 0;
            // 
            // lsvFunctionSources
            // 
            this.lsvFunctionSources.CheckBoxes = true;
            this.lsvFunctionSources.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmFunctionSourceName});
            this.lsvFunctionSources.ContextMenuStrip = this.mnuFunctionSources;
            this.lsvFunctionSources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvFunctionSources.HideSelection = false;
            this.lsvFunctionSources.Location = new System.Drawing.Point(0, 22);
            this.lsvFunctionSources.Name = "lsvFunctionSources";
            this.lsvFunctionSources.Size = new System.Drawing.Size(200, 164);
            this.lsvFunctionSources.TabIndex = 3;
            this.lsvFunctionSources.UseCompatibleStateImageBehavior = false;
            this.lsvFunctionSources.View = System.Windows.Forms.View.Details;
            this.lsvFunctionSources.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lsvFunctionSources_ItemChecked);
            this.lsvFunctionSources.SelectedIndexChanged += new System.EventHandler(this.lsvFunctionSources_SelectedIndexChanged);
            // 
            // clmFunctionSourceName
            // 
            this.clmFunctionSourceName.Text = "Name";
            this.clmFunctionSourceName.Width = 192;
            // 
            // mnuFunctionSources
            // 
            this.mnuFunctionSources.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFunctionSourcesEnabled,
            mnuFunctionSourcesSeparator1,
            this.mnuFunctionSourcesCreate,
            this.mnuFunctionSourcesDelete});
            this.mnuFunctionSources.Name = "mnuFunctionSources";
            this.mnuFunctionSources.Size = new System.Drawing.Size(117, 76);
            this.mnuFunctionSources.Opening += new System.ComponentModel.CancelEventHandler(this.mnuFunctionSources_Opening);
            // 
            // mnuFunctionSourcesEnabled
            // 
            this.mnuFunctionSourcesEnabled.CheckOnClick = true;
            this.mnuFunctionSourcesEnabled.Name = "mnuFunctionSourcesEnabled";
            this.mnuFunctionSourcesEnabled.Size = new System.Drawing.Size(116, 22);
            this.mnuFunctionSourcesEnabled.Text = "E&nabled";
            this.mnuFunctionSourcesEnabled.CheckedChanged += new System.EventHandler(this.mnuFunctionSourcesEnabled_CheckedChanged);
            // 
            // mnuFunctionSourcesCreate
            // 
            this.mnuFunctionSourcesCreate.Name = "mnuFunctionSourcesCreate";
            this.mnuFunctionSourcesCreate.Size = new System.Drawing.Size(116, 22);
            this.mnuFunctionSourcesCreate.Text = "Cre&ate";
            this.mnuFunctionSourcesCreate.Click += new System.EventHandler(this.mnuFunctionSourcesCreate_Click);
            // 
            // mnuFunctionSourcesDelete
            // 
            this.mnuFunctionSourcesDelete.Name = "mnuFunctionSourcesDelete";
            this.mnuFunctionSourcesDelete.Size = new System.Drawing.Size(116, 22);
            this.mnuFunctionSourcesDelete.Text = "&Delete";
            this.mnuFunctionSourcesDelete.Click += new System.EventHandler(this.mnuFunctionSourcesDelete_Click);
            // 
            // lsvPolylines
            // 
            this.lsvPolylines.CheckBoxes = true;
            this.lsvPolylines.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lsvPolylines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvPolylines.HideSelection = false;
            this.lsvPolylines.Location = new System.Drawing.Point(0, 22);
            this.lsvPolylines.Name = "lsvPolylines";
            this.lsvPolylines.Size = new System.Drawing.Size(200, 184);
            this.lsvPolylines.TabIndex = 5;
            this.lsvPolylines.UseCompatibleStateImageBehavior = false;
            this.lsvPolylines.View = System.Windows.Forms.View.Details;
            this.lsvPolylines.SelectedIndexChanged += new System.EventHandler(this.lsvPolylines_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 192;
            // 
            // tabFunctionSources
            // 
            this.tabFunctionSources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabFunctionSources.Location = new System.Drawing.Point(0, 0);
            this.tabFunctionSources.Name = "tabFunctionSources";
            this.tabFunctionSources.SelectedIndex = 0;
            this.tabFunctionSources.Size = new System.Drawing.Size(396, 396);
            this.tabFunctionSources.TabIndex = 2;
            // 
            // FunctionEditorScreenView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "FunctionEditorScreenView";
            this.Size = new System.Drawing.Size(600, 396);
            this.Load += new System.EventHandler(this.FunctionEditorScreenView_Load);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            this.mnuPlay.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.mnuFunctionSources.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip mnuPlay;
        private System.Windows.Forms.ToolStripMenuItem mnuPlayTone;
        private System.Windows.Forms.ToolStripMenuItem mnuPlayLength;
        private System.Windows.Forms.ToolStripMenuItem mnuPlayTempo;
        private System.Windows.Forms.ToolStripMenuItem mnuPlayVerb;
        private System.Windows.Forms.ToolStripMenuItem mnuPlayRepeat;
        private System.Windows.Forms.ToolStripMenuItem mnuPlayPlay;
        private System.Windows.Forms.ToolStripMenuItem mnuPlayStop;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TabControl tabFunctionSources;
        private System.Windows.Forms.TextBox txtSearchFunctionSources;
        private System.Windows.Forms.Button btnDeleteFunctionSource;
        private System.Windows.Forms.Button btnAddFunctionSource;
        private System.Windows.Forms.TextBox txtPolylines;
        private System.Windows.Forms.Button btnDeletePolyline;
        private System.Windows.Forms.Button btnAddPolyline;
        private System.Windows.Forms.ListView lsvFunctionSources;
        private System.Windows.Forms.ColumnHeader clmFunctionSourceName;
        private System.Windows.Forms.ListView lsvPolylines;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ContextMenuStrip mnuFunctionSources;
        private System.Windows.Forms.ToolStripMenuItem mnuFunctionSourcesEnabled;
        private System.Windows.Forms.ToolStripMenuItem mnuFunctionSourcesCreate;
        private System.Windows.Forms.ToolStripMenuItem mnuFunctionSourcesDelete;
    }
}
