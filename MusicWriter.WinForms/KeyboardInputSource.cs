using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms
{
    public class KeyboardInputSource : IInputSource
    {
        List<Keys> previewactions = new List<Keys>();

        public InputController Controller { get; set; }

        public ObservableCollection<MusicTrack> Tracks { get; } =
            new ObservableCollection<MusicTrack>();

        private Keys[][] keymap = new Keys[][] {
            new Keys[] { Keys.NoName, Keys.Z, Keys.X, Keys.C, Keys.V, Keys.B, Keys.N, Keys.M, Keys.Oemcomma, Keys.OemPeriod, Keys.OemQuestion },
            new Keys[] { Keys.NoName, Keys.A, Keys.S, Keys.D, Keys.F, Keys.G, Keys.H, Keys.J, Keys.K, Keys.L, Keys.OemSemicolon, Keys.OemQuotes },
            new Keys[] { Keys.NoName, Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y, Keys.U, Keys.I, Keys.O, Keys.P, Keys.OemOpenBrackets, Keys.OemCloseBrackets, Keys.OemBackslash },
            new Keys[] { Keys.Oemtilde, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0, Keys.OemMinus, Keys.Oemplus },
        };

        bool drawing = false;
        bool selecting = false;
        bool dragging = false;
        int old_x = 0,
            old_y = 0;
        Keys dragging_key1 = default(Keys);
        Keys dragging_key2 = default(Keys);

        public void OnKeyDown(KeyEventArgs e) {
            if (e.Alt) {
                // shift by semitones

                if (e.KeyCode == Keys.Down) {
                    Controller.OffsetTone(-1, false);
                    Controller.FinishTone();
                }
                if (e.KeyCode == Keys.Up) {
                    Controller.OffsetTone(+1, false);
                    Controller.FinishTone();
                }
            }
            else {
                // shift by whole tones
                //TODO

                if (e.KeyCode == Keys.Down) {
                    Controller.OffsetTone(-1, true);
                    Controller.FinishTone();
                }
                if (e.KeyCode == Keys.Up) {
                    Controller.OffsetTone(+1, true);
                    Controller.FinishTone();
                }
            }

            int x, y;
            if (GetXY(e.KeyCode, out x, out y)) {
                if (dragging_key1 != default(Keys)) {
                    var dx = x - old_x;
                    var dy = y - old_y;

                    if (dx != 0)
                        Controller.OffsetTime(dx * Controller.UnitLength);

                    if (dy != 0)
                        Controller.OffsetTone(dy, !e.Alt);

                    dragging = true;
                    dragging_key2 = e.KeyCode;
                }
                else {
                    if (e.Shift) {
                        Controller.StartSelecting();
                        selecting = true;
                    }

                    if (e.Control) {
                        Controller.StartDrawingNote();
                        drawing = true;
                    }

                    dragging_key1 = e.KeyCode;
                }

                old_x = x;
                old_y = y;
            }

            if (e.KeyCode == Keys.Enter) {
                Controller.FinishTime();
                Controller.FinishTone();

                dragging = false;
                drawing = false;
                selecting = false;
            }
        }

        bool GetXY(Keys key, out int x, out int y) {
            for (y = 0; y < keymap.Length; y++)
                for (x = 0; x < keymap[y].Length; x++)
                    if (keymap[y][x] == key)
                        return true;

            x = y = -1;
            return false;
        }

        public void OnKeyUp(KeyEventArgs e) {
            if (e.KeyCode == dragging_key1) {
                dragging_key1 = dragging_key2;
                dragging_key2 = default(Keys);

                if (dragging_key1 == default(Keys)) {
                    Controller.FinishTime();
                    Controller.FinishTone();

                    if (drawing)
                        Controller.FinishDrawingNote();

                    if (selecting)
                        Controller.FinishSelecting();

                    dragging = false;
                    drawing = false;
                    selecting = false;
                }
            }
            else if (e.KeyCode == dragging_key2 && dragging) {
                dragging_key2 = default(Keys);
                GetXY(dragging_key1, out old_x, out old_y);

                Controller.CancelTime();
                Controller.CancelTone();
            }
        }
    }
}
