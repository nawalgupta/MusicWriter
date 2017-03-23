namespace MusicWriter
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
            this.spltManipulators = new System.Windows.Forms.SplitContainer();
            this.button1 = new System.Windows.Forms.Button();
            this.mnuPlay = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuPlayTone = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPlayLength = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPlayTempo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPlayVerb = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPlayRepeat = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPlayPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPlayStop = new System.Windows.Forms.ToolStripMenuItem();
            this.tabFunctionSources = new System.Windows.Forms.TabControl();
            mnuPlaySeparator1 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.spltManipulators)).BeginInit();
            this.spltManipulators.Panel1.SuspendLayout();
            this.spltManipulators.SuspendLayout();
            this.mnuPlay.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuPlaySeparator1
            // 
            mnuPlaySeparator1.Name = "mnuPlaySeparator1";
            mnuPlaySeparator1.Size = new System.Drawing.Size(108, 6);
            // 
            // spltManipulators
            // 
            this.spltManipulators.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltManipulators.Location = new System.Drawing.Point(0, 0);
            this.spltManipulators.Name = "spltManipulators";
            this.spltManipulators.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spltManipulators.Panel1
            // 
            this.spltManipulators.Panel1.Controls.Add(this.button1);
            this.spltManipulators.Panel1.Controls.Add(this.tabFunctionSources);
            this.spltManipulators.Size = new System.Drawing.Size(540, 396);
            this.spltManipulators.SplitterDistance = 225;
            this.spltManipulators.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.ContextMenuStrip = this.mnuPlay;
            this.button1.Location = new System.Drawing.Point(435, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 33);
            this.button1.TabIndex = 0;
            this.button1.Text = "Play...";
            this.button1.UseVisualStyleBackColor = true;
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
            // tabFunctionSources
            // 
            this.tabFunctionSources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabFunctionSources.Location = new System.Drawing.Point(0, 0);
            this.tabFunctionSources.Name = "tabFunctionSources";
            this.tabFunctionSources.SelectedIndex = 0;
            this.tabFunctionSources.Size = new System.Drawing.Size(540, 225);
            this.tabFunctionSources.TabIndex = 0;
            // 
            // FunctionEditorScreenView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.spltManipulators);
            this.Name = "FunctionEditorScreenView";
            this.Size = new System.Drawing.Size(540, 396);
            this.Load += new System.EventHandler(this.FunctionEditorScreenView_Load);
            this.spltManipulators.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spltManipulators)).EndInit();
            this.spltManipulators.ResumeLayout(false);
            this.mnuPlay.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer spltManipulators;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tabFunctionSources;
        private System.Windows.Forms.ContextMenuStrip mnuPlay;
        private System.Windows.Forms.ToolStripMenuItem mnuPlayTone;
        private System.Windows.Forms.ToolStripMenuItem mnuPlayLength;
        private System.Windows.Forms.ToolStripMenuItem mnuPlayTempo;
        private System.Windows.Forms.ToolStripMenuItem mnuPlayVerb;
        private System.Windows.Forms.ToolStripMenuItem mnuPlayRepeat;
        private System.Windows.Forms.ToolStripMenuItem mnuPlayPlay;
        private System.Windows.Forms.ToolStripMenuItem mnuPlayStop;
    }
}
