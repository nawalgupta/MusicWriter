using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms
{
    public sealed class KeyboardMenuShortcuts
    {
        public MenuStrip Menu { get; set; }

        Keys potentialcommandkey = Keys.None;

        Dictionary<Keys, string> keymappings = new Dictionary<Keys, string> {
            {Keys.OemBackslash, "\\" },
                {Keys.OemCloseBrackets, "}" },
            {Keys.Oemcomma, "," },
            {Keys.OemMinus, "-" },
            {Keys.OemOpenBrackets, "{" },
            {Keys.OemPeriod, "." },
            {Keys.OemPipe, "|" },
            {Keys.Oemplus, "+" },
            {Keys.OemQuestion, "?" },
            {Keys.OemQuotes, "\"" },
            {Keys.OemSemicolon, ";" },
            {Keys.Oemtilde, "~" }
        };

        public void OnKeyUp(KeyEventArgs args) {
            if (potentialcommandkey == args.KeyCode) {
                var shortcut = args.KeyCode.ToString();

                if (shortcut[0] == 'D' && char.IsDigit(shortcut[1]))
                    shortcut = shortcut.Substring(1);
                else {
                    const int shortcutmask = (int)Keys.Shift | (int)Keys.Alt | (int)Keys.Control;

                    foreach (var keymap in keymappings)
                        if ((~shortcutmask & (int)args.KeyCode) == (int)keymap.Key &&
                            (~(int)args.KeyCode & (int)keymap.Key) == 0)
                            shortcut = keymap.Value;
                }

                if (args.Alt)
                    shortcut = "Alt+" + shortcut;

                if (args.Shift)
                    shortcut = "Shift+" + shortcut;

                if (args.Control)
                    shortcut = "Ctrl+" + shortcut;

                foreach (ToolStripMenuItem item in Menu.Items)
                    OnKeyDown(shortcut, item);
            }

            potentialcommandkey = Keys.None;
        }

        public void OnKeyDown(KeyEventArgs args) {
            if (args.Control || args.Alt)
                return;

            if (potentialcommandkey == Keys.None)
                potentialcommandkey = args.KeyCode;
            else if (potentialcommandkey != args.KeyCode)
                potentialcommandkey = Keys.NoName; // impossible to execute
        }

        void OnKeyDown(string keys, ToolStripMenuItem strip) {
            if (strip.ShortcutKeyDisplayString == keys)
                strip.PerformClick();

            foreach (var item in strip.DropDownItems) {
                var menustrip =
                    item as ToolStripMenuItem;

                if (menustrip != null)
                    OnKeyDown(keys, menustrip);
            }
        }
    }
}
