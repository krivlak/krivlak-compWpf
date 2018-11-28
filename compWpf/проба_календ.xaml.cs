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
    /// Логика взаимодействия для проба_календ.xaml
    /// </summary>
    public partial class проба_календ : Window
    {
        public проба_календ()
        {
            InitializeComponent();
        }
        Entities de = new Entities();
        System.Windows.Data.CollectionViewSource viewSource1= new CollectionViewSource();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            de.слеты.OrderBy(n => n.порядок).Load();
            viewSource1.Source = de.слеты.Local.ToBindingList();
            dataGrid1.ItemsSource = viewSource1.View;

        }
    }
}
