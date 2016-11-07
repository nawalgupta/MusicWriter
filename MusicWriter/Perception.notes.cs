using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    partial class Perception {
        IEnumerable<PerceptualNote> Decompose_Note(Note note) {
            var cells =
                track.Rhythm.CellsIn(note.Duration);

            var i = 0;

            foreach (var cell in cells) {
                var cutduration = cell.Duration.Intersection(note.Duration);

                var length =
                    Decompose_Time(cutduration.Length);

                yield return new PerceptualNote(
                        new PerceptualNoteID(note.ID, i++),
                        cutduration,
                        length,
                        cell
                    );
            }
        }

        PerceptualTime Decompose_Time(Time length) {

        }
    }
}
