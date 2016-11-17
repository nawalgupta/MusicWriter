using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace MusicWriter.WinForms {
    public partial class ScreenView : TabPage {
        EditorFile file;
        Screen<Control> screen;

        public ITrackController<Control> SelectedController {
            get {
                if (lsvControllers.SelectedItems.Count == 1)
                    return lsvControllers.SelectedItems[0]?.Tag as ITrackController<Control>;

                return null;
            }
        }

        public ITrack SelectedTrack {
            get {
                if (lsvTracks.SelectedItems.Count == 0)
                    return null;

                return lsvTracks.SelectedItems[0].Tag as ITrack;
            }
        }

        public Screen<Control> Screen {
            get { return screen; }
            set {
                if (screen != null) {
                    screen.Name.AfterChange -= Name_AfterChange;
                    screen.Controllers.CollectionChanged -= Controllers_CollectionChanged;

                    screen.Capabilities.ControllerFactories.CollectionChanged -= ControllerFactories_CollectionChanged;
                    screen.Capabilities.TrackFactories.CollectionChanged -= TrackFactories_CollectionChanged;
                }

                var old = screen;

                screen = value;
                Name_AfterChange(old?.Name.Value, screen.Name.Value);
                screen.Name.AfterChange += Name_AfterChange;

                screen.Controllers.CollectionChanged += Controllers_CollectionChanged;
                Controllers_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, screen.Controllers, (IList<ITrackController<Control>>)old?.Controllers ?? new List<ITrackController<Control>>(), 0));

                screen.Capabilities.ControllerFactories.CollectionChanged += ControllerFactories_CollectionChanged;
                ControllerFactories_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, screen.Capabilities.ControllerFactories, (IList<ITrackControllerFactory<Control>>)old?.Capabilities.ControllerFactories ?? new List<ITrackControllerFactory<Control>>()));

                screen.Capabilities.TrackFactories.CollectionChanged += TrackFactories_CollectionChanged;
                TrackFactories_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, screen.Capabilities.TrackFactories, (IList<ITrackFactory>)old?.Capabilities.TrackFactories ?? new List<ITrackFactory>()));
            }
        }

        public EditorFile File {
            get { return file; }
            set {
                if (file != null)
                    file.Tracks.CollectionChanged -= Tracks_CollectionChanged;

                var old = file;

                file = value;
                Tracks_CollectionChanged(file, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, file.Tracks, (IList<ITrack>)old?.Tracks ?? new List<ITrack>(), 0));
                file.Tracks.CollectionChanged += Tracks_CollectionChanged;
            }
        }

        public ScreenView() {
            InitializeComponent();
        }

        private void ControllerFactories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            mnuAddView.Items.Clear();

            foreach (var controllerfactory in screen.Capabilities.ControllerFactories)
                mnuAddView.Items.Add(new ToolStripMenuItem {
                    Text = controllerfactory.Name,
                    Tag = controllerfactory
                });
        }

        private void TrackFactories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            mnuAddTrack.Items.Clear();

            foreach (var trackfactory in screen.Capabilities.TrackFactories)
                mnuAddTrack.Items.Add(new ToolStripMenuItem {
                    Text = trackfactory.Name,
                    Tag = trackfactory
                });
        }

        private void Name_AfterChange(string old, string @new) {
            Text = @new;
            Name = $"tabScreen_{@new}";
        }

        private void Tracks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.OldItems != null)
                for (var i = 0; i < e.OldItems.Count; i++)
                    if (lsvTracks.Items.Count > i)
                        lsvTracks.Items.RemoveAt(e.OldStartingIndex + i);

            for (var i = 0; i < e.NewItems.Count; i++) {
                var value =
                    e.NewItems[i] as ITrack;

                if (value != null) {
                    var item =
                        new ListViewItem();

                    item.Text = value.Name.Value;
                    item.Tag = value;

                    lsvTracks.Items.Insert(e.NewStartingIndex + i, item);
                }
            }
        }

        private void Controllers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.OldItems != null) {
                for (var i = 0; i < e.OldItems.Count; i++) {
                    if (lsvControllers.Items.Count > i)
                        lsvControllers.Items.RemoveAt(e.OldStartingIndex + i);

                    if (pnlViews.Controls.Count > i)
                        pnlViews.Controls.RemoveAt(e.OldStartingIndex + i);
                }
            }

            if (e.NewItems != null) {
                for (var i = 0; i < e.NewItems.Count; i++) {
                    var value =
                        e.NewItems[i] as ITrackController<Control>;

                    if (value != null) {
                        var item =
                            new ListViewItem();

                        item.Text = value.Name.Value;
                        item.Tag = value;
                        
                        value.View.Dock = DockStyle.Top;

                        lsvControllers.Items.Insert(e.NewStartingIndex + i, item);
                        pnlViews.Controls.Add(value.View);
                        pnlViews.Controls.SetChildIndex(value.View, e.NewStartingIndex + i);
                    }
                }
            }
        }

        private void lsvViews_SelectedIndexChanged(object sender, EventArgs e) {
            lsvTracks_disabled = true;

            foreach (ListViewItem item in lsvTracks.Items) {
                var track = (ITrack)item.Tag;

                item.Checked = SelectedController.Tracks.Contains(track);
            }

            lsvTracks_disabled = false;
        }

        private void txtSearchViews_TextChanged(object sender, EventArgs e) {

        }

        private void btnDeleteView_Click(object sender, EventArgs e) {
            var controller = SelectedController;

            if (controller == null)
                MessageBox.Show(this, "Select a view to delet it.", "Cannot delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (MessageBox.Show(this, "Delete this view?", "Delete?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) == DialogResult.Yes) {
                screen.Controllers.Remove(controller);
            }
        }

        private void btnAddView_Click(object sender, EventArgs e) {
            mnuAddView.Show(btnAddView, new Point(btnAddView.Width, btnAddView.Height), ToolStripDropDownDirection.BelowRight);
        }
        
        private void txtSearchTracks_TextChanged(object sender, EventArgs e) {

        }

        private void btnDeleteTrack_Click(object sender, EventArgs e) {
            var track = SelectedTrack;

            if (track == null)
                MessageBox.Show(this, "Select a track to delete it.", "Cannot delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (MessageBox.Show(this, "Delete this track?", "Delete?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) == DialogResult.Yes) {
                foreach (var controller in screen.Controllers)
                    controller.Tracks.Remove(track);

                file.Remove(track);
            }
        }

        private void btnAddTrack_Click(object sender, EventArgs e) {
            mnuAddTrack.Show(btnAddTrack, new Point(btnAddTrack.Width, btnAddTrack.Height), ToolStripDropDownDirection.BelowLeft);
        }

        private void mnuAddView_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            var controllerfactory =
                (ITrackControllerFactory<Control>)
                e.ClickedItem.Tag;

            var newcontroller =
                controllerfactory.Create();
            
            screen.Controllers.Add(newcontroller);
        }

        private void mnuAddTrack_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            var trackfactory =
                (ITrackFactory)
                e.ClickedItem.Tag;

            var newtrack =
                trackfactory.Create();

            File.Add(newtrack);
        }

        bool lsvTracks_disabled = false;
        private void lsvTracks_ItemChecked(object sender, ItemCheckedEventArgs e) {
            if (lsvTracks_disabled)
                return;

            var track =
                (ITrack)
                e.Item.Tag;

            if (e.Item.Checked)
                SelectedController?.Tracks.Add(track);
            else {
                SelectedController?.Tracks.Remove(track);
            }
        }
    }
}
