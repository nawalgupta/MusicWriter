namespace MusicWriter.WinForms
{
    partial class BoundListView
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
            this.tblLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lsvItems = new System.Windows.Forms.ListView();
            this.clmName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtSearch = new PlaceholderTextBox.PlaceholderTextBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.tblLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblLayout
            // 
            this.tblLayout.ColumnCount = 3;
            this.tblLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tblLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tblLayout.Controls.Add(this.lsvItems, 0, 1);
            this.tblLayout.Controls.Add(this.txtSearch, 0, 0);
            this.tblLayout.Controls.Add(this.btnRemove, 1, 0);
            this.tblLayout.Controls.Add(this.btnCreate, 2, 0);
            this.tblLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLayout.Location = new System.Drawing.Point(0, 0);
            this.tblLayout.Name = "tblLayout";
            this.tblLayout.RowCount = 2;
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblLayout.Size = new System.Drawing.Size(164, 242);
            this.tblLayout.TabIndex = 0;
            // 
            // lsvItems
            // 
            this.lsvItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmName});
            this.tblLayout.SetColumnSpan(this.lsvItems, 3);
            this.lsvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lsvItems.HideSelection = false;
            this.lsvItems.Location = new System.Drawing.Point(0, 21);
            this.lsvItems.Margin = new System.Windows.Forms.Padding(0);
            this.lsvItems.Name = "lsvItems";
            this.lsvItems.Size = new System.Drawing.Size(164, 221);
            this.lsvItems.TabIndex = 1;
            this.lsvItems.UseCompatibleStateImageBehavior = false;
            this.lsvItems.View = System.Windows.Forms.View.Details;
            this.lsvItems.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lsvItems_ItemChecked);
            this.lsvItems.SelectedIndexChanged += new System.EventHandler(this.lsvItems_SelectedIndexChanged);
            // 
            // clmName
            // 
            this.clmName.Text = "Name";
            this.clmName.Width = 120;
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(0, 0);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(0);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PlaceholderText = "Search";
            this.txtSearch.Size = new System.Drawing.Size(120, 20);
            this.txtSearch.TabIndex = 1;
            // 
            // btnRemove
            // 
            this.btnRemove.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRemove.Location = new System.Drawing.Point(120, 0);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(0);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(22, 21);
            this.btnRemove.TabIndex = 0;
            this.btnRemove.Text = "-";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // btnCreate
            // 
            this.btnCreate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCreate.Location = new System.Drawing.Point(142, 0);
            this.btnCreate.Margin = new System.Windows.Forms.Padding(0);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(22, 21);
            this.btnCreate.TabIndex = 1;
            this.btnCreate.Text = "+";
            this.btnCreate.UseVisualStyleBackColor = true;
            // 
            // BoundListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblLayout);
            this.Name = "BoundListView";
            this.Size = new System.Drawing.Size(164, 242);
            this.tblLayout.ResumeLayout(false);
            this.tblLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblLayout;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.ListView lsvItems;
        private System.Windows.Forms.ColumnHeader clmName;
        private PlaceholderTextBox.PlaceholderTextBox txtSearch;
    }
}
