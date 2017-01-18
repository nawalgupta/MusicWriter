using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms
{
    public sealed class TabPageInterop : TabPage
    {
        readonly Control control;

        public Control Control {
            get { return control; }
        }

        public TabPageInterop(Control control) 
            :base(control.Text) {
            control.TextChanged += Control_TextChanged;
            TextChanged += TabPageInterop_TextChanged;
            Controls.Add(control);
            control.Dock = DockStyle.Fill;

            this.control = control;
        }

        private void TabPageInterop_TextChanged(object sender, EventArgs e) {
            if(control.Text != Text)
                control.Text = Text;
        }

        private void Control_TextChanged(object sender, EventArgs e) {
            if (Text != control.Text)
                Text = control.Text;
        }
    }
}
