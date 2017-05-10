namespace MusicWriter.WinForms
{
    partial class FunctionSourceEditorControl
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
            this.spltContainer = new System.Windows.Forms.SplitContainer();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.mnuCode = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuCodeCut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCodeCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCodePaste = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCodeSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCodeSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCodeInsert = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCodeSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCodeFont = new System.Windows.Forms.ToolStripMenuItem();
            this.diagCodeFont = new System.Windows.Forms.FontDialog();
            this.mnuCodeBackColor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCodeForeColor = new System.Windows.Forms.ToolStripMenuItem();
            this.diagCodeForeColor = new System.Windows.Forms.ColorDialog();
            this.diagCodeBackColor = new System.Windows.Forms.ColorDialog();
            ((System.ComponentModel.ISupportInitialize)(this.spltContainer)).BeginInit();
            this.spltContainer.Panel1.SuspendLayout();
            this.spltContainer.SuspendLayout();
            this.mnuCode.SuspendLayout();
            this.SuspendLayout();
            // 
            // spltContainer
            // 
            this.spltContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltContainer.Location = new System.Drawing.Point(0, 0);
            this.spltContainer.Name = "spltContainer";
            this.spltContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spltContainer.Panel1
            // 
            this.spltContainer.Panel1.Controls.Add(this.txtCode);
            this.spltContainer.Size = new System.Drawing.Size(552, 313);
            this.spltContainer.SplitterDistance = 156;
            this.spltContainer.TabIndex = 2;
            this.spltContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.spltContainer_SplitterMoved);
            // 
            // txtCode
            // 
            this.txtCode.ContextMenuStrip = this.mnuCode;
            this.txtCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCode.Location = new System.Drawing.Point(0, 0);
            this.txtCode.Multiline = true;
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(552, 156);
            this.txtCode.TabIndex = 1;
            this.txtCode.TextChanged += new System.EventHandler(this.txtCode_TextChanged);
            // 
            // mnuCode
            // 
            this.mnuCode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCodeCut,
            this.mnuCodeCopy,
            this.mnuCodePaste,
            this.mnuCodeSelectAll,
            this.mnuCodeSeparator1,
            this.mnuCodeInsert,
            this.mnuCodeSeparator2,
            this.mnuCodeFont,
            this.mnuCodeForeColor,
            this.mnuCodeBackColor});
            this.mnuCode.Name = "mnuCode";
            this.mnuCode.Size = new System.Drawing.Size(132, 192);
            // 
            // mnuCodeCut
            // 
            this.mnuCodeCut.Name = "mnuCodeCut";
            this.mnuCodeCut.Size = new System.Drawing.Size(131, 22);
            this.mnuCodeCut.Text = "Cu&t";
            // 
            // mnuCodeCopy
            // 
            this.mnuCodeCopy.Name = "mnuCodeCopy";
            this.mnuCodeCopy.Size = new System.Drawing.Size(131, 22);
            this.mnuCodeCopy.Text = "&Copy";
            // 
            // mnuCodePaste
            // 
            this.mnuCodePaste.Name = "mnuCodePaste";
            this.mnuCodePaste.Size = new System.Drawing.Size(131, 22);
            this.mnuCodePaste.Text = "&Paste";
            // 
            // mnuCodeSelectAll
            // 
            this.mnuCodeSelectAll.Name = "mnuCodeSelectAll";
            this.mnuCodeSelectAll.Size = new System.Drawing.Size(131, 22);
            this.mnuCodeSelectAll.Text = "Select &All";
            // 
            // mnuCodeSeparator1
            // 
            this.mnuCodeSeparator1.Name = "mnuCodeSeparator1";
            this.mnuCodeSeparator1.Size = new System.Drawing.Size(128, 6);
            // 
            // mnuCodeInsert
            // 
            this.mnuCodeInsert.Name = "mnuCodeInsert";
            this.mnuCodeInsert.Size = new System.Drawing.Size(131, 22);
            this.mnuCodeInsert.Text = "I&nsert...";
            // 
            // mnuCodeSeparator2
            // 
            this.mnuCodeSeparator2.Name = "mnuCodeSeparator2";
            this.mnuCodeSeparator2.Size = new System.Drawing.Size(128, 6);
            // 
            // mnuCodeFont
            // 
            this.mnuCodeFont.Name = "mnuCodeFont";
            this.mnuCodeFont.Size = new System.Drawing.Size(131, 22);
            this.mnuCodeFont.Text = "&Font";
            this.mnuCodeFont.Click += new System.EventHandler(this.mnuCodeFont_Click);
            // 
            // diagCodeFont
            // 
            this.diagCodeFont.ShowApply = true;
            this.diagCodeFont.ShowColor = true;
            this.diagCodeFont.Apply += new System.EventHandler(this.diagCodeFont_Apply);
            // 
            // mnuCodeBackColor
            // 
            this.mnuCodeBackColor.Name = "mnuCodeBackColor";
            this.mnuCodeBackColor.Size = new System.Drawing.Size(131, 22);
            this.mnuCodeBackColor.Text = "&Back Color";
            this.mnuCodeBackColor.Click += new System.EventHandler(this.mnuCodeBackColor_Click);
            // 
            // mnuCodeForeColor
            // 
            this.mnuCodeForeColor.Name = "mnuCodeForeColor";
            this.mnuCodeForeColor.Size = new System.Drawing.Size(131, 22);
            this.mnuCodeForeColor.Text = "F&ore Color";
            this.mnuCodeForeColor.Click += new System.EventHandler(this.mnuCodeForeColor_Click);
            // 
            // diagCodeForeColor
            // 
            this.diagCodeForeColor.AnyColor = true;
            // 
            // diagCodeBackColor
            // 
            this.diagCodeBackColor.AnyColor = true;
            // 
            // FunctionSourceEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.spltContainer);
            this.Name = "FunctionSourceEditorControl";
            this.Size = new System.Drawing.Size(552, 313);
            this.Load += new System.EventHandler(this.FunctionSourceEditorControl_Load);
            this.spltContainer.Panel1.ResumeLayout(false);
            this.spltContainer.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltContainer)).EndInit();
            this.spltContainer.ResumeLayout(false);
            this.mnuCode.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer spltContainer;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.ContextMenuStrip mnuCode;
        private System.Windows.Forms.ToolStripMenuItem mnuCodeCut;
        private System.Windows.Forms.ToolStripMenuItem mnuCodeCopy;
        private System.Windows.Forms.ToolStripMenuItem mnuCodePaste;
        private System.Windows.Forms.ToolStripMenuItem mnuCodeSelectAll;
        private System.Windows.Forms.ToolStripSeparator mnuCodeSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuCodeInsert;
        private System.Windows.Forms.ToolStripSeparator mnuCodeSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuCodeFont;
        private System.Windows.Forms.FontDialog diagCodeFont;
        private System.Windows.Forms.ToolStripMenuItem mnuCodeForeColor;
        private System.Windows.Forms.ToolStripMenuItem mnuCodeBackColor;
        private System.Windows.Forms.ColorDialog diagCodeForeColor;
        private System.Windows.Forms.ColorDialog diagCodeBackColor;
    }
}
