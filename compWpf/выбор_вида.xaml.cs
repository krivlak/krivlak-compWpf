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
    /// Логика взаимодействия для выбор_вида.xaml
    /// </summary>
    public partial class выбор_вида : Window
    {
        Entities de = new Entities();
        public выбор_вида()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //var style = (Style)Application.Current.FindResource("WindowsTreeViewItemStyle");
            foreach (виды vRow in de.виды.OrderBy(n=>n.порядок))
            {
                //   treeView1.Items.Add(vRow.наимен);
                TreeViewItem item = new TreeViewItem();
                item.Tag = vRow;
                item.Header = vRow.наимен;
          //      item.Style = style;
          //      item.Items.Add("*");
          
                treeView1.Items.Add(item);
           //     item.Selected += Item_Selected;
                if(vRow.вид==клВид.вид)
                {
                    item.IsSelected = true;
                }
            }
            treeView1.Focus();
            if(treeView1.Items.Count>0 && treeView1.SelectedItem==null)
            {
                TreeViewItem item0 = (TreeViewItem)treeView1.Items[0];
                item0.IsSelected = true;
            }

        

            //foreach (var treeViewItem in treeView1.Items)
            //{
            //    treeviewitem.Style = style;
            //}
            //   treeView1.SelectedItemChanged += TreeView1_SelectedItemChanged;

        }

        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem node = (TreeViewItem) sender;


            клВид.deRow = (виды)node.Tag;
            клВид.вид = клВид.deRow.вид;
            клВид.наимен = клВид.deRow.наимен;
        }

        private void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem node = (TreeViewItem) e.OldValue;
            
         
            клВид.deRow = (виды) node.Tag;
            клВид.вид = клВид.deRow.вид;
            клВид.наимен = клВид.deRow.наимен;
        }

        private void выбор_Click(object sender, RoutedEventArgs e)
        {
            if (treeView1.Items.Count > 0 && treeView1.SelectedItem != null)
            {
                TreeViewItem node = (TreeViewItem) treeView1.SelectedItem;

                клВид.deRow = (виды)node.Tag;
                клВид.вид = клВид.deRow.вид;
                клВид.наимен = клВид.deRow.наимен;

                клВид.выбран = true;
                this.DialogResult = true;

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
