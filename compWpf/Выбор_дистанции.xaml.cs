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
    /// Логика взаимодействия для Выбор_дистанции.xaml
    /// </summary>
    public partial class Выбор_дистанции : Window
    {
        public Выбор_дистанции()
        {
            InitializeComponent();
        }
        Entities de = new Entities();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (de.дистанции.Any(n => n.слет == клСлет.слет))
            {
                foreach (виды vRow in de.виды.OrderBy(n => n.порядок))
                {
                    //   treeView1.Items.Add(vRow.наимен);
                    TreeViewItem item = new TreeViewItem();
                    item.Tag = vRow;
                    item.Header = vRow.наимен;
                    //      item.Style = style;
                    //      item.Items.Add("*");

                    treeView1.Items.Add(item);
                    foreach (дистанции dRow in vRow.дистанции.Where(n => n.слет == клСлет.слет).OrderBy(n => n.порядок))
                    {
                        TreeViewItem item2 = new TreeViewItem();
                        item2.Tag = dRow;
                        item2.Header = dRow.наимен + " " + dRow.этапов.ToString();
                        item.Items.Add(item2);
                        //     item.Selected += Item_Selected;
                        if (dRow.дистанция == клДистанция.дистанция)
                        {
                            item2.IsSelected = true;
                            TreeViewItem item0 = (TreeViewItem)item2.Parent;
                            item0.IsExpanded = true;
                            выбор.IsEnabled = true;
                        }
                    }
                }
                treeView1.Focus();

                if (treeView1.Items.Count > 0 && treeView1.SelectedItem == null)
                {
                    TreeViewItem item0 = (TreeViewItem)treeView1.Items[0];
                    item0.IsSelected = true;

                }
            }
            else
            {
                MessageBox.Show("Предварительно заполните  список дистанций на слете");
                Close();
            }
            treeView1.SelectedItemChanged += TreeView1_SelectedItemChanged;
        }

        private void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            выбор.IsEnabled = false;
            if (treeView1.SelectedItem != null)
            {
                TreeViewItem node = (TreeViewItem)treeView1.SelectedItem;
                if(node.Tag is дистанции)
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


            Close();
        }

        private void отмена_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }
    }
}
