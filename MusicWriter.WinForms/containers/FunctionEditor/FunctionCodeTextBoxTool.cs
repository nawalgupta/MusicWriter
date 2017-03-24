using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms
{
    public sealed class FunctionCodeTextBoxTool
    {
        readonly TextBox box;

        public TextBox Box {
            get { return box; }
        }

        public FunctionCodeTextBoxTool(TextBox box) {
            this.box = box;

            box.TextChanged += TextChanged;
        }

        CancellationTokenSource tokensource = new CancellationTokenSource();
        object tokensourcelocker = new object();

        public FunctionCodeTools CodeTools { get; set; }
        public EditorFile File { get; set; }

        public float DelaySeconds { get; set; } = 0;

        public event Action<KeyValuePair<Tuple<int, int>, string>[]> ErrorsFound;
        public event Action<IFunction> FunctionParsed;

        async void TextChanged(object sender, EventArgs e) {
            lock (tokensourcelocker) {
                tokensource.Cancel();
                tokensource = new CancellationTokenSource();
            }

            var token = tokensource.Token;

            if (DelaySeconds != 0)
                await Task.Delay((int)(DelaySeconds * 1000), token);

            if (!token.IsCancellationRequested) {
                var text = box.Text;
                var errors = new List<KeyValuePair<Tuple<int, int>, string>>();
                var func = CodeTools.Parse(ref text, File, errors);

                if (errors.Count > 0)
                    ErrorsFound?.Invoke(errors.ToArray());

                if (func != null)
                    FunctionParsed?.Invoke(func);
            }
        }
    }
}
