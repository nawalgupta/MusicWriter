using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicWriter.WinForms
{
    public sealed class FactoryMenuStrip<T> where T : IBoundObject<T>
    {
        BoundList<T> boundlist;
        
        public BoundList<T> BoundList {
            get { return boundlist; }
            set {
                if (boundlist != null) {
                    boundlist.FactorySet.Factories.ItemInserted -= Factories_ItemInserted;
                    boundlist.FactorySet.Factories.ItemWithdrawn -= Factories_ItemWithdrawn;

                    ToolStripDropDown.Items.Clear();
                }

                boundlist = value;

                boundlist.FactorySet.Factories.ItemInserted += Factories_ItemInserted;
                boundlist.FactorySet.Factories.ItemWithdrawn += Factories_ItemWithdrawn;
            }
        }

        public ToolStripDropDown ToolStripDropDown { get; set; }
        
        private void Factories_ItemInserted(IFactory<T> factory, int i) {
            var item =
                new ToolStripMenuItem();

            item.Text = factory.Name;
            item.Tag = factory;
            item.Click += Item_Click;

            ToolStripDropDown.Items.Insert(i, item);
        }

        private void Factories_ItemWithdrawn(IFactory<T> factory, int i) {
            ToolStripDropDown.Items.RemoveAt(i);
        }

        private void Item_Click(object sender, EventArgs e) {
            var item =
                (ToolStripMenuItem)sender;

            var factory =
                item.Tag as IFactory<T>;

            boundlist.Create(factory.Name);
        }
    }
}
