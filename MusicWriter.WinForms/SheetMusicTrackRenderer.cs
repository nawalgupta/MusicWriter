using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms {
    public sealed class SheetMusicTrackRenderer {
        public MusicTrack Track { get; set; }
        public PerceptualMemory Memory { get; set; }
        public SheetMusicRenderSettings Settings { get; set; }
        
        public int GetLeft(Time time) {
            var duration =
                new Duration {
                    Start = Time.Zero,
                    Length = time
                };

            var skippedmeasureswidth =
                Memory
                    .Analyses<RenderedSheetMusicItem>(duration)
                    .Sum(measure_item => measure_item.Value.Width(Settings));

            return (int)skippedmeasureswidth;
        }
    }
}
