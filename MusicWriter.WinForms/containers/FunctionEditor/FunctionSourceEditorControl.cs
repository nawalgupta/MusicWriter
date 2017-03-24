using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms
{
    public partial class FunctionSourceEditorControl : UserControl
    {
        FunctionSource source;
        FunctionCodeTextBoxTool textboxtool;

        bool haserrors;
        bool HasErrors {
            get { return haserrors; }
            set {
                haserrors = value;

                btnPlay.Enabled = !haserrors;
            }
        }

        public FunctionSourceEditorControl() {
            InitializeComponent();
        }

        public void Setup(FunctionSource source) {
            this.source = source;

            source.Code.AfterChange += Code_AfterChange;

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
            HasErrors = true;
        }

        private void Textboxtool_FunctionParsed(IFunction obj) {
            source.Function.Value = obj;

            HasErrors = false;
        }

        private void btnPlay_Click(object sender, EventArgs e) {

        }
    }
}
