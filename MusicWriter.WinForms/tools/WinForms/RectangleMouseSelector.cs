using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms
{
    public sealed class RectangleMouseSelector
    {
        RectangleF rect = new RectangleF() {
            X = float.NaN,
            Y = float.NaN
        };
        NoteSelectionMode mode = NoteSelectionMode.None;

        public event Action<RectangleF, NoteSelectionMode> Selected;
        public event Action Redraw;

        public void MouseMove(MouseEventArgs args, Keys modifiers) {
            if (mode != NoteSelectionMode.None) {
                rect.Width = args.X - rect.X;
                rect.Height = args.Y - rect.Y;

                if (modifiers == Keys.Shift)
                    mode = NoteSelectionMode.Add;
                else if (modifiers == Keys.Alt)
                    mode = NoteSelectionMode.Intersect;
                else if (modifiers == Keys.Control)
                    mode = NoteSelectionMode.Subtract;
                else if (modifiers == Keys.Space)
                    mode = NoteSelectionMode.Replace;

                Redraw?.Invoke();
            }
        }

        public void MouseDown(MouseEventArgs args, Keys modifiers) {
            rect.X = args.X;
            rect.Y = args.Y;

            if (modifiers == Keys.Shift)
                mode = NoteSelectionMode.Add;
            else if (modifiers == Keys.Alt)
                mode = NoteSelectionMode.Intersect;
            else if (modifiers == Keys.Control)
                mode = NoteSelectionMode.Subtract;
            else mode = NoteSelectionMode.Replace;
        }

        public void MouseUp(MouseEventArgs args) {
            if (rect.Width < 0) {
                rect.X += rect.Width;
                rect.Width *= -1;
            }

            if (rect.Height < 0) {
                rect.Y += rect.Height;
                rect.Height *= -1;
            }

            Selected?.Invoke(rect, mode);

            mode = NoteSelectionMode.None;
            rect.X = float.NaN;
            rect.Y = float.NaN;
        }

        public void Draw(Graphics gfx) {
            if (mode != NoteSelectionMode.None) {
                Color fill;

                switch (mode) {
                    case NoteSelectionMode.Replace:
                        fill = Color.CornflowerBlue;
                        break;

                    case NoteSelectionMode.Add:
                        fill = Color.SeaGreen;
                        break;

                    case NoteSelectionMode.Intersect:
                        fill = Color.BlueViolet;
                        break;

                    case NoteSelectionMode.Subtract:
                        fill = Color.MediumPurple;
                        break;

                    case NoteSelectionMode.Xor:
                        fill = Color.SteelBlue;
                        break;

                    default:
                        fill = default(Color);
                        break;
                }

                gfx.FillRectangle(
                        new SolidBrush(Color.FromArgb(100, fill)),
                        Math.Min(rect.Left, rect.Right),
                        Math.Min(rect.Top, rect.Bottom),
                        Math.Abs(rect.Width),
                        Math.Abs(rect.Height)
                    );

                gfx.DrawRectangle(
                        new Pen(fill, 1.3f),
                        Math.Min(rect.Left, rect.Right),
                        Math.Min(rect.Top, rect.Bottom),
                        Math.Abs(rect.Width),
                        Math.Abs(rect.Height)
                    );
            }
        }
    }
}
