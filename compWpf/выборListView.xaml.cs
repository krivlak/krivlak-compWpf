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
using System.Data.Entity;
using System.ComponentModel;
using System.Data;

namespace compWpf
{
    /// <summary>
    /// Логика взаимодействия для выборListView.xaml
    /// </summary>
    public partial class выборListView : Window
    {
        public выборListView()
        {
            InitializeComponent();
        }
        compWpf.Entities de = new Entities();
        System.Windows.Data.CollectionViewSource viewSource1 = new CollectionViewSource();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          //  de.виды.OrderBy(n => n.порядок).Load();
            viewSource1.Source = de.виды.OrderBy(n => n.порядок).ToList();

            listView1.ItemsSource = viewSource1.View;
            listView1.SelectedIndex = 0;


            if (клВид.deRow != null)
            {
                виды tRow = de.виды.Single(n => n.вид == клВид.вид);
                listView1.SelectedItem = tRow;
            }
            актСлет.Text = клСлет.наимен;
        }
        private void выбрать_Click(object sender, RoutedEventArgs e)
        {
            //if(viewSource1.View.CurrentItem != null)
            //{

            //    виды tRow = (виды)viewSource1.View.CurrentItem as виды;
            //    MessageBox.Show(tRow.наимен);
            //}
            if (listView1.Items.Count > 0)
            {
                
                if (listView1.SelectedItem is виды)
                {
                    виды vRow = (виды)listView1.SelectedItem;
                    клВид.deRow = vRow;
                    клВид.вид = vRow.вид;
                    клВид.наимен = vRow.наимен;
                    клВид.выбран = true;
                    this.DialogResult = true;
                    Close();
                }
            }
        }

        private void отмена_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }
    }
}
