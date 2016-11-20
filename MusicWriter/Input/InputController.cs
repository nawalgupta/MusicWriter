using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class InputController {
        public event TimeChangedDelegate PreviewTimeChanged;
        public event ToneChangedDelegate PreviewToneChanged;
        public event TimeChangedDelegate TimeChanged;
        public event ToneChangedDelegate ToneChanged;
        public event Action TimeReset;
        public event Action ToneReset;
        public event Action TimeStart;
        public event Action ToneStart;
        public event Action NotePlacementStart;
        public event Action NotePlacementFinish;
        public event Action SelectionStart;
        public event Action SelectionFinish;

        int? tone = null;
        CaretMode? tone_mode = null;

        Time? time = null;
        CaretMode? time_mode = null;

        public Time UnitTime { get; set; } = Time.Note_8th;

        public void StartDrawingNote() {
            NotePlacementStart?.Invoke();

            time = Time.Zero;
            time_mode = CaretMode.Delta;

            tone = 0;
            tone_mode = CaretMode.Delta;
        }

        public void FinishDrawingNote() {
            NotePlacementFinish?.Invoke();

            time = null;
            time_mode = null;
            tone = null;
            tone_mode = null;
        }

        public void StartSelecting() {
            SelectionStart?.Invoke();

            time = Time.Zero;
            time_mode = CaretMode.Delta;
        }

        public void FinishSelecting() {
            SelectionFinish?.Invoke();

            time = null;
            time_mode = null;
        }

        public void OffsetTime(Time offset) {
            if (time_mode == null) {
                time_mode = CaretMode.Delta;
                time = Time.Zero;

                TimeStart?.Invoke();
            }

            time += offset;

            PreviewTimeChanged?.Invoke(time.Value, time_mode.Value);
        }

        public void OffsetTone(int offset) {
            if (tone_mode == null) {
                tone_mode = CaretMode.Delta;
                tone = 0;

                ToneStart?.Invoke();
            }

            tone += offset;

            PreviewToneChanged?.Invoke(tone.Value, tone_mode.Value);
        }

        public void SetTime(Time value) {
            if (time_mode == null)
                TimeStart?.Invoke();

            time_mode = CaretMode.Absolute;
            time = value;

            PreviewTimeChanged?.Invoke(time.Value, time_mode.Value);
        }

        public void SetTone(int value) {
            if (tone_mode == null)
                ToneStart?.Invoke();

            tone_mode = CaretMode.Absolute;
            tone = value;

            PreviewToneChanged?.Invoke(tone.Value, tone_mode.Value);
        }
        
        public void FinishTime() {
            TimeChanged?.Invoke(time.Value, time_mode.Value);
        }

        public void CancelTime() {
            TimeReset?.Invoke();

            time = null;
            time_mode = null;
        }

        public void FinishTone() {
            ToneChanged?.Invoke(tone.Value, tone_mode.Value);
        }

        public void CancelTone() {
            ToneReset?.Invoke();

            tone = null;
            tone_mode = null;
        }
    }
}
