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
    /// Логика взаимодействия для список_этапов.xaml
    /// </summary>
    public partial class список_этапов : Window
    {
        public список_этапов()
        {
            InitializeComponent();
        }
        compWpf.Entities de = new Entities();
        System.Windows.Data.CollectionViewSource viewSource1;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                viewSource1 = new CollectionViewSource();
                de.этапы.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.порядок).Load();

                viewSource1.Source = de.этапы.Local.ToBindingList();
                dataGrid1.ItemsSource = viewSource1.View;
            //    dataGrid1.SelectedIndex = 0;
                dataGrid1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сбой загрузки " + ex.Message);
            }

            наимен_слета.Text ="Список этапов  на "+ клДистанция.наимен + "  " + клСлет.наимен;

            this.Closed += Список_школ_Closed;
            viewSource1.View.CollectionChanged += View_CollectionChanged;
            dataGrid1.CellEditEnding += DataGrid1_CellEditEnding;
           // viewSource1.View.CurrentChanging += View_CurrentChanging;
        }

        private void View_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            var строка = viewSource1.View.CurrentItem;
            этапы eRow = viewSource1.View.CurrentItem as этапы;
            if(eRow !=null)
            {
                if(String.IsNullOrWhiteSpace(eRow.наимен))
                {
                    MessageBox.Show("Наименование не может быть пустым//////////");
                  //  e.Cancel = true;
                    viewSource1.View.MoveCurrentTo(строка);
                    viewSource1.View.Refresh();
                }
            }
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
                        e.Cancel = true ;
                    }
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
            int maxPor = 0;
            if (de.этапы.Local.Any())
            {
                maxPor = de.этапы.Local.Max(n => n.порядок);
            }
            дистанции выбр_дист = de.дистанции.Single(n => n.дистанция == клДистанция.дистанция);
            этапы newRow = new этапы
            {
                наимен = "Новый",
                порядок = maxPor + 1,
                дистанция = клДистанция.дистанция,
                этап = Guid.NewGuid(),
                судья = "",
                 дистанции=выбр_дист
            };
            de.этапы.Local.Add(newRow);
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
                этапы delRow = viewSource1.View.CurrentItem as этапы;
                if (delRow != null)
                {


                    if (delRow.штрафов == 0)
                    {
                        de.этапы.Local.Remove(delRow);

                        label1.Visibility = Visibility;
                    }
                    else
                    {
                        MessageBox.Show("Предварительно удалите штрафы  этого этапа");
                    }
                }

            }
            dataGrid1.Focus();

        }

        private void Вверх_Click(object sender, RoutedEventArgs e)
        {

            if (!viewSource1.View.IsEmpty)
            {
                этапы oldRow = viewSource1.View.CurrentItem as этапы;

              //  int oldPor = oldRow.порядок;
                if (viewSource1.View.CurrentPosition > 0)
                {
                    viewSource1.View.MoveCurrentToPrevious();

                    этапы lastRow = viewSource1.View.CurrentItem as этапы;
                    //int lastPor = lastRow.порядок;
                    //oldRow.порядок = lastPor;
                    //lastRow.порядок = oldPor;
                    (oldRow.порядок, lastRow.порядок) = (lastRow.порядок, oldRow.порядок);
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
                этапы oldRow = viewSource1.View.CurrentItem as этапы;

               // int oldPor = oldRow.порядок;
                if (viewSource1.View.CurrentPosition < de.этапы.Local.Count - 1)
                {
                    viewSource1.View.MoveCurrentToNext();
                    этапы lastRow = viewSource1.View.CurrentItem as этапы;
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

        //private void копировать_Click(object sender, RoutedEventArgs e)
        //{
        //    //дистанции oldДистанции = клДистанция.deRow;
        //    клДистанция.выбран = false;
        //    выбор_кода_дистанции выборДистанции = new выбор_кода_дистанции();
        //    выборДистанции.ShowDialog();

            

        //    if (клДистанция.выбран || выборДистанции.DialogResult == true)
        //    {

        //        Guid кодНов = (Guid)выборДистанции.Tag;
        //        string sqlString = "select наимен, судья, порядок from этапы where дистанция = @p0  order by порядок";
        //     //   var query = de.Database.ExecuteSqlCommand(sqlString, кодНов);
        //        List<temp> tempList = de.Database.SqlQuery<temp>(sqlString, кодНов).ToList();
        //        //дистанции dRow = (дистанции)выборДистанции.Tag;
        //        //Guid кодНов = dRow.дистанция;
        //        //востановление 
        //        //клДистанция.deRow = oldДистанции;
        //        //клДистанция.дистанция = клДистанция.deRow.дистанция;
        //        //клДистанция.наимен = клДистанция.deRow.наимен;
        //        //клДистанция.вид = клДистанция.deRow.вид;
        //        de.этапы.Local.Clear();
        //        //de.SaveChanges();
        //        //viewSource1.View.Refresh();
        //      //  var query = de.этапы.Where(n => n.дистанция == кодНов).OrderBy(n=>n.порядок).ToList();
               
        //        foreach (temp eRow in tempList)
        //        {
        //            этапы newRow = new этапы()
        //            {
        //                дистанция = клДистанция.дистанция,
        //                наимен = eRow.наимен,
        //                порядок = eRow.порядок,
        //                судья = eRow.судья,
        //                этап = Guid.NewGuid()
        //            };
        //            de.этапы.Local.Add(newRow);
        //         //   Console.WriteLine(newRow.наимен);
        //        }
        //        //de.SaveChanges();
        //        //de.этапы.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.порядок).Load();
        //        //viewSource1.Source = de.этапы.Local.ToBindingList();
        //        //dataGrid1.ItemsSource = viewSource1.View;

        //           label1.Visibility = Visibility.Visible;
        //        //   viewSource1.View.Refresh();
        //      //  viewSource1.Source = null;
        //      //viewSource1.Source = de.этапы.Local.ToBindingList();
        //        //dataGrid1.ItemsSource = viewSource1.View;
        //     //   Close();
        //    }
        //}

        //class temp
        //{
        //    public string наимен { get; set; }
        //    public string судья { get; set; }
        //    public int порядок { get; set; }

        //}
    }
}
