using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms {
    public partial class SheetMusicEditor :
        Control,
        ITrackController<Control> {
        public float ScrollX { get; set; } = 0;
        public Caret Caret { get; set; }
        public MusicBrain Brain { get; set; }
        public SheetMusicRenderSettings Settings { get; set; } = new SheetMusicRenderSettings();
        ObservableProperty<string> ITrackController<Control>.Name { get; } =
            new ObservableProperty<string>("abc");

        readonly ConverterList<ITrack, MusicTrack> tracks =
            new ConverterList<ITrack, MusicTrack>();
        
        public IList<ITrack> Tracks {
            get { return tracks.RegularCollection; }
        }

        public Control View {
            get { return this; }
        }

        public void LoadFrom(EditorFile file, string track) {
            Settings = new SheetMusicRenderSettings();
            
            Invalidate();
        }
        
        public SheetMusicEditor() {
            InitializeComponent();

            tracks.SpecialCollection.CollectionChanged += Tracks_CollectionChanged;
        }

        private void Tracks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.OldItems != null)
                foreach (MusicTrack track in e.OldItems)
                    track.Memory.RemoveMemoryModule<RenderedSheetMusicItem>();

            if (e.NewItems != null)
                foreach (MusicTrack track in e.NewItems)
                    track.Memory.InsertMemoryModule(new RenderedSheetMusicItemPerceptualCog.MemoryModule());
            
            Invalidate();
        }

        protected override void OnInvalidated(InvalidateEventArgs e) {
            Height = (int)(Settings.Height * tracks.Count);

            base.OnInvalidated(e);
        }

        protected override void OnPaint(PaintEventArgs pe) {
            foreach (var track in tracks.SpecialCollection) {
                // draw staff
                if (Settings?.Staff != null)
                    for (int line = 0; line < Settings.Staff.Lines; line++)
                        pe.Graphics.DrawLine(
                                Pens.Black,
                                0,
                                Settings.YVal(line * 2),
                                Width,
                                Settings.YVal(line * 2)
                            );

                // draw sheet items
                var rendered = new List<RenderedSheetMusicItem>();

                var starttimes =
                    track
                        .Memory
                        .Analyses<RenderedSheetMusicItem>(Duration.Eternity)
                        .Select(item => item.Duration)
                        .Select(duration => duration.Start)
                        .Distinct()
                        .ToList();

                starttimes.Sort();

                if (starttimes.Count != 0) {
                    var focusstarttime =
                        starttimes[0];

                    var focusitems = new List<RenderedSheetMusicItem>();

                    var x = -ScrollX;

                    while (x < Width) {
                        if (focusitems.Count == 0) {
                            if (starttimes.Count == 0)
                                break;

                            focusstarttime = starttimes[0];
                            starttimes.RemoveAt(0);

                            focusitems
                                .AddRange(
                                        track
                                            .Memory
                                            .Analyses<RenderedSheetMusicItem>(focusstarttime)
                                            .Select(focusitem => focusitem.Value)
                                    );

                            focusitems.Sort((a, b) => a.Priority.CompareTo(b.Priority));
                        }

                        var item = focusitems[0];
                        focusitems.RemoveAt(0);

                        var width = item.Width(Settings);
                        if (x + width > 0)
                            pe.Graphics.DrawImageUnscaled(item.Draw(Settings), (int)x, 0);

                        x += width;
                    }
                }

                pe.Graphics.TranslateTransform(0, Settings.Height);
            }

            base.OnPaint(pe);
        }

    }
}
