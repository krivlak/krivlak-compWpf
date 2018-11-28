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
    /// Логика взаимодействия для список_экипажей.xaml
    /// </summary>
    public partial class список_экипажей : Window
    {
        public список_экипажей()
        {
            InitializeComponent();
        }
        
        compWpf.Entities de =  new Entities();
    //    List<экипажи> экипажиList;
            
        System.Windows.Data.CollectionViewSource viewSource1;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {


                viewSource1 = new CollectionViewSource();
                de.школы.OrderBy(n => n.порядок).Load();
                de.суда.OrderBy(n => n.порядок).Load();
                de.экипажи.Where(n=>n.дистанция==клДистанция.дистанция).OrderBy(n => n.номер).Load();
                de.туристы.Where(n => n.слет == клСлет.слет).OrderBy(n => n.фамилия).Load();
                de.экипажи.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.номер).Load();
                //  экипажиList = de.экипажи.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.номер).ToList();
                viewSource1.Source = de.экипажи.Local.ToBindingList();
                dataGrid1.ItemsSource = viewSource1.View;
                наимен_слета.Text = "Экипажи " + клДистанция.наимен + "   " + клСлет.наимен;
                dataGrid1.Focus();


                this.Closed += Список_школ_Closed;
                viewSource1.View.CollectionChanged += View_CollectionChanged;
                viewSource1.View.CurrentChanging += View_CurrentChanging;

                // de.экипажи.Local.CollectionChanged += Local_CollectionChanged;

                dataGrid1.CellEditEnding += DataGrid1_CellEditEnding;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки " + ex.Message);
            }

        }

        private void DataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            if (e.EditAction == DataGridEditAction.Commit)
            {
                label1.Visibility = Visibility.Visible;
            }
          
        }
   

        private void Local_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            label1.Visibility = Visibility.Visible;
        }

        private void View_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            экипажи oldRow = viewSource1.View.CurrentItem as экипажи;
            if (oldRow != null)
            {
                if (de.Entry(oldRow).State != EntityState.Unchanged)
                {
                    label1.Visibility = Visibility.Visible;

                }
            }
        }

        private void View_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            label1.Visibility = Visibility.Visible;
        }

        private void Список_школ_Closed(object sender, EventArgs e)
        {


            //if (de.экипажи.Any())
            //{
            //    экипажи oldRow = viewSource1.View.CurrentItem as экипажи;
            //    if (de.Entry(oldRow).State != EntityState.Unchanged)
            //    {
            //        label1.Visibility = Visibility.Visible;

            //    }
            //}


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
            клШкола.выбран = false;
            список_школ выборШколы = new список_школ();
            выборШколы.Title = " Выберите школу, клуб";
            выборШколы.наимен_слета.Text= " Выберите школу, клуб";
            выборШколы.Выход.Content = "Отмена";
            выборШколы.ShowDialog();
            if (выборШколы.DialogResult == true)
            {
               
                список_судов выборСудна = new список_судов();

                выборСудна.Title="Выберите судно";
                выборСудна.наимен_слета.Text = "Выберите судно";
           
                выборСудна.Выход.Content = "Отмена";
                выборСудна.ShowDialog();
                if (выборСудна.DialogResult == true)
                {

                   

                    int maxPor = 0;
                    if (de.экипажи.Local.Any())
                    {
                        maxPor = de.экипажи.Local.Max(n => n.номер);
                    }
                    школы выбр_школа = de.школы.Single(n => n.школа == клШкола.школа);
                    суда выбр_судно = de.суда.Single(n => n.судно == клСудно.судно);
                    экипажи newRow = new экипажи
                    {
                        номер = maxPor + 1,
                        экипаж = Guid.NewGuid(),
                        прим = "",
                        школа = клШкола.школа,
                        итог = 0,
                        место = 0,
                        судно = клСудно.судно,
                        школы = выбр_школа,
                        суда = выбр_судно,
                         дистанция=клДистанция.дистанция
                        //суда = выбр_судно
                    };
                    de.экипажи.Local.Add(newRow);
//                    экипажиList.Add(newRow);
                    viewSource1.View.MoveCurrentTo(newRow);
                    viewSource1.View.Refresh();
                }
            }
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
                экипажи delRow = viewSource1.View.CurrentItem as экипажи;
                if (delRow != null)
                {


                    if (delRow.попыток == 0)
                    {
                        de.экипажи.Local.Remove(delRow);
                     //   экипажиList.Remove(delRow);
                        label1.Visibility = Visibility;
                        viewSource1.View.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("Предварительно удалите попытки этого экипажа");
                    }
                }

            }
            dataGrid1.Focus();

        }

        private void Вверх_Click(object sender, RoutedEventArgs e)
        {

            if (!viewSource1.View.IsEmpty)
            {
                экипажи oldRow = viewSource1.View.CurrentItem as экипажи;

             //   int oldPor = oldRow.номер;
                if (viewSource1.View.CurrentPosition > 0)
                {
                    viewSource1.View.MoveCurrentToPrevious();

                    экипажи lastRow = viewSource1.View.CurrentItem as экипажи;
                    //int lastPor = lastRow.номер;
                    //oldRow.номер = lastPor;
                    //lastRow.номер = oldPor;
                    (oldRow.номер, lastRow.номер) = (lastRow.номер, oldRow.номер);

                    label1.Visibility = Visibility;
                    dataGrid1.Items.Refresh();

                    dataGrid1.Items.SortDescriptions.Clear();

                    dataGrid1.Items.SortDescriptions.Add(new SortDescription("номер", ListSortDirection.Ascending));
                    viewSource1.View.MoveCurrentToPrevious();

                }
            }

            dataGrid1.Focus();
        }

        private void вНиз_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                экипажи oldRow = viewSource1.View.CurrentItem as экипажи;

             //   int oldPor = oldRow.номер;
                if (viewSource1.View.CurrentPosition < de.экипажи.Local.Count - 1)
                {
                    viewSource1.View.MoveCurrentToNext();
                    экипажи lastRow = viewSource1.View.CurrentItem as экипажи;
                    //int lastPor = lastRow.номер;
                    //oldRow.номер = lastPor;
                    //lastRow.номер = oldPor;
                    (oldRow.номер, lastRow.номер) = (lastRow.номер, oldRow.номер);
                    label1.Visibility = Visibility;
                    dataGrid1.Items.Refresh();

                    dataGrid1.Items.SortDescriptions.Clear();

                    dataGrid1.Items.SortDescriptions.Add(new SortDescription("номер", ListSortDirection.Ascending));
                    viewSource1.View.MoveCurrentToNext();

                }
            }
            dataGrid1.Focus();
        }

        private void новый_матрос_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                экипажи eRow = viewSource1.View.CurrentItem as экипажи;
                клТурист.выбран = false;
                список_участников выборТуриста = new список_участников();
                выборТуриста.Title = "Выберите участника";
                выборТуриста.Выход.Content = "Отмена";
                выборТуриста.ShowDialog();
                if (выборТуриста.DialogResult == true)
                {
                    if (eRow.туристы.Any(n => n.турист == клТурист.турист))
                    {
                        MessageBox.Show("Уже есть");
                    }
                    else
                    {
                        туристы новТур = de.туристы.Single(n => n.турист == клТурист.турист);
                        eRow.туристы.Add(новТур);
                        label1.Visibility = Visibility;
                        viewSource1.View.Refresh();
                    }
                    

                }
            }
            dataGrid1.Focus();

        }

        private void очистить_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                экипажи eRow = viewSource1.View.CurrentItem as экипажи;
                eRow.туристы.Clear();
                label1.Visibility = Visibility;
                viewSource1.View.Refresh();
            }
            dataGrid1.Focus();
        }

        private void список_Click(object sender, RoutedEventArgs e)
        {

            //if (de.экипажи.Any())
            //{
            //  экипажи eRow = viewSource1.View.CurrentItem as экипажи;
            список_судов выборСудна = new список_судов();

            выборСудна.Title = "Выберите судно";
            выборСудна.наимен_слета.Text = "Выберите судно";

            выборСудна.Выход.Content = "Отмена";
            выборСудна.ShowDialog();
            if (выборСудна.DialogResult == true)
            {
                клТурист.выбран = false;
                выборVучастников выборТуриста = new выборVучастников();
                выборТуриста.наимен_слета.Text = " Выберите участников для " + клДистанция.наимен;
                выборТуриста.ShowDialog();
                if (выборТуриста.DialogResult == true)
                {
                    if (клТурист.turList.Count > 0)
                    {
                        foreach (туристы tRow in клТурист.turList)
                        {
                            int maxPor = 0;
                            if (de.экипажи.Local.Any())
                            {
                                maxPor = de.экипажи.Local.Max(n => n.номер);
                            }
                            школы выбр_школа = de.школы.Single(n => n.школа == tRow.школа);
                            суда выбр_судно = de.суда.Single(n => n.судно == клСудно.судно);
                            экипажи newRow = new экипажи()
                            {
                                итог = 0,
                                дистанция = клДистанция.дистанция,
                                место = 0,
                                номер = maxPor + 1,
                                прим = "",
                                школа = tRow.школа,
                                экипаж = Guid.NewGuid(),
                                судно = клСудно.судно,
                                школы = выбр_школа,
                                суда = выбр_судно
                            };

                            туристы newTur = de.туристы.Single(n => n.турист == tRow.турист);
                            newRow.туристы.Add(newTur);
                            de.экипажи.Local.Add(newRow);
                         //   экипажиList.Add(newRow);
                            label1.Visibility = Visibility;
                            viewSource1.View.Refresh();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Отметьте участников");
                    }
                }


            }
            dataGrid1.Focus();
        }

        private void одиночка_Click(object sender, RoutedEventArgs e)
        {
            список_судов выборСудна = new список_судов();

            выборСудна.Title = "Выберите судно";
            выборСудна.наимен_слета.Text = "Выберите судно";

            выборСудна.Выход.Content = "Отмена";
            выборСудна.ShowDialog();
            if (выборСудна.DialogResult == true)
            {
                //if (de.экипажи.Local.Any())
                //{
                //  экипажи eRow = viewSource1.View.CurrentItem as экипажи;
                клТурист.выбран = false;
                список_участников выборТуриста = new список_участников();
                выборТуриста.ShowDialog();
                if (выборТуриста.DialogResult == true)
                {
                    int maxPor = 0;
                    if (de.экипажи.Local.Any())
                    {
                       // maxPor = de.экипажи.Max(n => n.номер);
                        maxPor = de.экипажи.Local.Max(n => n.номер);
                    }
                    экипажи newRow = new экипажи()
                    {
                        итог = 0,
                        дистанция = клДистанция.дистанция,
                        место = 0,
                        номер = maxPor + 1,
                        прим = "",
                        школа = клТурист.deRow.школа,
                        экипаж = Guid.NewGuid(),
                         судно=клСудно.судно
                    };

                    туристы newTur = de.туристы.Single(n => n.турист == клТурист.турист);
                    newRow.туристы.Add(newTur);
                    de.экипажи.Local.Add(newRow);
                 //   экипажиList.Add(newRow);
                    viewSource1.View.MoveCurrentTo(newRow);
                    label1.Visibility = Visibility;
                    viewSource1.View.Refresh();

                }
            }
            dataGrid1.Focus();
        }

        private void список33_Click(object sender, RoutedEventArgs e)
        {
            список_судов выборСудна = new список_судов();

            выборСудна.Title = "Выберите судно";
            выборСудна.наимен_слета.Text = "Выберите судно";

            выборСудна.Выход.Content = "Отмена";
            выборСудна.ShowDialog();
            if (выборСудна.DialogResult == true)
            {
                клТурист.выбран = false;
                список_участников выборТуриста = new список_участников();
                выборТуриста.ShowDialog();
                if (выборТуриста.DialogResult == true)
                {
                    if (клТурист.turList.Count > 0)
                    {
                        foreach (туристы tRow in клТурист.turList)
                        {
                            int maxPor = 0;
                            if (de.экипажи.Local.Any())
                            {
                                maxPor = de.экипажи.Local.Max(n => n.номер);
                            }
                            школы выбр_школа = de.школы.Single(n => n.школа == tRow.школа);
                            суда выбр_судно = de.суда.Single(n => n.судно == клСудно.судно);
                            экипажи newRow = new экипажи()
                            {
                                итог = 0,
                                дистанция = клДистанция.дистанция,
                                место = 0,
                                номер = maxPor + 1,
                                прим = "",
                                школа = tRow.школа,
                                экипаж = Guid.NewGuid(),
                                судно = клСудно.судно,
                                школы = выбр_школа,
                                суда = выбр_судно
                            };

                            туристы newTur = de.туристы.Single(n => n.турист == tRow.турист);
                            newRow.туристы.Add(newTur);
                            de.экипажи.Local.Add(newRow);
                            //   экипажиList.Add(newRow);
                            label1.Visibility = Visibility;
                            viewSource1.View.Refresh();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Отметьте участников");
                    }
                }


            }

        }
    }
}
