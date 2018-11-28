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
    /// Логика взаимодействия для список_слетов.xaml
    /// </summary>
    public partial class список_слетов : Window
    {
        compWpf.Entities de = new Entities();
        //    BindingList<слеты> слетыЛист;
        //  System.Windows.Data.Binding binding1;
        System.Windows.Data.CollectionViewSource viewSource1;
        public список_слетов()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                viewSource1 = new CollectionViewSource();
                de.слеты.OrderBy(n => n.порядок).Load();
                //   слетыЛист = de.слеты.Local.ToBindingList();

                viewSource1.Source = de.слеты.Local.ToBindingList();
                //слетыDataGrid.ItemsSource = viewSource1.View;
                dataGrid1.ItemsSource = viewSource1.View;
                dataGrid1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сбой загрузки " + ex.Message);
            }

            //   слетыЛист.ListChanged += слетыЛист_ListChanged;
            this.Closed += Список_школ_Closed;

            viewSource1.View.CollectionChanged += View_CollectionChanged;
            //     dataGrid1.MouseRightButtonDown += DataGrid1_MouseRightButtonDown;
            //   dataGrid1.RowEditEnding += DataGrid1_RowEditEnding;
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
                var editedTextbox = e.EditingElement as TextBox;

                if (editedTextbox != null)
                {
                    string ss = editedTextbox.Text;
                    if (ss == String.Empty)
                    {
                        MessageBox.Show("Наименование не может быть пустым");
                        e.Cancel = true;
                    }
                }
            }

        }
    
                  

        private void View_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            label1.Visibility = Visibility.Visible;
        }

        private void Список_школ_Closed(object sender, EventArgs e)
        {
            foreach (слеты sRow in de.слеты.Local)
            {
                if(de.Entry(sRow).State != EntityState.Unchanged)
                {
                    label1.Visibility = Visibility.Visible;
                }
            }

            if (label1.Visibility == Visibility.Visible)
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
            int maxPor = 0;
            if (de.слеты.Local.Any())
            {
                maxPor = de.слеты.Local.Max(n => n.порядок);
            }
            слеты newRow = new слеты
            {
                наимен = "Новая",
                порядок = maxPor + 1,
                 слет = Guid.NewGuid(),
                  дата_по=DateTime.Today,
                   дата_с=DateTime.Today
            };
            de.слеты.Local.Add(newRow);
            viewSource1.View.MoveCurrentTo(newRow);
            dataGrid1.Focus();
        }





        private void Выход_Click(object sender, RoutedEventArgs e)
        {

            Close();
        }



        private void Удалить_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                слеты delRow = viewSource1.View.CurrentItem as слеты;
                if (delRow != null)
                {
                    Guid кодслеты = delRow.слет;
                    //int всегоТуристов = de.туристы.Count(n => n.школа == кодслеты);
                    //int всегоЭкипажей = de.экипажи.Count(n => n.школа == кодслеты);

                    if (delRow.туристов == 0 && delRow.дистанций == 0 )
                    {
                        de.слеты.Local.Remove(delRow);

                        label1.Visibility = Visibility;
                    }
                    else
                    {
                        MessageBox.Show("Предварительно удалите туристов и дистанции слета");
                    }
                }

            }
            dataGrid1.Focus();

        }

        private void Вверх_Click(object sender, RoutedEventArgs e)
        {

            if (!viewSource1.View.IsEmpty)
            {
                слеты oldRow = viewSource1.View.CurrentItem as слеты;

              //  int oldPor = oldRow.порядок;
                if (viewSource1.View.CurrentPosition > 0)
                {
                    viewSource1.View.MoveCurrentToPrevious();

                    слеты lastRow = viewSource1.View.CurrentItem as слеты;
                    (oldRow.порядок, lastRow.порядок) = (lastRow.порядок, oldRow.порядок);
                    //int lastPor = lastRow.порядок;
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
                слеты oldRow = viewSource1.View.CurrentItem as слеты;

              //  int oldPor = oldRow.порядок;
                if (viewSource1.View.CurrentPosition < de.слеты.Local.Count - 1)
                {
                    viewSource1.View.MoveCurrentToNext();
                    слеты lastRow = viewSource1.View.CurrentItem as слеты;
                    //int lastPor = lastRow.порядок;
                    //oldRow.порядок = lastPor;
                    //lastRow.порядок = oldPor;
                    (oldRow.порядок, lastRow.порядок) = (lastRow.порядок, oldRow.порядок);
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

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            label1.Visibility = Visibility.Visible;
        }

        private void DatePicker_SelectedDateChanged_1(object sender, SelectionChangedEventArgs e)
        {
            label1.Visibility = Visibility.Visible;
        }

     

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
           //     label1.Visibility = Visibility.Visible;
          
        }
    }
}
