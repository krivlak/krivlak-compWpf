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

namespace compWpf
{
    /// <summary>
    /// Логика взаимодействия для источник.xaml
    /// </summary>
    public partial class источник : Window
    {
        public источник()
        {
            InitializeComponent();
        }
        compWpf.Entities de = new Entities();
        System.Windows.Data.CollectionViewSource школыViewSource ;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            школыViewSource = new CollectionViewSource();
     //      школыViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("школыViewSource")));
            // Загрузите данные, установив свойство CollectionViewSource.Source:
            de.школы.OrderBy(n => n.порядок).Load();
            // школыViewSource.Source = [универсальный источник данных]
            школыDataGrid.AutoGenerateColumns = true;
            школыViewSource.Source = de.школы.Local.ToBindingList();
            школыDataGrid.ItemsSource = школыViewSource.View;
        }

        private void вниз_Click(object sender, RoutedEventArgs e)
        {
            школыViewSource.View.MoveCurrentToNext();
            школыDataGrid.Focus();
            школы tRow = школыViewSource.View.CurrentItem as школы;
            MessageBox.Show(tRow.наимен);
        }

        private void Вверх_Click(object sender, RoutedEventArgs e)
        {
            школыViewSource.View.MoveCurrentToPrevious();
            школыDataGrid.Focus();
        }
    }
}
