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
    public partial class ScreenView : TabPage {
        EditorFile<Control> file;
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
                    screen.Controllers.CollectionChanged -= ScreenControllers_CollectionChanged;
                }

                var old = screen;

                screen = value;
                Name_AfterChange(old?.Name.Value, screen.Name.Value);
                screen.Name.AfterChange += Name_AfterChange;

                screen.Controllers.CollectionChanged += ScreenControllers_CollectionChanged;
                ScreenControllers_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, (IList)screen.Controllers, (IList<ITrackController<Control>>)old?.Controllers ?? new List<ITrackController<Control>>(), 0));
            }
        }

        public EditorFile<Control> File {
            get { return file; }
            set {
                if (file != null) {
                    file.Tracks.CollectionChanged -= Tracks_CollectionChanged;
                    
                    file.Capabilities.ControllerFactories.CollectionChanged -= ControllerFactories_CollectionChanged;
                    file.Capabilities.TrackFactories.CollectionChanged -= TrackFactories_CollectionChanged;
                }

                var old = file;

                file = value;

                Tracks_CollectionChanged(file, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, file.Tracks, (IList<ITrack>)old?.Tracks ?? new List<ITrack>(), 0));
                file.Tracks.CollectionChanged += Tracks_CollectionChanged;

                ScreenControllers_CollectionChanged(file, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, file.Controllers, (IList<ITrackController<Control>>)old?.Controllers ?? new List<ITrackController<Control>>(), 0));
                file.Controllers.CollectionChanged += FileControllers_CollectionChanged;

                file.Capabilities.ControllerFactories.CollectionChanged += ControllerFactories_CollectionChanged;
                ControllerFactories_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, file.Capabilities.ControllerFactories, (IList<ITrackControllerFactory<Control>>)old?.Capabilities.ControllerFactories ?? new List<ITrackControllerFactory<Control>>()));

                file.Capabilities.TrackFactories.CollectionChanged += TrackFactories_CollectionChanged;
                TrackFactories_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, file.Capabilities.TrackFactories, (IList<ITrackFactory>)old?.Capabilities.TrackFactories ?? new List<ITrackFactory>()));
            }
        }

        public ScreenView() {
            InitializeComponent();
        }

        private void FileControllers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            var newitems = ExpandWierdArgs<ITrackController<Control>>(e.NewItems).ToArray();

            if (e.OldItems != null) {
                for (var i = 0; i < e.OldItems.Count; i++) {
                    if (lsvControllers.Items.Count > i)
                        lsvControllers.Items.RemoveAt(e.OldStartingIndex + i);

                    if (pnlViews.Controls.Count > i)
                        pnlViews.Controls.RemoveAt(e.OldStartingIndex + i);
                }
            }

            if (e.NewItems != null) {
                for (var i = 0; i < newitems.Length; i++) {
                    var value =
                        newitems[i] as ITrackController<Control>;

                    if (value != null) {
                        var item =
                            new ListViewItem();

                        item.Text = value.Name.Value;
                        item.Tag = value;
                        item.Checked = true;

                        value.View.Dock = DockStyle.Top;

                        lsvControllers.Items.Insert(e.NewStartingIndex + i, item);

                        pnlViews.Controls.Add(value.View);
                        pnlViews.Controls.SetChildIndex(value.View, e.NewStartingIndex + i);

                        //value.View.Focus();

                        value.View.GotFocus += View_GotFocus;
                        value.View.LostFocus += View_LostFocus;
                        value.View.ParentChanged += View_Dispose;
                        value.View.Disposed += View_Dispose;
                    }
                }
            }
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

        private void Name_AfterChange(string old, string @new) {
            Text = @new;
            Name = $"tabScreen_{@new}";
        }

        private void Tracks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            var newitems = ExpandWierdArgs<ITrack>(e.NewItems).ToArray();

            if (e.OldItems != null) {
                for (var i = 0; i < e.OldItems.Count; i++) {
                    if (lsvTracks.Items.Count > i) {
                        lsvTracks.Items.RemoveAt(e.OldStartingIndex + i);

                        var track = (ITrack)e.OldItems[i];
                        track.Name.AfterChange -= Tracks_NameChanged;
                        track.Length.AfterChange -= Tracks_LengthChanged;
                    }
                }
            }

            for (var i = 0; i < newitems.Length; i++) {
                var value =
                    newitems[i] as ITrack;

                if (value != null) {
                    var item =
                        new ListViewItem();

                    value.Name.AfterChange += Tracks_NameChanged;
                    value.Length.AfterChange += Tracks_LengthChanged;

                    item.Text = value.Name.Value;
                    item.Tag = value;

                    lsvTracks.Items.Insert(e.NewStartingIndex + i, item);
                }
            }
        }

        private void Tracks_LengthChanged(Time old, Time @new) {
            var longestlength =
                SelectedController.Tracks.Max(track => track.Length.Value);

            sclOffset.Maximum = longestlength.Ticks;
        }

        private void Tracks_NameChanged(string old, string @new) {
            Invoke(new Action(() => {
                for (int i = 0; i < lsvTracks.Items.Count; i++) {
                    if (lsvTracks.Items[i].Text == old)
                        lsvTracks.Items[i].Text = @new;
                }
            }));
        }

        IEnumerable<T> ExpandWierdArgs<T>(IList wierdlist) where T : class {
            if (wierdlist == null)
                yield break;

            foreach(var x in wierdlist) {
                var list =
                    x as IList<T>;

                var t =
                    x as T;

                if (t != null)
                    yield return t;

                if (list != null)
                    foreach (T t2 in list)
                        yield return t2;
            }
        }

        private void ScreenControllers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            var newitems = ExpandWierdArgs<ITrackController<Control>>(e.NewItems).ToArray();

            if (e.OldItems != null) {
                for (var i = 0; i < e.OldItems.Count; i++) {
                    if (lsvControllers.Items.Count > i)
                        lsvControllers.Items[e.OldStartingIndex + i].Checked = false;
                }
            }

            if (e.NewItems != null) {
                for (var i = 0; i < e.NewItems.Count; i++) {
                    if (lsvControllers.Items.Count > i)
                        lsvControllers.Items[e.NewStartingIndex + i].Checked = true;
                }
            }

            //if (e.NewItems.Count == lsvControllers.Items.Count &&
            //    e.NewItems.Count > 0) {// all items are fresh
            //    lsvControllers.Items[0].Selected = true;
            //    Invalidate(true);
            //}
        }

        private void View_Dispose(object sender, EventArgs e) {
            var ctrl = sender as Control;

            ctrl.Disposed -= View_Dispose;
            ctrl.ParentChanged -= View_Dispose;
            ctrl.GotFocus -= View_GotFocus;
        }

        private void View_LostFocus(object sender, EventArgs e) {
            var itemindex = pnlViews.Controls.GetChildIndex(sender as Control);

            var controller = screen.Controllers[itemindex];

            foreach (var track in controller.Tracks) {
                track.Length.AfterChange -= Tracks_LengthChanged;
            }

            controller.Pin.ActualTime.AfterChange -= ActiveController_PinMoved;
        }

        private void View_GotFocus(object sender, EventArgs e) {
            var itemindex = pnlViews.Controls.GetChildIndex(sender as Control);

            var item = lsvControllers.Items[itemindex];

            item.Selected = true;

            var controller = screen.Controllers[itemindex];

            foreach (var track in controller.Tracks) {
                track.Length.AfterChange += Tracks_LengthChanged;
            }

            controller.Pin.ActualTime.AfterChange += ActiveController_PinMoved;
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
                
                screen.Controllers[i].CommandCenter.Enabled = selected;

                if (selected) {
                    screen.Controllers[i].View.Focus();
                }
            }

            lsvTracks_disabled = false;
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

            var newcontroller =
                controllerfactory.Create(File);

            file.Controllers.Add(newcontroller);
            screen.Controllers.Add(newcontroller);
        }

        private void mnuAddTrack_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            var trackfactory =
                (ITrackFactory)
                e.ClickedItem.Tag;

            var newtrack =
                trackfactory.Create();

            var prefix = newtrack.Name.Value = "Track ";
            var i = 0;

            while (File.Tracks.Any(t => t.Name.Value == newtrack.Name.Value))
                newtrack.Name.Value = prefix + (++i).ToString();

            File.Tracks.Add(newtrack);
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

            controller.View.Visible = e.Item.Checked;

            pnlViews.Invalidate(true);
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
