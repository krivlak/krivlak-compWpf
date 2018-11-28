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
    /// Логика взаимодействия для список_видов.xaml
    /// </summary>
    /// 

    public partial class список_видов : Window
    {
        compWpf.Entities de = new Entities();
        System.Windows.Data.CollectionViewSource viewSource1;
        public список_видов()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                viewSource1 = new CollectionViewSource();
                de.виды.OrderBy(n => n.порядок).Load();
                //   видыЛист = de.виды.Local.ToBindingList();

                viewSource1.Source = de.виды.Local.ToBindingList();
                //видыDataGrid.ItemsSource = viewSource1.View;
                dataGrid1.ItemsSource = viewSource1.View;
                //   видыЛист.ListChanged += видыЛист_ListChanged;
                наимен_слета.Text = клСлет.наимен;
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

          

           if(e.EditAction== DataGridEditAction.Commit)
            {
                label1.Visibility = Visibility.Visible;
            }

            if (e.Column == наименColumn)
            {
              //  e.Column.GetValue()
                TextBox tb = e.EditingElement as TextBox;
                if(string.IsNullOrEmpty(tb.Text))
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

            label1.Visibility = Visibility.Visible;
        }

        private void Список_школ_Closed(object sender, EventArgs e)
        {
            foreach (виды sRow in de.виды.Local)
            {
                if (de.Entry(sRow).State != EntityState.Unchanged)
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
            if (de.виды.Local.Any())
            {
                maxPor = de.виды.Local.Max(n => n.порядок);
            }
            виды newRow = new виды
            {
                наимен = "Новая",
                порядок = maxPor + 1,
                вид = Guid.NewGuid()
            };
            de.виды.Local.Add(newRow);
            viewSource1.View.MoveCurrentTo(newRow);
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
                виды delRow = viewSource1.View.CurrentItem as виды;
                if (delRow != null)
                {
                    Guid кодвиды = delRow.вид;
                    //int всегоТуристов = de.туристы.Count(n => n.школа == кодвиды);
                    int всегоДистанций = de.дистанции.Count(n => n.вид == кодвиды);

                    if (  delRow.дистанций == 0 && всегоДистанций==0)
                    {
                        de.виды.Local.Remove(delRow);

                        label1.Visibility = Visibility;
                    }
                    else
                    {
                        MessageBox.Show("Предварительно удалите дистанции этого вида");
                    }
                }

            }
            dataGrid1.Focus();

        }

        private void Вверх_Click(object sender, RoutedEventArgs e)
        {

            if (!viewSource1.View.IsEmpty)
            {
                виды oldRow = viewSource1.View.CurrentItem as виды;

              //  int oldPor = oldRow.порядок;
                if (viewSource1.View.CurrentPosition > 0)
                {
                    viewSource1.View.MoveCurrentToPrevious();

                    виды lastRow = viewSource1.View.CurrentItem as виды;
                 //   int lastPor = lastRow.порядок;
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
                виды oldRow = viewSource1.View.CurrentItem as виды;

              //  int oldPor = oldRow.порядок;
                if (viewSource1.View.CurrentPosition < de.виды.Local.Count - 1)
                {
                    viewSource1.View.MoveCurrentToNext();
                    виды lastRow = viewSource1.View.CurrentItem as виды;
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
                виды vRow = viewSource1.View.CurrentItem as виды;
                клВид.deRow = vRow;
                клВид.вид = vRow.вид;
                клВид.наимен = vRow.наимен;
                DialogResult = true;
            }
            Close();
        }
    }
}
