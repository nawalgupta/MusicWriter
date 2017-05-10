using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicWriter.WinForms.Properties;

namespace MusicWriter.WinForms
{
    public partial class FunctionSourceEditorControl : UserControl
    {
        FunctionSource source;
        FunctionCodeTextBoxTool textboxtool;

        public ObservableProperty<bool> HasErrors { get; } =
            new ObservableProperty<bool>(false);

        public FunctionSourceEditorControl() {
            InitializeComponent();
        }

        public void Setup(FunctionSource source) {
            this.source = source;

            source.Code.AfterChange += Code_AfterChange;
            Code_AfterChange(null, source.Code.Value);

            textboxtool = new FunctionCodeTextBoxTool(txtCode);
            textboxtool.CodeTools = source.Container.FunctionCodeTools;
            textboxtool.File = source.File;
            textboxtool.FunctionParsed += Textboxtool_FunctionParsed;
            textboxtool.ErrorsFound += Textboxtool_ErrorsFound;
        }

        public void UnSetup() {
            source.Code.AfterChange -= Code_AfterChange;
        }

        private void Code_AfterChange(string old, string @new) {
            if (@new != txtCode.Text)
                txtCode.Text = @new;
        }

        private void Textboxtool_ErrorsFound(KeyValuePair<Tuple<int, int>, string>[] errors) {
            HasErrors.Value = true;
        }

        private void Textboxtool_FunctionParsed(IFunction obj) {
            source.Function.Value = obj;

            HasErrors.Value = false;
        }

        private void txtCode_TextChanged(object sender, EventArgs e) {

        }

        private void FunctionSourceEditorControl_Load(object sender, EventArgs e) {
            FunctionEditorThemelet.CodeFont.Set += CodeFont_Set;
            FunctionEditorThemelet.CodePaint.Set += CodePaint_Set;
            FunctionEditorThemelet.SplitterFraction.Set += SplitterFraction_Set;
        }

        private void CodeFont_Set(Font value) {
            txtCode.Font = value;
        }

        private void CodePaint_Set(Theme.Paint value) {
            txtCode.BackColor = value.BackColor;
            txtCode.ForeColor = value.ForeColor;
        }

        private void SplitterFraction_Set(float value) {
            spltContainer.SplitterDistance = (int)(spltContainer.Height * value);
        }

        private void mnuCodeFont_Click(object sender, EventArgs e) {
            diagCodeFont.Font = FunctionEditorThemelet.CodeFont.Value;
            diagCodeFont.Color = FunctionEditorThemelet.CodePaint.Value.ForeColor;

            if (diagCodeFont.ShowDialog(this) == DialogResult.OK) {
                FunctionEditorThemelet.CodeFont.Value = diagCodeFont.Font;

                if (diagCodeFont.Color != FunctionEditorThemelet.CodePaint.Value.ForeColor)
                    FunctionEditorThemelet.CodePaint.Value =
                        new Theme.Paint(
                                diagCodeFont.Color,
                                FunctionEditorThemelet.CodePaint.Value.BackColor
                            );
            }
        }

        private void mnuCodeForeColor_Click(object sender, EventArgs e) {
            diagCodeForeColor.Color = FunctionEditorThemelet.CodePaint.Value.ForeColor;
            if (diagCodeForeColor.ShowDialog(this) == DialogResult.OK)
                FunctionEditorThemelet.CodePaint.Value =
                    new Theme.Paint(
                            diagCodeForeColor.Color,
                            FunctionEditorThemelet.CodePaint.Value.BackColor
                        );
        }

        private void mnuCodeBackColor_Click(object sender, EventArgs e) {
            diagCodeBackColor.Color = FunctionEditorThemelet.CodePaint.Value.BackColor;
            if (diagCodeBackColor.ShowDialog(this) == DialogResult.OK)
                FunctionEditorThemelet.CodePaint.Value =
                    new Theme.Paint(
                            FunctionEditorThemelet.CodePaint.Value.ForeColor,
                            diagCodeBackColor.Color
                        );
        }

        private void diagCodeFont_Apply(object sender, EventArgs e) {
            txtCode.Font = diagCodeFont.Font;
            txtCode.ForeColor = diagCodeFont.Color;
        }

        private void spltContainer_SplitterMoved(object sender, SplitterEventArgs e) {
            FunctionEditorThemelet.SplitterFraction.Value = spltContainer.SplitterDistance / spltContainer.Height;
        }
    }
}
