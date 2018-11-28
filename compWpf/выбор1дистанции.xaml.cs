using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace compWpf
{
    /// <summary>
    /// Логика взаимодействия для выбор1дистанции.xaml
    /// </summary>
    public partial class выбор1дистанции : Window
    {
        public выбор1дистанции()
        {
            InitializeComponent();
        }
        Entities de = new Entities();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //foreach (виды vRow in de.виды.Where(n=>n.вид==клВид.вид).OrderBy(n => n.порядок))
            //{
            //    //   treeView1.Items.Add(vRow.наимен);
            //    TreeViewItem item = new TreeViewItem();
            //    item.Tag = vRow;
            //    item.Header = vRow.наимен;
            //    //      item.Style = style;
            //    //      item.Items.Add("*");

            //    treeView1.Items.Add(item);
                foreach (дистанции dRow in de.дистанции.Where(n => n.слет == клСлет.слет).Where(n=>n.вид==клВид.вид).OrderBy(n => n.порядок))
                {
                    TreeViewItem item2 = new TreeViewItem();
                    item2.Tag = dRow;
                    item2.Header = dRow.наимен + " " + dRow.этапов.ToString();
                    treeView1.Items.Add(item2);
                    //     item.Selected += Item_Selected;
                    if (dRow.дистанция == клДистанция.дистанция)
                    {
                        item2.IsSelected = true;
                        //TreeViewItem item0 = (TreeViewItem)item2.Parent;
                        //item0.IsExpanded = true;
                        выбор.IsEnabled = true;
                    }
                }
            //}
            treeView1.Focus();

            if (treeView1.Items.Count > 0 && treeView1.SelectedItem == null)
            {
                TreeViewItem item0 = (TreeViewItem)treeView1.Items[0];
                item0.IsSelected = true;

            }
          //  treeView1.SelectedItemChanged += TreeView1_SelectedItemChanged;
        }

        private void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            выбор.IsEnabled = false;
            if (treeView1.SelectedItem != null)
            {
                TreeViewItem node = (TreeViewItem)treeView1.SelectedItem;
                if (node.Tag is дистанции)
                {
                    выбор.IsEnabled = true;
                }
            }
        }

        private void выбор_Click(object sender, RoutedEventArgs e)
        {
            if (treeView1.Items.Count > 0 && treeView1.SelectedItem != null)
            {
                TreeViewItem node = (TreeViewItem)treeView1.SelectedItem;
                if (node.Tag is дистанции)
                {

                    клДистанция.deRow = (дистанции)node.Tag;
                    клДистанция.дистанция = клДистанция.deRow.дистанция;
                    клДистанция.наимен = клДистанция.deRow.наимен;

                    клДистанция.выбран = true;
                    this.DialogResult = true;
                }

                //TreeViewItem item0 = (TreeViewItem)treeView1.Items[0];
                //item0.IsSelected = true;
            }

       //     this.DialogResult = true;
            Close();
        }

        private void отмена_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }
    }
}
