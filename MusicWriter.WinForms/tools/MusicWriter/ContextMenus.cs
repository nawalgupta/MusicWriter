using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter
{
    public static class ContextMenus
    {
        public static void Attach_Tone(
                ToolStripMenuItem mnuTone,
                ObservableProperty<SemiTone> tone,
                int octave_min = -1,
                int octave_max = 8
            ) {
            SemiTone val = tone.Value;
            //TODO: adjust to make it possible for the tone observable property
            // to change and the menu to update accordingly.

            var mnuTone_class_checkedchanged = new EventHandler((sender, e) => {
                var mnuTone_class = (ToolStripMenuItem)sender;
                var pitchclass = (ChromaticPitchClass)mnuTone_class.Tag;

                if (mnuTone_class.Checked) {
                    foreach (ChromaticPitchClass other_pitchclass in Enum.GetValues(typeof(ChromaticPitchClass))) {
                        var other_mnuTone_class = (ToolStripMenuItem)mnuTone.DropDownItems[(int)other_pitchclass];
                        other_mnuTone_class.Checked = other_mnuTone_class == mnuTone_class;
                    }

                    val.PitchClass = pitchclass;
                    tone.Value = val;
                }
            });

            var mnuTone_class_octave_checked = new EventHandler((sender, e) => {
                var mnuTone_class_octave = (ToolStripMenuItem)sender;
                var semitone = (SemiTone)mnuTone_class_octave.Tag;

                if (mnuTone_class_octave.Checked) {
                    var mnuTone_class = (ToolStripMenuItem)mnuTone.DropDownItems[(int)semitone.PitchClass];
                    for (var other_octave = octave_min; other_octave <= octave_max; other_octave++) {
                        var other_mnuTone_class_octave = (ToolStripMenuItem)mnuTone_class.DropDownItems[other_octave];
                        
                        other_mnuTone_class_octave.Checked = other_mnuTone_class_octave == mnuTone_class_octave;
                    }

                    val = semitone;
                    tone.Value = val;
                }
            });

            var mnuTone_class_dropdownopening = new EventHandler((sender, e) => {
                var mnuTone_class = (ToolStripMenuItem)sender;
                var pitchclass = (ChromaticPitchClass)mnuTone_class.Tag;

                if (mnuTone_class.DropDownItems.Count == 1) {
                    // replace stub with items

                    mnuTone_class.DropDownItems.Clear();

                    for (var octave = octave_min; octave <= octave_max; octave++) {
                        var mnuTone_class_octave = new ToolStripMenuItem($"Octave {octave}");
                        mnuTone_class_octave.CheckedChanged += mnuTone_class_octave_checked;
                        mnuTone_class_octave.Tag = new SemiTone(pitchclass, octave);

                        mnuTone_class.DropDownItems.Add(mnuTone_class_octave);
                    }
                }

                for (var octave = octave_min; octave <= octave_max; octave++) {
                    var mnuTone_class_octave = (ToolStripMenuItem)mnuTone_class.DropDownItems[octave - octave_min];
                    var semitone = (SemiTone)mnuTone_class_octave.Tag;

                    mnuTone_class_octave.Checked = semitone == val;
                }
            });

            foreach (ChromaticPitchClass pitchclass in Enum.GetValues(typeof(ChromaticPitchClass))) {
                var mnuTone_class =
                    new ToolStripMenuItem();

                mnuTone_class.Tag = pitchclass;

                var pitch_str = pitchclass.Stringify_sharps();
                if (!pitch_str.Contains("#") &&
                    !pitch_str.Contains("♭"))
                    pitch_str = "&" + pitch_str;

                mnuTone_class.CheckOnClick = true;
                mnuTone_class.Checked = val.PitchClass == pitchclass;
                mnuTone_class.CheckedChanged += mnuTone_class_checkedchanged;

                mnuTone_class.Text = pitch_str;
                mnuTone_class.DropDownItems.Add("_");

                mnuTone_class.DropDownOpening += mnuTone_class_dropdownopening;
            }
        }

        public static void Attach_PerceptualTime(
                ToolStripMenuItem mnuPerceptualTime,
                ObservableProperty<PerceptualTime> perceptualtime
            ) {
            var mnuPerceptualTime_lengthclasses = new Dictionary<LengthClass, ToolStripMenuItem>();
            var mnuPerceptualTime_tupletclasses = new Dictionary<TupletClass, ToolStripMenuItem>();
            var mnuPerceptualTime_dotsclasses = new Dictionary<int, ToolStripMenuItem>();

            var mnuPerceptualTime_length = (ToolStripMenuItem)mnuPerceptualTime.DropDownItems.Add("Length");
            var mnuPerceptualTime_tuplet = (ToolStripMenuItem)mnuPerceptualTime.DropDownItems.Add("Tuplet");
            var mnuPerceptualTime_dots = (ToolStripMenuItem)mnuPerceptualTime.DropDownItems.Add("Dots");

            mnuPerceptualTime_length.DropDownOpening += (sender, e) => {
                foreach (ToolStripMenuItem item in mnuPerceptualTime_length.DropDownItems)
                    item.Checked = (LengthClass)item.Tag == perceptualtime.Value.Length;
            };

            mnuPerceptualTime_tuplet.DropDownOpening += (sender, e) => {
                foreach (ToolStripMenuItem item in mnuPerceptualTime_length.DropDownItems)
                    item.Checked = (TupletClass)item.Tag == perceptualtime.Value.Tuplet;
            };

            mnuPerceptualTime_dots.DropDownOpening += (sender, e) => {
                foreach (ToolStripMenuItem item in mnuPerceptualTime_length.DropDownItems)
                    item.Checked = (int)item.Tag == perceptualtime.Value.Dots;
            };

            foreach (LengthClass _ in Enum.GetValues(typeof(LengthClass)))
                (new Action<LengthClass>(lengthclass => {
                    var mnuPerceptualTime_lengthclass =
                        mnuPerceptualTime_length.DropDownItems.Add(lengthclass.ToString()) as ToolStripMenuItem;

                    mnuPerceptualTime_lengthclass.Tag = lengthclass;

                    mnuPerceptualTime_lengthclass.Click += (sender, e) => {
                        perceptualtime.Value =
                            new PerceptualTime(
                                    perceptualtime.Value.Tuplet,
                                    lengthclass,
                                    perceptualtime.Value.Dots
                                );
                    };

                    mnuPerceptualTime_lengthclasses.Add(lengthclass, mnuPerceptualTime_lengthclass);
                }))(_);

            foreach (TupletClass _ in Enum.GetValues(typeof(TupletClass)))
                (new Action<TupletClass>(tupletclass => {
                    var mnuPerceptualTime_tupletclass =
                        mnuPerceptualTime_tuplet.DropDownItems.Add(tupletclass.ToString()) as ToolStripMenuItem;

                    mnuPerceptualTime_tupletclass.Tag = tupletclass;

                    mnuPerceptualTime_tupletclass.Click += (sender, e) => {
                        perceptualtime.Value =
                            new PerceptualTime(
                                    tupletclass,
                                    perceptualtime.Value.Length,
                                    perceptualtime.Value.Dots
                                );
                    };

                    mnuPerceptualTime_tupletclasses.Add(tupletclass, mnuPerceptualTime_tupletclass);
                }))(_);

            foreach (int _ in Enumerable.Range(0, 3))
                (new Action<int>(dotsclass => {
                    var pretty = dotsclass.ToString() + (dotsclass == 1 ? "dot" : "dots");
                    var mnuPerceptualTime_dotsclass =
                        mnuPerceptualTime_dots.DropDownItems.Add(pretty) as ToolStripMenuItem;

                    mnuPerceptualTime_dotsclass.Tag = dotsclass;

                    mnuPerceptualTime_dotsclass.Click += (sender, e) => {
                        perceptualtime.Value =
                            new PerceptualTime(
                                    perceptualtime.Value.Tuplet,
                                    perceptualtime.Value.Length,
                                    dotsclass
                                );
                    };

                    mnuPerceptualTime_dotsclasses.Add(dotsclass, mnuPerceptualTime_dotsclass);
                }))(_);
        }
    }
}
