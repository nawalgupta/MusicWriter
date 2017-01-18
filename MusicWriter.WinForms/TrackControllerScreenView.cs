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
using System.Collections;

namespace MusicWriter.WinForms {
    //TODO: if you want to use the visual studio designer with this control,
    // (1) change base type from TabPage to UserControl,
    // (2) resolve any build errors by commenting out or deleting code,
    // (3) make sure to change it back before committing the code.
    public partial class TrackControllerScreenView : UserControl {
        EditorFile<Control> file;
        TrackControllerScreen<Control> screen;

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

        public TrackControllerScreen<Control> Screen {
            get { return screen; }
            set {
                if (screen != null) {
                    screen.Name.AfterChange -= Screen_NameChanged;
                    screen.Controllers.ItemAdded -= ScreenControllers_ItemAdded;
                    screen.Controllers.ItemRemoved -= ScreenControllers_ItemRemoved;
                }

                var old = screen;

                screen = value;
                Screen_NameChanged(old?.Name.Value, screen.Name.Value);
                screen.Name.AfterChange += Screen_NameChanged;

                screen.Controllers.ItemAdded += ScreenControllers_ItemAdded;
                screen.Controllers.ItemRemoved += ScreenControllers_ItemRemoved;
            }
        }

        public EditorFile<Control> File {
            get { return file; }
            set {
                if (file != null) {
                    file.Tracks.ItemAdded -= Tracks_ItemAdded;
                    file.Tracks.ItemRemoved -= Tracks_ItemRemoved;

                    file.Controllers.ItemAdded -= FileControllers_ItemAdded;
                    file.Controllers.ItemRemoved -= FileControllers_ItemRemoved;

                    file.Capabilities.ControllerFactories.CollectionChanged -= ControllerFactories_CollectionChanged;
                    file.Capabilities.TrackFactories.CollectionChanged -= TrackFactories_CollectionChanged;
                }

                var old = file;

                file = value;

                file.Tracks.ItemAdded += Tracks_ItemAdded;
                file.Tracks.ItemRemoved += Tracks_ItemRemoved;

                file.Controllers.ItemAdded += FileControllers_ItemAdded;
                file.Controllers.ItemRemoved += FileControllers_ItemRemoved;

                file.Capabilities.ControllerFactories.CollectionChanged += ControllerFactories_CollectionChanged;
                ControllerFactories_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, file.Capabilities.ControllerFactories, (IList<ITrackControllerFactory<Control>>)old?.Capabilities.ControllerFactories ?? new List<ITrackControllerFactory<Control>>()));

                file.Capabilities.TrackFactories.CollectionChanged += TrackFactories_CollectionChanged;
                TrackFactories_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, file.Capabilities.TrackFactories, (IList<ITrackFactory>)old?.Capabilities.TrackFactories ?? new List<ITrackFactory>()));
            }
        }

        public TrackControllerScreenView() {
            InitializeComponent();
        }

        private void ControllerName_AfterChange(string old, string @new) {
            Invoke(new Action(() => {
                var item = lsvControllers.Items[$"lsvControllersItem_{old}"];
                item.Name = $"lsvControllersItem_{@new}";

                var controller = file.GetController(@new);
                controller.View.Text = controller.Name.Value;
            }));
        }

        private void FileControllers_ItemAdded(ITrackController<Control> controller) {
            var item =
                new ListViewItem();

            item.Text = controller.Name.Value;
            item.Name = $"lsvControllersItem_{controller.Name}";
            item.Tag = controller;
            item.Checked = true;

            controller.View.Dock = DockStyle.Top;
            controller.View.Text = controller.Name.Value;

            lsvControllers.Items.Add(item);
            
            //pnlViews.Controls.Add(controller.View);

            //pnlViews.Controls.SetChildIndex(controller.View, ?);

            //controller.View.Focus();

            controller.Name.AfterChange += ControllerName_AfterChange;

            controller.View.GotFocus += View_GotFocus;
            controller.View.LostFocus += View_LostFocus;
            //controller.View.ParentChanged += View_Dispose;
            controller.View.Disposed += View_Dispose;
        }

        private void FileControllers_ItemRemoved(ITrackController<Control> controller) {
            lsvControllers.Items.RemoveByKey($"lsvControllersItem_{controller.Name}");
        }

        private void ControllerFactories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            mnuAddController.Items.Clear();

            foreach (var controllerfactory in file.Capabilities.ControllerFactories)
                mnuAddController.Items.Add(new ToolStripMenuItem {
                    Text = controllerfactory.Name,
                    Tag = controllerfactory
                });
        }

        private void TrackFactories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            mnuAddTrack.Items.Clear();

            foreach (var trackfactory in file.Capabilities.TrackFactories)
                mnuAddTrack.Items.Add(new ToolStripMenuItem {
                    Text = trackfactory.Name,
                    Tag = trackfactory
                });
        }

        private void Screen_NameChanged(string old, string @new) {
            Text = @new;
            Name = $"tabScreen_{@new}";
        }
        
        private void Tracks_ItemAdded(ITrack track) {
            var item =
                new ListViewItem();

            track.Name.AfterChange += Tracks_NameChanged;
            track.Length.AfterChange += Tracks_LengthChanged;
            Tracks_LengthChanged(Time.Zero, track.Length.Value);

            item.Tag = track;
            item.Text = track.Name.Value;
            item.Name = $"lsvTracksItem_{track.Name}";

            lsvTracks.Items.Add(item);
        }
        
        private void Tracks_ItemRemoved(ITrack track) {
            track.Name.AfterChange -= Tracks_NameChanged;
            track.Length.AfterChange -= Tracks_LengthChanged;

            lsvTracks.Items.RemoveByKey($"lsvTracksItem_{track.Name}");
        }

        private void Tracks_LengthChanged(Time old, Time @new) {
            var longestlength =
                file.Tracks.Max(track => track.Length.Value);

            sclOffset.Maximum = longestlength.Ticks;
        }

        private void Tracks_NameChanged(string old, string @new) {
            Invoke(new Action(() => {
                var item = lsvTracks.Items[$"lsvTracksItem_{old}"];
                item.Name = $"lsvTracksItem_{@new}";
            }));
        }
        
        private void ScreenControllers_ItemAdded(ITrackController<Control> controller) {
            var item = lsvControllers.Items[$"lsvControllersItem_{controller.Name}"];

            pnlViews.Controls.Add(controller.View);
            controller.View.Invalidate(true);
            controller.View.Focus();

            item.Checked = true;
        }

        private void ScreenControllers_ItemRemoved(ITrackController<Control> controller) {
            var item = lsvControllers.Items[$"lsvControllersItem_{controller.Name}"];
            item.Checked = false;

            pnlViews.Controls.Remove(controller.View);

            controller.CommandCenter.DesubscribeFrom(screen.CommandCenter);
        }

        private void View_Dispose(object sender, EventArgs e) {
            var ctrl = sender as Control;

            ctrl.Disposed -= View_Dispose;
            //ctrl.ParentChanged -= View_Dispose;
            ctrl.GotFocus -= View_GotFocus;
            ctrl.LostFocus -= View_LostFocus;
        }
        
        private void View_LostFocus(object sender, EventArgs e) {
            var controllername = (sender as Control).Text;
            var controller = file.GetController(controllername);
            controller.CommandCenter.Enabled = false;

            foreach (var track in controller.Tracks) {
                track.Length.AfterChange -= Tracks_LengthChanged;
            }

            controller.Pin.Time.ActualTime.AfterChange -= ActiveController_PinMoved;
        }

        private void View_GotFocus(object sender, EventArgs e) {
            var controllername = (sender as Control).Text;
            var controller = file.GetController(controllername);
            controller.CommandCenter.Enabled = true;

            var item = lsvControllers.Items[$"lsvControllersItem_{controllername}"];

            item.Selected = true;

            foreach (var track in controller.Tracks) {
                track.Length.AfterChange += Tracks_LengthChanged;
            }

            controller.Pin.Time.ActualTime.AfterChange += ActiveController_PinMoved;
        }

        private void ActiveController_PinMoved(Time old, Time @new) {
            var word = SelectedController.Hints.WordSize(@new);
            var unit = SelectedController.Hints.UnitSize(@new);

            sclOffset.LargeChange = word.Ticks;
            sclOffset.SmallChange = unit.Ticks;
        }

        private void lsvControllers_SelectedIndexChanged(object sender, EventArgs e) {
            if (lsvControllers.SelectedIndices.Count == 0)
                return;

            lsvTracks_disabled = true;

            foreach (ListViewItem item in lsvTracks.Items) {
                var track = (ITrack)item.Tag;

                item.Checked = SelectedController.Tracks.Contains(track);
            }

            for (int i = 0; i < lsvControllers.Items.Count; i++) {
                var selected =
                    lsvControllers.SelectedIndices.Contains(i) &&
                    screen.Controllers.Contains(lsvControllers.Items[i].Tag as ITrackController<Control>);

                var item =
                    lsvControllers.Items[i];

                var controller = file.GetController(item.Text);

                controller.CommandCenter.Enabled = selected;

                if (selected) {
                    controller.View.Focus();

                    sclOffset.Value = controller.Pin.Time.Offset.Value.Ticks;
                }
            }

            lsvTracks_disabled = false;

            Invalidate(true);
        }

        private void txtSearchControllers_TextChanged(object sender, EventArgs e) {

        }

        private void btnDeleteController_Click(object sender, EventArgs e) {
            var controller = SelectedController;

            if (controller == null)
                MessageBox.Show(this, "Select a view to delet it.", "Cannot delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (MessageBox.Show(this, "Delete this view?", "Delete?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) == DialogResult.Yes) {
                screen.Controllers.Remove(controller);
            }
        }

        private void btnAddController_Click(object sender, EventArgs e) {
            mnuAddController.Show(btnAddView, new Point(btnAddView.Width, btnAddView.Height), ToolStripDropDownDirection.BelowLeft);
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

                file.Tracks.Remove(track);
            }
        }

        private void btnAddTrack_Click(object sender, EventArgs e) {
            mnuAddTrack.Show(btnAddTrack, new Point(btnAddTrack.Width, btnAddTrack.Height), ToolStripDropDownDirection.BelowLeft);
        }

        private void mnuAddController_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            var controllerfactory =
                (ITrackControllerFactory<Control>)
                e.ClickedItem.Tag;
            
            file.CreateTrackController(controllerfactory.Name);
        }

        private void mnuAddTrack_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            var trackfactory =
                (ITrackFactory)
                e.ClickedItem.Tag;
            
            file.CreateTrack(trackfactory.Name);
        }

        bool lsvTracks_disabled = false;
        private void lsvTracks_ItemChecked(object sender, ItemCheckedEventArgs e) {
            if (lsvTracks_disabled)
                return;

            var track =
                (ITrack)
                e.Item.Tag;

            if (SelectedController != null) {
                if (e.Item.Checked)
                    SelectedController.Tracks.Add(track);
                else {
                    SelectedController.Tracks.Remove(track);
                }
            }
            else {
                //MessageBox.Show(
                //        owner: FindForm(),
                //        text: "Select a controller first to select its tracks.",
                //        caption: "Select controller",
                //        buttons: MessageBoxButtons.OK,
                //        icon: MessageBoxIcon.Hand
                //    );
            }
        }

        private void lsvTracks_AfterLabelEdit(object sender, LabelEditEventArgs e) {
            var track =
                lsvTracks.Items[e.Item].Tag as ITrack;

            if (e.Label != null)
                track.Name.Value = e.Label;
        }

        private void lsvControllers_AfterLabelEdit(object sender, LabelEditEventArgs e) {
            var controller =
                lsvControllers.Items[e.Item].Tag as ITrackController<Control>;

            if (e.Label != null)
                controller.Name.Value = e.Label;
        }

        private void lsvControllers_ItemChecked(object sender, ItemCheckedEventArgs e) {
            var controller =
                e.Item.Tag as ITrackController<Control>;

            if (e.Item.Checked)
                Screen.Controllers.Add(controller);
            else {
                Screen.Controllers.Remove(controller);
            }
        }

        private void spltMasterDetail_SplitterMoved(object sender, SplitterEventArgs e) {
            clmTrackName.Width = spltMasterDetail.Panel1.Width - 4;
            clmViewName.Width = spltMasterDetail.Panel1.Width - 4;
        }

        private void sclOffset_Scroll(object sender, ScrollEventArgs e) {
            SelectedController.Pin.Time.Offset.Value += Time.FromTicks(e.NewValue - e.OldValue);
        }
    }
}
