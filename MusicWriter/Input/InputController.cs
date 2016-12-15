using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class InputController {
        readonly CommandCenter commandcenter;
        //public event TimeChangedDelegate PreviewTimeChanged;
        //public event ToneChangedDelegate PreviewToneChanged;
        //public event TimeChangedDelegate TimeChanged;
        //public event ToneChangedDelegate ToneChanged;
        //public event Action TimeReset;
        //public event Action ToneReset;
        //public event Action TimeStart;
        //public event Action ToneStart;
        //public event Action NotePlacementStart;
        //public event Action NotePlacementFinish;
        //public event Action SelectionStart;
        //public event Action SelectionFinish;

        int? tone = null;
        CaretMode? tone_mode = null;

        Time? time = null;
        CaretMode? time_mode = null;
        
        public CommandCenter CommandCenter {
            get { return commandcenter; }
        }

        public Time UnitLength {
            get { return commandcenter.PickCaretUnit().GetValueOrDefault(); }
        }

        public InputController(
                CommandCenter commandcenter
            ) {
            this.commandcenter = commandcenter;
        }

        public void StartDrawingNote() {
            commandcenter.StartNotePlacement();

            time = Time.Zero;
            time_mode = CaretMode.Delta;

            tone = 0;
            tone_mode = CaretMode.Delta;
        }

        public void FinishDrawingNote() {
            commandcenter.FinishNotePlacement();

            time = null;
            time_mode = null;
            tone = null;
            tone_mode = null;
        }

        public void StartSelecting() {
            commandcenter.StartSelection();

            time = Time.Zero;
            time_mode = CaretMode.Delta;
        }

        public void FinishSelecting() {
            commandcenter.FinishSelection();

            time = null;
            time_mode = null;
        }

        public void OffsetTime(Time offset) {
            if (time_mode == null) {
                time_mode = CaretMode.Delta;
                time = Time.Zero;

                commandcenter.StartTime();
            }

            time += offset;

            if ((time != Time.Zero && time_mode.Value.HasFlag(CaretMode.Delta)) || !time_mode.Value.HasFlag(CaretMode.Delta))
                commandcenter.ChangeTime_Preview(time.Value, time_mode.Value);
        }

        public void OffsetTone(int offset, bool wholetones) {
            if (tone_mode == null) {
                tone_mode = CaretMode.Delta | (wholetones ? CaretMode.WholeTones : CaretMode.SemiTones);
                tone = 0;

                commandcenter.StartTone();
            }

            tone += offset;

            if ((tone != 0 && tone_mode.Value.HasFlag(CaretMode.Delta)) || !tone_mode.Value.HasFlag(CaretMode.Delta))
                commandcenter.ChangeTone_Preview(tone.Value, tone_mode.Value);
        }

        public void SetTime(Time value) {
            if (time_mode == null)
                commandcenter.StartTime();

            time_mode = CaretMode.Absolute;
            time = value;

            commandcenter.ChangeTime_Preview(time.Value, time_mode.Value);
        }

        public void SetTone(int value) {
            if (tone_mode == null)
                commandcenter.StartTone();

            tone_mode = CaretMode.Absolute;
            tone = value;

            commandcenter.ChangeTone_Preview(tone.Value, tone_mode.Value);
        }

        public void FinishTime() {
            if (time.HasValue) {
                commandcenter.ChangeTime(time.Value, time_mode.Value);

                time = null;
                time_mode = null;
            }
        }

        public void CancelTime() {
            commandcenter.ResetTime();

            time = null;
            time_mode = null;
        }

        public void FinishTone() {
            if (tone.HasValue) {
                commandcenter.ChangeTone(tone.Value, tone_mode.Value);

                tone = null;
                tone_mode = null;
            }
        }

        public void CancelTone() {
            commandcenter.ResetTone();

            tone = null;
            tone_mode = null;
        }
    }
}
