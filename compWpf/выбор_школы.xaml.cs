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
    /// Логика взаимодействия для выбор_школы.xaml
    /// </summary>
    public partial class выбор_школы : Window
    {
        public выбор_школы()
        {
            InitializeComponent();
        }
        Entities de = new Entities();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (школы vRow in de.школы.OrderBy(n => n.порядок))
            {
                TreeViewItem item = new TreeViewItem();
                item.Tag = vRow;
                item.Header = vRow.наимен;

                treeView1.Items.Add(item);
                if (vRow.школа == клШкола.школа)
                {
                    item.IsSelected = true;
                }
            }
            treeView1.Focus();
            if (treeView1.Items.Count > 0 && treeView1.SelectedItem == null)
            {
                TreeViewItem item0 = (TreeViewItem)treeView1.Items[0];
                item0.IsSelected = true;
            }
        }

   

        private void выбор_Click(object sender, RoutedEventArgs e)
        {
            if (treeView1.Items.Count > 0 && treeView1.SelectedItem != null)
            {
                TreeViewItem node = (TreeViewItem)treeView1.SelectedItem;

                клШкола.deRow = (школы)node.Tag;
                клШкола.школа = клШкола.deRow.школа;
                клШкола.наимен = клШкола.deRow.наимен;

                клШкола.выбран = true;
                this.DialogResult = true;
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
