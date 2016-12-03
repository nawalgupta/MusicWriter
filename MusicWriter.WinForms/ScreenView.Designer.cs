namespace MusicWriter.WinForms {
    partial class ScreenView {
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
            System.Windows.Forms.Panel pnlViewTools;
            System.Windows.Forms.Panel panel1;
            this.txtSearchControllers = new System.Windows.Forms.TextBox();
            this.btnDeleteController = new System.Windows.Forms.Button();
            this.btnAddView = new System.Windows.Forms.Button();
            this.txtSearchTracks = new System.Windows.Forms.TextBox();
            this.btnDeleteTrack = new System.Windows.Forms.Button();
            this.btnAddTrack = new System.Windows.Forms.Button();
            this.spltMasterDetail = new System.Windows.Forms.SplitContainer();
            this.spltSidebar = new System.Windows.Forms.SplitContainer();
            this.lsvControllers = new System.Windows.Forms.ListView();
            this.clmViewName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lsvTracks = new System.Windows.Forms.ListView();
            this.clmTrackName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlViews = new System.Windows.Forms.Panel();
            this.mnuAddController = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuAddTrack = new System.Windows.Forms.ContextMenuStrip(this.components);
            pnlViewTools = new System.Windows.Forms.Panel();
            panel1 = new System.Windows.Forms.Panel();
            pnlViewTools.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltMasterDetail)).BeginInit();
            this.spltMasterDetail.Panel1.SuspendLayout();
            this.spltMasterDetail.Panel2.SuspendLayout();
            this.spltMasterDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltSidebar)).BeginInit();
            this.spltSidebar.Panel1.SuspendLayout();
            this.spltSidebar.Panel2.SuspendLayout();
            this.spltSidebar.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlViewTools
            // 
            pnlViewTools.Controls.Add(this.txtSearchControllers);
            pnlViewTools.Controls.Add(this.btnDeleteController);
            pnlViewTools.Controls.Add(this.btnAddView);
            pnlViewTools.Dock = System.Windows.Forms.DockStyle.Top;
            pnlViewTools.Location = new System.Drawing.Point(0, 0);
            pnlViewTools.Name = "pnlViewTools";
            pnlViewTools.Size = new System.Drawing.Size(211, 22);
            pnlViewTools.TabIndex = 0;
            // 
            // txtSearchControllers
            // 
            this.txtSearchControllers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearchControllers.Location = new System.Drawing.Point(0, 0);
            this.txtSearchControllers.Name = "txtSearchControllers";
            this.txtSearchControllers.Size = new System.Drawing.Size(165, 20);
            this.txtSearchControllers.TabIndex = 3;
            this.txtSearchControllers.Text = "Controllers";
            this.txtSearchControllers.TextChanged += new System.EventHandler(this.txtSearchControllers_TextChanged);
            // 
            // btnDeleteController
            // 
            this.btnDeleteController.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnDeleteController.Location = new System.Drawing.Point(165, 0);
            this.btnDeleteController.Name = "btnDeleteController";
            this.btnDeleteController.Size = new System.Drawing.Size(23, 22);
            this.btnDeleteController.TabIndex = 2;
            this.btnDeleteController.Text = "-";
            this.btnDeleteController.UseVisualStyleBackColor = true;
            this.btnDeleteController.Click += new System.EventHandler(this.btnDeleteController_Click);
            // 
            // btnAddView
            // 
            this.btnAddView.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAddView.Location = new System.Drawing.Point(188, 0);
            this.btnAddView.Name = "btnAddView";
            this.btnAddView.Size = new System.Drawing.Size(23, 22);
            this.btnAddView.TabIndex = 1;
            this.btnAddView.Text = "+";
            this.btnAddView.UseVisualStyleBackColor = true;
            this.btnAddView.Click += new System.EventHandler(this.btnAddController_Click);
            // 
            // panel1
            // 
            panel1.Controls.Add(this.txtSearchTracks);
            panel1.Controls.Add(this.btnDeleteTrack);
            panel1.Controls.Add(this.btnAddTrack);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(211, 22);
            panel1.TabIndex = 1;
            // 
            // txtSearchTracks
            // 
            this.txtSearchTracks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearchTracks.Location = new System.Drawing.Point(0, 0);
            this.txtSearchTracks.Name = "txtSearchTracks";
            this.txtSearchTracks.Size = new System.Drawing.Size(165, 20);
            this.txtSearchTracks.TabIndex = 3;
            this.txtSearchTracks.Text = "Tracks";
            this.txtSearchTracks.TextChanged += new System.EventHandler(this.txtSearchTracks_TextChanged);
            // 
            // btnDeleteTrack
            // 
            this.btnDeleteTrack.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnDeleteTrack.Location = new System.Drawing.Point(165, 0);
            this.btnDeleteTrack.Name = "btnDeleteTrack";
            this.btnDeleteTrack.Size = new System.Drawing.Size(23, 22);
            this.btnDeleteTrack.TabIndex = 2;
            this.btnDeleteTrack.Text = "-";
            this.btnDeleteTrack.UseVisualStyleBackColor = true;
            this.btnDeleteTrack.Click += new System.EventHandler(this.btnDeleteTrack_Click);
            // 
            // btnAddTrack
            // 
            this.btnAddTrack.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAddTrack.Location = new System.Drawing.Point(188, 0);
            this.btnAddTrack.Name = "btnAddTrack";
            this.btnAddTrack.Size = new System.Drawing.Size(23, 22);
            this.btnAddTrack.TabIndex = 1;
            this.btnAddTrack.Text = "+";
            this.btnAddTrack.UseVisualStyleBackColor = true;
            this.btnAddTrack.Click += new System.EventHandler(this.btnAddTrack_Click);
            // 
            // spltMasterDetail
            // 
            this.spltMasterDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltMasterDetail.Location = new System.Drawing.Point(0, 0);
            this.spltMasterDetail.Name = "spltMasterDetail";
            // 
            // spltMasterDetail.Panel1
            // 
            this.spltMasterDetail.Panel1.Controls.Add(this.spltSidebar);
            // 
            // spltMasterDetail.Panel2
            // 
            this.spltMasterDetail.Panel2.Controls.Add(this.pnlViews);
            this.spltMasterDetail.Size = new System.Drawing.Size(637, 381);
            this.spltMasterDetail.SplitterDistance = 211;
            this.spltMasterDetail.TabIndex = 3;
            this.spltMasterDetail.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.spltMasterDetail_SplitterMoved);
            // 
            // spltSidebar
            // 
            this.spltSidebar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltSidebar.Location = new System.Drawing.Point(0, 0);
            this.spltSidebar.Name = "spltSidebar";
            this.spltSidebar.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spltSidebar.Panel1
            // 
            this.spltSidebar.Panel1.Controls.Add(this.lsvControllers);
            this.spltSidebar.Panel1.Controls.Add(pnlViewTools);
            // 
            // spltSidebar.Panel2
            // 
            this.spltSidebar.Panel2.Controls.Add(this.lsvTracks);
            this.spltSidebar.Panel2.Controls.Add(panel1);
            this.spltSidebar.Size = new System.Drawing.Size(211, 381);
            this.spltSidebar.SplitterDistance = 190;
            this.spltSidebar.TabIndex = 2;
            // 
            // lsvControllers
            // 
            this.lsvControllers.CheckBoxes = true;
            this.lsvControllers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmViewName});
            this.lsvControllers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvControllers.FullRowSelect = true;
            this.lsvControllers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lsvControllers.HideSelection = false;
            this.lsvControllers.LabelEdit = true;
            this.lsvControllers.Location = new System.Drawing.Point(0, 22);
            this.lsvControllers.MultiSelect = false;
            this.lsvControllers.Name = "lsvControllers";
            this.lsvControllers.Size = new System.Drawing.Size(211, 168);
            this.lsvControllers.TabIndex = 1;
            this.lsvControllers.UseCompatibleStateImageBehavior = false;
            this.lsvControllers.View = System.Windows.Forms.View.Details;
            this.lsvControllers.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.lsvControllers_AfterLabelEdit);
            this.lsvControllers.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lsvControllers_ItemChecked);
            this.lsvControllers.SelectedIndexChanged += new System.EventHandler(this.lsvControllers_SelectedIndexChanged);
            // 
            // clmViewName
            // 
            this.clmViewName.Text = "Name";
            this.clmViewName.Width = 200;
            // 
            // lsvTracks
            // 
            this.lsvTracks.CheckBoxes = true;
            this.lsvTracks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmTrackName});
            this.lsvTracks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvTracks.FullRowSelect = true;
            this.lsvTracks.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lsvTracks.HideSelection = false;
            this.lsvTracks.LabelEdit = true;
            this.lsvTracks.Location = new System.Drawing.Point(0, 22);
            this.lsvTracks.MultiSelect = false;
            this.lsvTracks.Name = "lsvTracks";
            this.lsvTracks.Size = new System.Drawing.Size(211, 165);
            this.lsvTracks.TabIndex = 2;
            this.lsvTracks.UseCompatibleStateImageBehavior = false;
            this.lsvTracks.View = System.Windows.Forms.View.List;
            this.lsvTracks.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.lsvTracks_AfterLabelEdit);
            this.lsvTracks.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lsvTracks_ItemChecked);
            // 
            // clmTrackName
            // 
            this.clmTrackName.Text = "Name";
            this.clmTrackName.Width = 200;
            // 
            // pnlViews
            // 
            this.pnlViews.AutoScroll = true;
            this.pnlViews.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.pnlViews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlViews.Location = new System.Drawing.Point(0, 0);
            this.pnlViews.Name = "pnlViews";
            this.pnlViews.Size = new System.Drawing.Size(422, 381);
            this.pnlViews.TabIndex = 0;
            // 
            // mnuAddController
            // 
            this.mnuAddController.Name = "mnuAddView";
            this.mnuAddController.Size = new System.Drawing.Size(61, 4);
            this.mnuAddController.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.mnuAddController_ItemClicked);
            // 
            // mnuAddTrack
            // 
            this.mnuAddTrack.Name = "mnuAddTrack";
            this.mnuAddTrack.Size = new System.Drawing.Size(61, 4);
            this.mnuAddTrack.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.mnuAddTrack_ItemClicked);
            // 
            // ScreenView
            // 
            this.Controls.Add(this.spltMasterDetail);
            this.Size = new System.Drawing.Size(637, 381);
            this.Text = "5";
            pnlViewTools.ResumeLayout(false);
            pnlViewTools.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            this.spltMasterDetail.Panel1.ResumeLayout(false);
            this.spltMasterDetail.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spltMasterDetail)).EndInit();
            this.spltMasterDetail.ResumeLayout(false);
            this.spltSidebar.Panel1.ResumeLayout(false);
            this.spltSidebar.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spltSidebar)).EndInit();
            this.spltSidebar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer spltMasterDetail;
        private System.Windows.Forms.SplitContainer spltSidebar;
        private System.Windows.Forms.Panel pnlViews;
        private System.Windows.Forms.ListView lsvControllers;
        private System.Windows.Forms.TextBox txtSearchControllers;
        private System.Windows.Forms.Button btnDeleteController;
        private System.Windows.Forms.Button btnAddView;
        private System.Windows.Forms.ListView lsvTracks;
        private System.Windows.Forms.TextBox txtSearchTracks;
        private System.Windows.Forms.Button btnDeleteTrack;
        private System.Windows.Forms.Button btnAddTrack;
        private System.Windows.Forms.ContextMenuStrip mnuAddController;
        private System.Windows.Forms.ContextMenuStrip mnuAddTrack;
        private System.Windows.Forms.ColumnHeader clmViewName;
        private System.Windows.Forms.ColumnHeader clmTrackName;
    }
}
