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
    /// Логика взаимодействия для список_школ.xaml
    /// </summary>
    public partial class список_школ : Window
    {
        compWpf.Entities de = new Entities();
        //   List<школы> школыЛист;
        //  System.Windows.Data.Binding binding1;
        BindingList<школы> bindList;
        System.Windows.Data.CollectionViewSource школыViewSource;
        public список_школ()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                de.школы.OrderBy(n => n.порядок).Load();
                школыViewSource = new CollectionViewSource();
                //  de.школы.OrderBy(n => n.порядок).Load();
                //школыЛист = de.школы.OrderBy(n=>n.порядок).ToList();
                bindList = de.школы.Local.ToBindingList();
                школыViewSource.Source = bindList;
                //школыDataGrid.ItemsSource = школыViewSource.View;
                dataGrid1.ItemsSource = школыViewSource.View;
                //   школыЛист.ListChanged += ШколыЛист_ListChanged;
            //    наимен_слета.Text = "Список школ, клубов " + клСлет.наимен;
                dataGrid1.Focus();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Сбой загрузки " + ex.Message);
            }
            this.Closed += Список_школ_Closed;
            школыViewSource.View.CollectionChanged += View_CollectionChanged;
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
            label1.Visibility = Visibility;
        }

        private void Список_школ_Closed(object sender, EventArgs e)
        {
          if(label1.Visibility== Visibility)
            {
                try
                {
                    de.SaveChanges();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Сбой записи " + ex.Message);
                }
            }
        }

        private void ШколыЛист_ListChanged(object sender, ListChangedEventArgs e)
        {
            label1.Visibility = Visibility;
        }

        private void новая_Click(object sender, RoutedEventArgs e)
        {
            int maxPor = 0;
            if(de.школы.Local.Any())
            {
                maxPor = de.школы.Local.Max(n => n.порядок);
            }
            школы newRow = new школы
            {
                наимен = "Новая",
                порядок = maxPor+1,
                школа = Guid.NewGuid()
            };
            de.школы.Local.Add(newRow);
          //  школыЛист.Add(newRow);
            школыViewSource.View.MoveCurrentTo(newRow);
            школыViewSource.View.Refresh();
            dataGrid1.Focus();
        }

      

       

        private void Выход_Click(object sender, RoutedEventArgs e)
        {
           
            Close();
        }

     

        private void Удалить_Click(object sender, RoutedEventArgs e)
        {
            if(! школыViewSource.View.IsEmpty)
            {
                школы delRow = школыViewSource.View.CurrentItem as школы;
                if (delRow != null)
                {
                    Guid кодШколы = delRow.школа;
                    int всегоТуристов = de.туристы.Count(n => n.школа == кодШколы);
                    int всегоЭкипажей = de.экипажи.Count(n => n.школа == кодШколы);

                    if (delRow.туристов == 0 && delRow.экипажей == 0 && всегоТуристов == 0 && всегоЭкипажей == 0)
                    {
                        de.школы.Local.Remove(delRow);
                    //    школыЛист.Remove(delRow);
                        школыViewSource.View.Refresh();

                        label1.Visibility = Visibility;
                    }
                    else
                    {
                        MessageBox.Show("Предварительно удалите туристо и экипажи школы");
                    }
                }

            }

            dataGrid1.Focus();

        }

        private void Вверх_Click(object sender, RoutedEventArgs e)
        {

            if (!школыViewSource.View.IsEmpty)
            {
                школы oldRow = школыViewSource.View.CurrentItem as школы;
             //   int oldIndex = школыViewSource.View.CurrentPosition;

              //  int oldPor = oldRow.порядок;
                if (школыViewSource.View.CurrentPosition > 0)
                {
                    школыViewSource.View.MoveCurrentToPrevious();

                    школы lastRow = школыViewSource.View.CurrentItem as школы;
                    //int lastPor = lastRow.порядок;
                    //oldRow.порядок = lastPor;
                    //lastRow.порядок = oldPor;
                    (oldRow.порядок, lastRow.порядок) = (lastRow.порядок, oldRow.порядок);
                    //       школы_деталейЛист.Sort((a, b) => a.порядок.CompareTo(b.порядок));
                    //   школыViewSource.View. = "порядок";
                    //    dataGrid1.re;

                    label1.Visibility= Visibility;
                    dataGrid1.Items.Refresh();

                    dataGrid1.Items.SortDescriptions.Clear();

                    dataGrid1.Items.SortDescriptions.Add(new SortDescription("порядок", ListSortDirection.Ascending));
                    школыViewSource.View.MoveCurrentToPrevious();

                    //   школыViewSource.View.Refresh();
                }
            }
            
            dataGrid1.Focus();
        }

        private void вНиз_Click(object sender, RoutedEventArgs e)
        {
            if (!школыViewSource.View.IsEmpty)
            {
                школы oldRow = школыViewSource.View.CurrentItem as школы;

              //  int oldPor = oldRow.порядок;
                if (школыViewSource.View.CurrentPosition < de.школы.Local.Count - 1)
                {
                    школыViewSource.View.MoveCurrentToNext();
                    школы lastRow = школыViewSource.View.CurrentItem as школы;
                    //bindingSource1.MoveNext();
                    //школы lastRow = bindingSource1.Current as школы;
                    //int lastPor = lastRow.порядок;
                    //oldRow.порядок = lastPor;
                    //lastRow.порядок = oldPor;
                    (oldRow.порядок, lastRow.порядок) = (lastRow.порядок, oldRow.порядок);
                    //   bindingSource1.Sort = "порядок";

                    //   школы_деталейЛист.Sort((a, b) => a.порядок.CompareTo(b.порядок));
                    //   dataGrid1.Refresh();
                    label1.Visibility = Visibility;
                    dataGrid1.Items.Refresh();

                    dataGrid1.Items.SortDescriptions.Clear();

                    dataGrid1.Items.SortDescriptions.Add(new SortDescription("порядок", ListSortDirection.Ascending));
                    школыViewSource.View.MoveCurrentToNext();

                    //  школыViewSource.View.Refresh();
                }
            }
            dataGrid1.Focus();
        }

        private void Выбор_Click(object sender, RoutedEventArgs e)
        {
            if (!школыViewSource.View.IsEmpty)
            {
                школы dRow = школыViewSource.View.CurrentItem as школы;
                клШкола.deRow = dRow;
                клШкола.школа = dRow.школа;
                клШкола.наимен = dRow.наимен;
                this.DialogResult = true;
            }
            Close();
        }
    }
}
