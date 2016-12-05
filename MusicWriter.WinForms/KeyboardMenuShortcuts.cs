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
        Keys nonmodifieractivekeys = Keys.None;

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
            {Keys.Oemtilde, "~" },
            {Keys.Delete, "Del" },
            {Keys.Insert, "Ins" },
            {Keys.Escape, "Esc" },
        };

        const int shortcutmask = (int)(Keys.Shift | Keys.Alt | Keys.Control);
        const int shortcutkeysmask = (int)(Keys.ControlKey | Keys.LControlKey | Keys.RControlKey | Keys.ShiftKey | Keys.LShiftKey | Keys.RShiftKey);

        public void OnKeyUp(KeyEventArgs args) {
            if (potentialcommandkey == args.KeyData || 
                (((int)potentialcommandkey & ~shortcutmask) == ((int)args.KeyData & ~shortcutmask) &&
                ((int)args.KeyData & ~shortcutmask) != 0)) {
                var shortcut = ((Keys)((int)potentialcommandkey & ~shortcutmask)).ToString();

                if (shortcut.Length >= 2 && shortcut[0] == 'D' && char.IsDigit(shortcut[1]))
                    shortcut = shortcut.Substring(1);
                else {
                    foreach (var keymap in keymappings)
                        if ((~shortcutmask & (int)potentialcommandkey) == (int)keymap.Key &&
                            (~(int)potentialcommandkey & (int)keymap.Key) == 0)
                            shortcut = keymap.Value;
                }

                if ((potentialcommandkey & Keys.Alt) != 0)
                    shortcut = "Alt+" + shortcut;

                if ((potentialcommandkey & Keys.Shift) != 0)
                    shortcut = "Shift+" + shortcut;

                if ((potentialcommandkey & Keys.Control) != 0)
                    shortcut = "Ctrl+" + shortcut;

                foreach (ToolStripMenuItem item in Menu.Items)
                    OnKeyDown(shortcut, item);
            }

            if (((int)args.KeyData & ~shortcutmask & ~shortcutkeysmask) != 0)
                potentialcommandkey = Keys.None;

            nonmodifieractivekeys &= (Keys)((int)args.KeyData & ~shortcutmask & ~shortcutkeysmask);
        }

        public void OnKeyDown(KeyEventArgs args) {
            if (((int)args.KeyData & ~shortcutmask & ~shortcutkeysmask) == 0)
                return;
            
            //if(nonmodifieractivekeys == Keys.None)
            if (((int)potentialcommandkey & ~shortcutmask) == (int)Keys.None)
                potentialcommandkey = args.KeyData;
            else if (((int)potentialcommandkey & ~shortcutmask) != ((int)args.KeyData & ~shortcutmask))
                potentialcommandkey = Keys.NoName; // impossible to execute

            nonmodifieractivekeys |= (Keys)((int)args.KeyData & ~shortcutkeysmask & ~shortcutmask);
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
