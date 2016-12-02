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

        public event TimeChangedDelegate WhenPreviewTimeChanged;
        public event ToneChangedDelegate WhenPreviewToneChanged;
        public event TimeChangedDelegate WhenTimeChanged;
        public event ToneChangedDelegate WhenToneChanged;

        public event Action WhenTimeReset;
        public event Action WhenToneReset;
        public event Action WhenTimeStart;
        public event Action WhenToneStart;

        public event Action WhenNotePlacementStart;
        public event Action WhenNotePlacementFinish;

        public event Action WhenSelectionStart;
        public event Action WhenSelectionFinish;

        public event CaretUnitPickerDelegate WhenUnitPicking;

        public bool Enabled { get; set; } = true;

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

        public void ChangeTime_Preview(Time change, CaretMode mode) =>
            Do(() => WhenPreviewTimeChanged?.Invoke(change, mode));

        public void ChangeTone_Preview(int change, CaretMode mode) =>
            Do(() => WhenPreviewToneChanged?.Invoke(change, mode));

        public void ChangeTime(Time change, CaretMode mode) =>
            Do(() => WhenTimeChanged?.Invoke(change, mode));

        public void ChangeTone(int change, CaretMode mode) =>
            Do(() => WhenToneChanged?.Invoke(change, mode));

        public void ResetTime() =>
            Do(() => WhenTimeReset?.Invoke());

        public void ResetTone() =>
            Do(() => WhenToneReset?.Invoke());

        public void StartTime() =>
            Do(() => WhenTimeStart?.Invoke());

        public void StartTone() =>
            Do(() => WhenToneStart?.Invoke());

        public void StartNotePlacement() =>
            Do(() => WhenNotePlacementStart?.Invoke());

        public void FinishNotePlacement() =>
            Do(() => WhenNotePlacementFinish?.Invoke());

        public void StartSelection() =>
            Do(() => WhenSelectionStart?.Invoke());

        public void FinishSelection() =>
            Do(() => WhenSelectionFinish?.Invoke());

        public void Delegate_CaretUnitPicking(CaretUnitPickerEventArgs args) =>
            Do(() => {
                foreach (CaretUnitPickerDelegate invoc in WhenUnitPicking.GetInvocationList()) {
                    invoc(args);

                    if (args.Handled)
                        return;
                }
            });

        public Time? PickCaretUnit() {
            if (Enabled) {
                var args =
                    new CaretUnitPickerEventArgs();

                Delegate_CaretUnitPicking(args);

                if (args.Handled)
                    return args.Length;
            }

            return null;
        }

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

            master.WhenPreviewTimeChanged += ChangeTime_Preview;
            master.WhenPreviewToneChanged += ChangeTone_Preview;
            master.WhenTimeChanged += ChangeTime;
            master.WhenToneChanged += ChangeTone;

            master.WhenTimeStart += StartTime;
            master.WhenToneStart += StartTone;
            master.WhenTimeReset += ResetTime;
            master.WhenToneReset += ResetTone;

            master.WhenNotePlacementStart += StartNotePlacement;
            master.WhenNotePlacementFinish += FinishNotePlacement;

            master.WhenSelectionStart += StartSelection;
            master.WhenSelectionFinish += FinishSelection;

            master.WhenUnitPicking += Delegate_CaretUnitPicking;
        }

        public void DesubscribeFrom(CommandCenter master) {
            master.WhenSelectAll -= SelectAll;
            master.WhenDeselectAll -= DeselectAll;
            master.WhenToggleSelectAll -= ToggleSelectAll;

            master.WhenCursor_Multiply -= MultiplyCursor;
            master.WhenCursor_Divide -= DivideCursor;
            master.WhenCursor_ResetOne -= ResetCursorToOne;

            master.WhenPreviewTimeChanged -= ChangeTime_Preview;
            master.WhenPreviewToneChanged -= ChangeTone_Preview;
            master.WhenTimeChanged -= ChangeTime;
            master.WhenToneChanged -= ChangeTone;

            master.WhenTimeStart -= StartTime;
            master.WhenToneStart -= StartTone;
            master.WhenTimeReset -= ResetTime;
            master.WhenToneReset -= ResetTone;

            master.WhenNotePlacementStart -= StartNotePlacement;
            master.WhenNotePlacementFinish -= FinishNotePlacement;

            master.WhenSelectionStart -= StartSelection;
            master.WhenSelectionFinish -= FinishSelection;

            master.WhenUnitPicking -= Delegate_CaretUnitPicking;
        }
    }
}
