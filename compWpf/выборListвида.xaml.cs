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
    /// Логика взаимодействия для выборListвида.xaml
    /// </summary>
    public partial class выборListвида : Window
    {
        public выборListвида()
        {
            InitializeComponent();
        }
        Entities de = new Entities();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lisBox1.ItemsSource = de.виды.OrderBy(n => n.порядок).ToList();
            lisBox1.SelectionChanged += LisBox1_SelectionChanged;
        }

        private void LisBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lisBox1.Items.Count > 0)
            {
                виды vRow = (виды)lisBox1.SelectedItem;
                MessageBox.Show(vRow.наимен);
            }
        }

            private void выбрать_Click(object sender, RoutedEventArgs e)
        {
            if(lisBox1.Items.Count>0)
            {
                виды vRow = (виды)lisBox1.SelectedItem;
                MessageBox.Show(vRow.наимен);
            }
        }
    }
}
