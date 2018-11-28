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
    /// Логика взаимодействия для выборVтуристов.xaml
    /// </summary>
    public partial class выборVтуристов : Window
    {
        public выборVтуристов()
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
                foreach (туристы dRow in vRow.туристы.Where(n => n.слет == клСлет.слет).OrderBy(n => n.фамилия))
                {


                    TreeViewItem item2 = new TreeViewItem()
                    {
                        Header = new CheckBox()
                        {
                            Content = dRow.фамилия.Trim() + " " + dRow.имя.Trim()
                        },
                        Tag = dRow

                    };
                    //item2.Tag = dRow;
                    //item2.Header = dRow.фамилия.Trim() + " " + dRow.имя.Trim();
                    item.Items.Add(item2);
                    //     item.Selected += Item_Selected;
                    if (dRow.турист == клТурист.турист)
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
         //   treeView1.SelectedItemChanged += TreeView1_SelectedItemChanged;
        }

        private void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //выбор.IsEnabled = false;
            //if (treeView1.SelectedItem != null)
            //{
            //    TreeViewItem node = (TreeViewItem)treeView1.SelectedItem;
            //    if (node.Tag is туристы)
            //    {
            //        выбор.IsEnabled = true;
            //    }
            //}
        }

        private void выбор_Click(object sender, RoutedEventArgs e)
        {
            
            клТурист.turList = new List<туристы>();
            foreach (TreeViewItem tRow in treeView1.Items)
            {
                foreach (TreeViewItem rRow in tRow.Items)
                {
                  //  Console.WriteLine(rRow.Header);

                    if (rRow.Header is CheckBox)
                    {
                        
                        CheckBox check = (CheckBox)rRow.Header;
                        if(check.IsChecked==true)
                        {
                            туристы турОб = (туристы)rRow.Tag;
                            клТурист.turList.Add(турОб);
                        }

                        //MessageBox.Show(check.IsChecked.Value.ToString() + check.Content);
                    }
                }
            }

            //if (treeView1.Items.Count > 0 && treeView1.SelectedItem != null)
            //{
            //    TreeViewItem node = (TreeViewItem)treeView1.SelectedItem;
            //    if (node.Tag is туристы)
            //    {

            //        клТурист.deRow = (туристы)node.Tag;
            //        клТурист.турист = клТурист.deRow.турист;
            //        клТурист.фамилия = клТурист.deRow.фамилия;

            //        клТурист.выбран = true;
            //        this.DialogResult = true;
            //    }


            //}

            клТурист.выбран = true;
            this.DialogResult = true;
            Close();
        }

        private void отмена_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }


    }
}
