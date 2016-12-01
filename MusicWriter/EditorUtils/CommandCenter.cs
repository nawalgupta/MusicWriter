using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class CommandCenter
    {
        public event Action WhenToggleSelectAll;
        public event Action WhenSelectAll;
        public event Action WhenDeselectAll;

        public event Action<int> WhenCursor_Multiply;
        public event Action<int> WhenCursor_Divide;
        public event Action WhenCursor_ResetOne;
        
        public bool Enabled {
            get;
            set;
        } = true;

        public void ToggleSelectAll() =>
            Do(() => WhenToggleSelectAll?.Invoke());

        public void SelectAll() =>
            Do(() => WhenSelectAll?.Invoke());

        public void DeselectAll() =>
            Do(() => WhenDeselectAll?.Invoke());

        public void MultiplyCursor(int factor) =>
            Do(() => WhenCursor_Multiply?.Invoke(factor));

        public void DivideCursor(int divisor) =>
            Do(() => WhenCursor_Divide?.Invoke(divisor));

        public void ResetCursorToOne() =>
            Do(() => WhenCursor_ResetOne?.Invoke());

        void Do(Action act) {
            if (Enabled)
                act();
        }

        public void SubscribeTo(CommandCenter master) {
            master.WhenSelectAll += SelectAll;
            master.WhenDeselectAll += DeselectAll;
            master.WhenToggleSelectAll += ToggleSelectAll;

            master.WhenCursor_Multiply += MultiplyCursor;
            master.WhenCursor_Divide += DivideCursor;
            master.WhenCursor_ResetOne += ResetCursorToOne;
        }

        public void DesubscribeFrom(CommandCenter master) {
            master.WhenSelectAll -= SelectAll;
            master.WhenDeselectAll -= DeselectAll;
            master.WhenToggleSelectAll -= ToggleSelectAll;

            master.WhenCursor_Multiply -= MultiplyCursor;
            master.WhenCursor_Divide -= DivideCursor;
            master.WhenCursor_ResetOne -= ResetCursorToOne;
        }
    }
}
