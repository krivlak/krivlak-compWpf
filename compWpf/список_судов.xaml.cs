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
    /// Логика взаимодействия для список_судов.xaml
    /// </summary>
    public partial class список_судов : Window
    {
        public список_судов()
        {
            InitializeComponent();
        }
        // compWpf.Entities de = new Entities();
        compWpf.Entities de = new Entities();
        System.Windows.Data.CollectionViewSource viewSource1;
     //   List<суда> судаList;
        BindingList<суда> bindList;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                de.суда.OrderBy(n => n.порядок).Load();
                viewSource1 = new CollectionViewSource();
                bindList = de.суда.Local.ToBindingList();
             //    судаList =   de.суда.OrderBy(n => n.порядок).ToList();

                viewSource1.Source = bindList;
                dataGrid1.ItemsSource = viewSource1.View;
                //    dataGrid1.SelectedIndex = 0;
                dataGrid1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сбой загрузки " + ex.Message);
            }

        //    наимен_слета.Text = "Список судов на "+клДистанция.наимен + "  " + клСлет.наимен;

            this.Closed += Список_школ_Closed;
            viewSource1.View.CollectionChanged += View_CollectionChanged;
            dataGrid1.CellEditEnding += DataGrid1_CellEditEnding;
            // viewSource1.View.CurrentChanging += View_CurrentChanging;
        }

        private void View_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            var строка = viewSource1.View.CurrentItem;
            суда eRow = viewSource1.View.CurrentItem as суда;
            if (eRow != null)
            {
                if (String.IsNullOrWhiteSpace(eRow.наимен))
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

                        //   dataGrid1.CancelEdit();
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
            if (de.суда.Local.Any())
            {
                maxPor = de.суда.Local.Max(n => n.порядок);
            }
            суда newRow = new суда
            {
                наимен = "Новый",
                порядок = maxPor + 1,
                судно = Guid.NewGuid()
                 
            };
            bindList.Add(newRow);
            //de.суда.Add(newRow);
            //судаList.Add(newRow);
            viewSource1.View.MoveCurrentTo(newRow);
            viewSource1.View.Refresh();
            dataGrid1.Focus();

        }





        private void Выход_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }



        private void Удалить_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                суда delRow = viewSource1.View.CurrentItem as суда;
                if (delRow != null)
                {


                    if (delRow.экипажей == 0)
                    {
                      //  de.суда.Remove(delRow);
                        bindList.Remove(delRow);
                        viewSource1.View.Refresh();
                        label1.Visibility = Visibility;
                    }
                    else
                    {
                        MessageBox.Show("Предварительно удалите экипажи  на этом судне");
                    }
                }

            }
            dataGrid1.Focus();

        }

        private void Вверх_Click(object sender, RoutedEventArgs e)
        {

            if (!viewSource1.View.IsEmpty)
            {
                суда oldRow = viewSource1.View.CurrentItem as суда;

              //  int oldPor = oldRow.порядок;
                if (viewSource1.View.CurrentPosition > 0)
                {
                    viewSource1.View.MoveCurrentToPrevious();

                    суда lastRow = viewSource1.View.CurrentItem as суда;
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
                суда oldRow = viewSource1.View.CurrentItem as суда;

              //  int oldPor = oldRow.порядок;
                if (viewSource1.View.CurrentPosition < de.суда.Local.Count - 1)
                {
                    viewSource1.View.MoveCurrentToNext();
                    суда lastRow = viewSource1.View.CurrentItem as суда;
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

        private void Выбор_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                суда oldRow = viewSource1.View.CurrentItem as суда;
                клСудно.deRow = oldRow;
                клСудно.судно = oldRow.судно;
                клСудно.наимен = oldRow.наимен;

                this.DialogResult = true;
            }
            Close();
        }

        private void ВНиз_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                суда oldRow = viewSource1.View.CurrentItem as суда;
               .

                LinkedList<суда> linkList = new LinkedList<суда>();
                foreach (суда tRow in bindList
                    .Where(n=>n.порядок >= oldRow.порядок)
                    .OrderBy(n => n.порядок))
                {
                    linkList.AddLast(tRow);
                }
                var верхRow= linkList.Find(oldRow);
                //if(верхRow.Next != null)
                if (верхRow != linkList.Last)
                {
                    var низRow = верхRow.Next;
                    (верхRow.Value.порядок, низRow.Value.порядок) = (низRow.Value.порядок, верхRow.Value.порядок);
                    label1.Visibility = Visibility;
                   

                    dataGrid1.Items.SortDescriptions.Clear();

                    dataGrid1.Items.SortDescriptions.Add(new SortDescription("порядок", ListSortDirection.Ascending));
                    dataGrid1.Items.Refresh();

                    //   viewSource1.View.MoveCurrentToNext();
                }

                ////  int oldPor = oldRow.порядок;
                //if (viewSource1.View.CurrentPosition < de.суда.Local.Count - 1)
                //{
                //    viewSource1.View.MoveCurrentToNext();
                //    суда lastRow = viewSource1.View.CurrentItem as суда;
                    
                //    //int lastPor = lastRow.порядок;
                //    //oldRow.порядок = lastPor;
                //    //lastRow.порядок = oldPor;
                //    (oldRow.порядок, lastRow.порядок) = (lastRow.порядок, oldRow.порядок);
                //    label1.Visibility = Visibility;
                //    dataGrid1.Items.Refresh();

                //    dataGrid1.Items.SortDescriptions.Clear();

                //    dataGrid1.Items.SortDescriptions.Add(new SortDescription("порядок", ListSortDirection.Ascending));
                //    viewSource1.View.MoveCurrentToNext();

                //    //  viewSource1.View.Refresh();
                //}
            }
            dataGrid1.Focus();
        }
    }
}
