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
    /// Логика взаимодействия для список_дистанций.xaml
    /// </summary>
    /// 
   
    public partial class список_дистанций : Window
    {
        public список_дистанций()
        {
            InitializeComponent();
        }

        compWpf.Entities de = new Entities();
        System.Windows.Data.CollectionViewSource viewSource1;
      //  List<дистанции> дистанцииList;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                viewSource1 = new CollectionViewSource();
                 de.дистанции.Where(n => n.слет == клСлет.слет).OrderBy(n => n.порядок).Load();
                //   дистанцииЛист = de.дистанции.Local.ToBindingList();
              //  дистанцииList = de.дистанции.Where(n => n.слет == клСлет.слет).OrderBy(n => n.порядок).ToList();
                viewSource1.Source = de.дистанции.Local.ToBindingList();
                //дистанцииDataGrid.ItemsSource = viewSource1.View;
                dataGrid1.ItemsSource = viewSource1.View;
                //   дистанцииЛист.ListChanged += дистанцииЛист_ListChanged;
             //   наимен_слета.Text = "Список дистанций вида " + клВид.наимен + "   " + клСлет.наимен;
                dataGrid1.Focus();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Сбой загрузки " + ex.Message);
            }

            this.Closed += Список_школ_Closed;
            viewSource1.View.CollectionChanged += View_CollectionChanged;

            dataGrid1.CellEditEnding += DataGrid1_CellEditEnding;
        }

        private void DataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                label1.Visibility = Visibility.Visible;
            }
            if (e.Column == наименColumn)
            {
                //  e.Column.GetValue()
                TextBox tb = e.EditingElement as TextBox;
                if (string.IsNullOrEmpty(tb.Text))
                {
                    MessageBox.Show("Наименование не может быть пустым");
                    e.Cancel = true;
                    //e.Row.Focus();
                    //tb.Focus();

                }
            }
        }

        private void View_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            label1.Visibility = Visibility;
        }

        private void Список_школ_Closed(object sender, EventArgs e)
        {
            if (label1.Visibility == Visibility)
            {
                try
                {
                    de.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Сбой записи " + ex.Message);
                }
            }
        }

        private void новая_Click(object sender, RoutedEventArgs e)
        {
            список_видов выборВида = new список_видов();
            выборВида.Выход.Content = " Отмена";
            выборВида.Title = " Выберите вид";
            выборВида.наимен_слета.Text = выборВида.Title;
            выборВида.ShowDialog();
            if (выборВида.DialogResult == true)
            {
                виды vRow = de.виды.Single(n => n.вид == клВид.вид);
                int maxPor = 0;
                if (de.дистанции.Local.Any())
                {
                    maxPor = de.дистанции.Local.Max(n => n.порядок);
                }
                дистанции newRow = new дистанции
                {
                    наимен = "Новая",
                    порядок = maxPor + 1,
                    дистанция = Guid.NewGuid(),
                    вид = клВид.вид,
                    слет = клСлет.слет,
                     виды=vRow
                };
                de.дистанции.Local.Add(newRow);
               // дистанцииList.Add(newRow);
                viewSource1.View.MoveCurrentTo(newRow);
                viewSource1.View.Refresh();
            }
            dataGrid1.Focus();

        }





        private void Выход_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }



        private void Удалить_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                дистанции delRow = viewSource1.View.CurrentItem as дистанции;
                if (delRow != null)
                {
                    

                    if (delRow.этапов == 0  && delRow.экипажей==0)
                    {
                        de.дистанции.Local.Remove(delRow);
                    //    дистанцииList.Remove(delRow);
                        label1.Visibility = Visibility;
                        viewSource1.View.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("Предварительно удалите этапы и экипажи этой дистанции");
                    }
                }

            }

        }

        private void Вверх_Click(object sender, RoutedEventArgs e)
        {

            if (!viewSource1.View.IsEmpty)
            {
                дистанции oldRow = viewSource1.View.CurrentItem as дистанции;

               // int oldPor = oldRow.порядок;
                if (viewSource1.View.CurrentPosition > 0)
                {
                    viewSource1.View.MoveCurrentToPrevious();

                    дистанции lastRow = viewSource1.View.CurrentItem as дистанции;
                  //  int lastPor = lastRow.порядок;
                    (oldRow.порядок, lastRow.порядок) = (lastRow.порядок, oldRow.порядок);
                    //oldRow.порядок = lastPor;
                    //lastRow.порядок = oldPor;

                    label1.Visibility = Visibility;
                    dataGrid1.Items.Refresh();

                    dataGrid1.Items.SortDescriptions.Clear();

                    dataGrid1.Items.SortDescriptions.Add(new SortDescription("порядок", ListSortDirection.Ascending));
                    viewSource1.View.MoveCurrentToPrevious();

                    //   viewSource1.View.Refresh();
                }
            }

            dataGrid1.Focus();
        }

        private void вНиз_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                дистанции oldRow = viewSource1.View.CurrentItem as дистанции;

             //   int oldPor = oldRow.порядок;
                if (viewSource1.View.CurrentPosition < de.дистанции.Local.Count - 1)
                {
                    viewSource1.View.MoveCurrentToNext();
                    дистанции lastRow = viewSource1.View.CurrentItem as дистанции;
                    (oldRow.порядок, lastRow.порядок) = (lastRow.порядок, oldRow.порядок);
                    //int lastPor = lastRow.порядок;
                    //oldRow.порядок = lastPor;
                    //lastRow.порядок = oldPor;
                    label1.Visibility = Visibility;
                    dataGrid1.Items.Refresh();

                    dataGrid1.Items.SortDescriptions.Clear();

                    dataGrid1.Items.SortDescriptions.Add(new SortDescription("порядок", ListSortDirection.Ascending));
                    viewSource1.View.MoveCurrentToNext();

                    //  viewSource1.View.Refresh();
                }
            }
            dataGrid1.Focus();
        }

        private void Выбор_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                дистанции dRow = viewSource1.View.CurrentItem as дистанции;
                клДистанция.deRow = dRow;
                клДистанция.дистанция = dRow.дистанция;
                клДистанция.наимен = dRow.наимен;
                this.DialogResult = true;
            }
            Close();
        }
    }
}
