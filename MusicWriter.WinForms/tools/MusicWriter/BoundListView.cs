using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms
{
    public partial class BoundListView : UserControl
    {
        public event Action<StorageObjectID> ItemSelected;
        public event Action<StorageObjectID> ItemChecked;
        public event Action<StorageObjectID> ItemUnchecked;

        public bool CheckBoxes {
            get { return lsvItems.CheckBoxes; }
            set { lsvItems.CheckBoxes = value; }
        }

        public BoundListView() {
            InitializeComponent();
        }

        private void lsvItems_SelectedIndexChanged(object sender, EventArgs e) {
            if (lsvItems.SelectedItems.Count > 0)
                ItemSelected?.Invoke((StorageObjectID)lsvItems.SelectedItems[0].Tag);
        }

        private void lsvItems_ItemChecked(object sender, ItemCheckedEventArgs e) {
            var objID = (StorageObjectID)e.Item.Tag;
            if (e.Item.Checked)
                ItemChecked?.Invoke(objID);
            else
                ItemUnchecked?.Invoke(objID);
        }

        private class ItemInfo
        {
            public ListViewItem ListViewItem;
            public ObservableProperty<string>.PropertySetHandler NameSetResponder;
        }

        readonly Dictionary<StorageObjectID, ItemInfo> itemmap =
            new Dictionary<StorageObjectID, ItemInfo>();

        public void InsertItem<T>(T item, int i)
            where T : INamedObject, IBoundObject<T> {
            var info = new ItemInfo();
            itemmap.Add(item.StorageObjectID, info);

            info.ListViewItem = lsvItems.Items.Insert(i, "");
            info.NameSetResponder = name => {
                info.ListViewItem.Text = name;
            };
            item.Name.Set += info.NameSetResponder;
            info.ListViewItem.Tag = item.StorageObjectID;
        }
        
        public void MoveItem<T>(T item, int oldindex, int newindex)
            where T : INamedObject, IBoundObject<T>  {
            var lsvItem = itemmap[item.StorageObjectID].ListViewItem;
            if (lsvItem.Index != oldindex)
                throw new InvalidOperationException();

            lsvItems.Items.Remove(lsvItem);
            lsvItems.Items.Insert(newindex, lsvItem);
        }

        public void WithdrawItem<T>(T item, int i)
            where T : INamedObject, IBoundObject<T>  {
            if (itemmap[item.StorageObjectID].ListViewItem.Index != i)
                throw new InvalidOperationException();
            
            itemmap[item.StorageObjectID].ListViewItem.Remove();
            itemmap.Remove(item.StorageObjectID);
        }

        public abstract class Binder<T>
            where T : INamedObject, IBoundObject<T>
        {
            readonly BoundListView listview;
            readonly BoundList<T> boundlist;

            public BoundListView ListView {
                get { return listview; }
            }

            public BoundList<T> BoundList {
                get { return boundlist; }
            }

            public Binder(
                    BoundListView listview,
                    BoundList<T> boundlist
                ) {
                this.listview = listview;
                this.boundlist = boundlist;
            }

            public abstract void Bind();
            public abstract void Unbind();
        }

        public sealed class ItemBinder<T>
            : Binder<T>
            where T : INamedObject, IBoundObject<T>
        {
            public ItemBinder(
                    BoundListView listview,
                    BoundList<T> boundlist
                ) :
                base(
                        listview,
                        boundlist
                    ) {
            }

            public override void Bind() {
                BoundList.ItemInserted += ListView.InsertItem;
                BoundList.ItemWithdrawn += ListView.WithdrawItem;
                BoundList.ItemMoved += ListView.MoveItem;
            }

            public override void Unbind() {
                BoundList.ItemInserted -= ListView.InsertItem;
                BoundList.ItemWithdrawn -= ListView.WithdrawItem;
                BoundList.ItemMoved -= ListView.MoveItem;
            }
        }

        public sealed class SelectorBinder<T>
            : Binder<T>
            where T : INamedObject, IBoundObject<T>
        {
            readonly Selector<T> selector;

            public Selector<T> Selector {
                get { return selector; }
            }

            public SelectorBinder(
                    BoundListView listview,
                    BoundList<T> boundlist,
                    Selector<T> selector
                ) :
                base(
                        listview,
                        boundlist
                    ) {
                this.selector = selector;
            }

            public override void Bind() {
                ListView.ItemSelected += ListView_ItemSelected;
            }

            public override void Unbind() {
                ListView.ItemSelected -= ListView_ItemSelected;
            }

            private void ListView_ItemSelected(StorageObjectID objID) {
                selector.Active.Value = BoundList[objID];
            }
        }
    }
}
