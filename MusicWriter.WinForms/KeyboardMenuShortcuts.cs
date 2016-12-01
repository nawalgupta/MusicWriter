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

        public void OnKeyUp(KeyEventArgs args) {
            if (potentialcommandkey == args.KeyCode) {
                var shortcut = args.KeyCode.ToString();

                if (args.Alt)
                    shortcut = "Alt+" + shortcut;

                if (args.Shift)
                    shortcut = "Shift+" + shortcut;

                if (args.Control)
                    shortcut = "Ctrl+" + shortcut;

                foreach (ToolStripMenuItem item in Menu.Items)
                    OnKeyDown(shortcut, item);
            }
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
