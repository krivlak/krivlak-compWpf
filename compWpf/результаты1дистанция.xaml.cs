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
    /// Логика взаимодействия для результаты1дистанция.xaml
    /// </summary>
    public partial class результаты1дистанция : Window
    {
        public результаты1дистанция()
        {
            InitializeComponent();
        }
        compWpf.Entities de = new Entities();
        List<экипажи> экипажиList;
        List<результаты> результатыList;

        System.Windows.Data.CollectionViewSource viewSource1;
        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
//        System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {


                viewSource1 = new CollectionViewSource();

                экипажиList = de.экипажи.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.номер).ToList();

                результатыList = de.результаты.Where(n => n.экипажи.дистанция == клДистанция.дистанция)
                    .OrderBy(n => n.порядок).ThenBy(n => n.экипажи.номер).ThenBy(n => n.попытка).ToList();



                foreach (экипажи eRow in экипажиList)
                {

                    if (результатыList.Where(n => n.экипаж == eRow.экипаж).Count() == 0)
                    {
                        int maxPor = 0;
                        if (результатыList.Any())
                        {
                            maxPor = результатыList.Max(n => n.порядок);
                        }
                        результаты newRow = new результаты
                        {
                            итог = 0,
                            время_сек = 0,
                            время_мин = 0,
                            попытка = 1,
                            результат = Guid.NewGuid(),
                            секунд = 0,
                            штраф = 0,
                            экипаж = eRow.экипаж,
                            экипажи = eRow,
                            зачетный = false,
                            порядок = maxPor + 1,
                            старт = DateTime.Today,
                            финиш = DateTime.Today
                        };

                        de.результаты.Add(newRow);
                        результатыList.Add(newRow);
                        label1.Visibility = Visibility.Visible;

                    }

                }

                viewSource1.Source = результатыList;
                dataGrid1.ItemsSource = viewSource1.View;
                наимен_слета.Text = "Экипажи " + клДистанция.наимен + "   " + клСлет.наимен;
                dataGrid1.Focus();

                //пересчет();

                //bindingSource1.DataSource = binList;
                //bindingSource1.Sort = " номер, попытка";


                this.Closed += Список_школ_Closed;
                timer1.Interval = 100;
                timer1.Start();

                timer1.Tick += Timer1_Tick;

             //   timer2.Interval = 100;
              //  timer2.Start();
             //   timer2.Tick += Timer2_Tick;
                //viewSource1.View.CollectionChanged += View_CollectionChanged;
                //viewSource1.View.CurrentChanging += View_CurrentChanging;

                //dataGrid1.CellEditEnding += DataGrid1_CellEditEnding;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки " + ex.Message);
            }
        }
        private void Timer2_Tick(object sender, EventArgs e)
        {
            viewSource1.View.Refresh();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
               // результаты tRow = viewSource1.View.CurrentItem as результаты;

                foreach (var dRow in dataGrid1.Items)
                {
                    результаты sRow = dRow as результаты;
                    if (sRow.плывут)
                    {
                        sRow.финиш = DateTime.Now;
                        TimeSpan rtr = (sRow.финиш - sRow.старт);
                        sRow.время_мин = rtr.Minutes;
                        sRow.время_сек = rtr.Seconds;
                        sRow.миллисекунд = rtr.Milliseconds;
                        Console.WriteLine(sRow.время_сек);
                    }
                }


                //foreach (результаты sRow in результатыList.Where(n => n.плывут))
                //{
                   
                //    sRow.финиш = DateTime.Now;
                //    TimeSpan rtr = (sRow.финиш - sRow.старт);
                //    sRow.время_мин = rtr.Minutes;
                //    sRow.время_сек = rtr.Seconds;
                //    sRow.миллисекунд = rtr.Milliseconds;
                //    Console.WriteLine(sRow.время_сек);
                //}
            //    viewSource1.View.Refresh();
              //  viewSource1.View.MoveCurrentTo(tRow);
            }
        }

        private void Список_школ_Closed(object sender, EventArgs e)
        {
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
            выборШколы.наимен_слета.Text = " Выберите школу, клуб";
            выборШколы.Выход.Content = "Отмена";
            выборШколы.ShowDialog();
            if (выборШколы.DialogResult == true)
            {

                список_судов выборСудна = new список_судов();

                выборСудна.Title = "Выберите судно";
                выборСудна.наимен_слета.Text = "Выберите судно";

                выборСудна.Выход.Content = "Отмена";
                выборСудна.ShowDialog();
                if (выборСудна.DialogResult == true)
                {
                    int maxPor = 0;
                    if (экипажиList.Any())
                    {
                        maxPor = экипажиList.Max(n => n.номер);
                    }
                    школы выбр_школа = de.школы.Single(n => n.школа == клШкола.школа);
                    суда выбр_судно = de.суда.Single(n => n.судно == клСудно.судно);
                    экипажи newЭкипаж = new экипажи
                    {
                        номер = maxPor + 1,
                        экипаж = Guid.NewGuid(),
                        прим = "",
                        школа = клШкола.школа,
                        итог = 0,
                        место = 0,
                        судно = клСудно.судно,
                        //  школы = выбр_школа,
                        суда = клСудно.deRow,
                        дистанция = клДистанция.дистанция
                        //суда = выбр_судно
                    };
                    de.экипажи.Add(newЭкипаж);
                    экипажиList.Add(newЭкипаж);

                    результаты newRow = new результаты
                    {
                        итог = 0,
                        время_сек = 0,
                        время_мин = 0,
                        попытка = 1,
                        результат = Guid.NewGuid(),
                        секунд = 0,
                        штраф = 0,
                        экипаж = newЭкипаж.экипаж,
                        экипажи = newЭкипаж,
                        зачетный = false,
                        порядок = maxPor + 1,
                        старт = DateTime.Today,
                        финиш = DateTime.Today
                    };
                    de.результаты.Add(newRow);
                    результатыList.Add(newRow);
                    viewSource1.View.MoveCurrentTo(newRow);
                    viewSource1.View.Refresh();
                    label1.Visibility = Visibility.Visible;
                }
            }
            dataGrid1.Focus();

        }
    

        private void новый_матрос_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                результаты  rRow = viewSource1.View.CurrentItem as результаты;
                клТурист.выбран = false;
                список_участников выборТуриста = new список_участников();
                выборТуриста.Title = "Выберите участника";
                выборТуриста.Выход.Content = "Отмена";
                выборТуриста.ShowDialog();
                if (выборТуриста.DialogResult == true)
                {
                    экипажи eRow = de.экипажи.Single(n => n.экипаж == rRow.экипаж);
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

        private void Вверх_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                результаты oldRow = viewSource1.View.CurrentItem as результаты;

                int oldPor = oldRow.порядок;
                if (viewSource1.View.CurrentPosition > 0)
                {
                    viewSource1.View.MoveCurrentToPrevious();

                    результаты lastRow = viewSource1.View.CurrentItem as результаты;
                    int lastPor = lastRow.порядок;
                    oldRow.порядок = lastPor;
                    lastRow.порядок = oldPor;

                    label1.Visibility = Visibility;
                    dataGrid1.Items.Refresh();

                    dataGrid1.Items.SortDescriptions.Clear();

                    dataGrid1.Items.SortDescriptions.Add(new SortDescription("порядок", ListSortDirection.Ascending));
                    viewSource1.View.MoveCurrentToPrevious();
                    label1.Visibility = Visibility;
                }
            }

            dataGrid1.Focus();
        }

        private void вНиз_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                результаты oldRow = viewSource1.View.CurrentItem as результаты;

                int oldPor = oldRow.порядок;
                if (viewSource1.View.CurrentPosition < de.результаты.Local.Count - 1)
                {
                    viewSource1.View.MoveCurrentToNext();
                    результаты lastRow = viewSource1.View.CurrentItem as результаты;
                    int lastPor = lastRow.порядок;
                    oldRow.порядок = lastPor;
                    lastRow.порядок = oldPor;
                    label1.Visibility = Visibility;
                    dataGrid1.Items.Refresh();

                    dataGrid1.Items.SortDescriptions.Clear();

                    dataGrid1.Items.SortDescriptions.Add(new SortDescription("порядок", ListSortDirection.Ascending));
                    viewSource1.View.MoveCurrentToNext();
                    label1.Visibility = Visibility;
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
                результаты delRow = viewSource1.View.CurrentItem as результаты;
                if (delRow != null)
                {

                    Guid кодЭкипажа = delRow.экипаж;
                    var ar = результатыList.Where(n => n.экипаж == delRow.экипаж).ToArray();
                    foreach (результаты tRow in ar)
                    {
                        de.результаты.Remove(tRow);
                        результатыList.Remove(tRow);
                    }
                    viewSource1.View.Refresh();
                    экипажи dRow = экипажиList.Single(n => n.экипаж == кодЭкипажа);
                    de.экипажи.Remove(dRow);
                    экипажиList.Remove(dRow);

                    label1.Visibility = Visibility;
                    viewSource1.View.Refresh();
                }
            }
            dataGrid1.Focus();
        }

        private void очистить_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                результаты  rRow = viewSource1.View.CurrentItem as результаты;
                экипажи eRow = de.экипажи.Single(n => n.экипаж == rRow.экипаж);
                eRow.туристы.Clear();
                label1.Visibility = Visibility;
                viewSource1.View.Refresh();
            }
            dataGrid1.Focus();
        }

        private void список_Click(object sender, RoutedEventArgs e)
        {
            список_судов выборСудна = new список_судов();

            выборСудна.Title = "Выберите судно";
            выборСудна.наимен_слета.Text = "Выберите судно";

            выборСудна.Выход.Content = "Отмена";
            выборСудна.ShowDialog();
            if (выборСудна.DialogResult == true)
            {
                клТурист.выбран = false;
                выборVтуристов выборТуриста = new выборVтуристов();
                выборТуриста.ShowDialog();
                if (выборТуриста.DialogResult == true)
                {
                    if (клТурист.turList.Count > 0)
                    {
                        foreach (туристы tRow in клТурист.turList)
                        {
                            int maxNum = 0;
                            if (экипажиList.Any())
                            {
                                maxNum = экипажиList.Max(n => n.номер);
                            }
                            школы выбр_школа = de.школы.Single(n => n.школа == tRow.школа);
                            суда выбр_судно = de.суда.Single(n => n.судно == клСудно.судно);
                            экипажи newRow = new экипажи()
                            {
                                итог = 0,
                                дистанция = клДистанция.дистанция,
                                место = 0,
                                номер = maxNum + 1,
                                прим = "",
                                школа = tRow.школа,
                                экипаж = Guid.NewGuid(),
                                судно = клСудно.судно,
                                школы = выбр_школа,
                                суда = выбр_судно
                            };

                            туристы newTur = de.туристы.Single(n => n.турист == tRow.турист);
                            newRow.туристы.Add(newTur);
                            de.экипажи.Add(newRow);
                            экипажиList.Add(newRow);


                            int maxPor = 0;
                            if (результатыList.Any())
                            {
                                maxPor = результатыList.Max(n => n.порядок);
                            }

                            результаты newRos = new результаты
                            {
                                итог = 0,
                                время_сек = 0,
                                время_мин = 0,
                                попытка = 1,
                                результат = Guid.NewGuid(),
                                секунд = 0,
                                штраф = 0,
                                экипаж = newRow.экипаж,
                                экипажи = newRow,
                                зачетный = false,
                                порядок = maxPor + 1,
                                старт = DateTime.Today,
                                финиш = DateTime.Today
                            };
                            de.результаты.Add(newRos);
                            результатыList.Add(newRos);

                            label1.Visibility = Visibility;
                        }
                        viewSource1.View.Refresh();

                    }
                    else
                    {
                        MessageBox.Show("Отметьте участников");
                    }
                }


            }
            dataGrid1.Focus();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            результаты rRow = viewSource1.View.CurrentItem as результаты;
            Button bt = (Button)sender;
            //   bt.Content = "Финиш";

            if (rRow.плывут)
            {
                rRow.плывут = false;
              //  пересчет();
                bt.Content = "Старт";
         
            }
            else
            {
                rRow.старт = DateTime.Now;
                rRow.финиш = rRow.старт;
                rRow.плывут = true;
                bt.Content = "Финиш" + rRow.миллисекунд.ToString();
            }
            //viewSource1.View.Refresh();
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
                    int maxNum = 0;
                    if (экипажиList.Any())
                    {
                        // для этого нужен экипажиList
                        maxNum = экипажиList.Max(n => n.номер);
                    }
                    экипажи newRow = new экипажи()
                    {
                        итог = 0,
                        дистанция = клДистанция.дистанция,
                        место = 0,
                        номер = maxNum + 1,
                        прим = "",
                        школа = клТурист.deRow.школа,
                        экипаж = Guid.NewGuid(),
                        судно = клСудно.судно
                    };

                    туристы newTur = de.туристы.Single(n => n.турист == клТурист.турист);
                    newRow.туристы.Add(newTur);
                    de.экипажи.Add(newRow);
                    экипажиList.Add(newRow);

                    int maxPor = 0;
                    if (результатыList.Any())
                    {
                        maxPor = результатыList.Max(n => n.порядок);
                    }

                    результаты newRos = new результаты
                    {
                        итог = 0,
                        время_сек = 0,
                        время_мин = 0,
                        попытка = 1,
                        результат = Guid.NewGuid(),
                        секунд = 0,
                        штраф = 0,
                        экипаж = newRow.экипаж,
                        экипажи = newRow,
                        зачетный = false,
                        порядок = maxPor + 1,
                        старт = DateTime.Today,
                        финиш = DateTime.Today
                    };
                    de.результаты.Add(newRos);
                    результатыList.Add(newRos);
                    viewSource1.View.MoveCurrentTo(newRos);
                    label1.Visibility = Visibility;
                    viewSource1.View.Refresh();

                }
            }
            dataGrid1.Focus();
        }

        private void новая_поп_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                int maxPor = результатыList.Max(n => n.порядок);

                результаты tRow = viewSource1.View.CurrentItem as результаты;
                Guid кодЭкипажа = tRow.экипаж;
                int maxPop = результатыList.Max(n => n.попытка);

                экипажи dRow = экипажиList.Single(n => n.экипаж == кодЭкипажа);
                результаты нижняя = результатыList.Single(n => n.экипаж == tRow.экипаж && n.попытка == maxPop);
                int индекс = результатыList.IndexOf(нижняя);

                результаты newRow = new результаты
                {
                    итог = 0,
                    время_сек = 0,
                    время_мин = 0,
                    попытка = maxPop + 1,
                    результат = Guid.NewGuid(),
                    секунд = 0,
                    штраф = 0,
                    экипаж = tRow.экипаж,
                    экипажи = dRow,
                    зачетный = false,
                    порядок = maxPor + 1,
                    старт = DateTime.Today,
                    финиш = DateTime.Today
                };
          
                результатыList.Insert(индекс + 1, newRow);
                de.результаты.Add(newRow);
                label1.Visibility= Visibility.Visible;
                viewSource1.View.MoveCurrentTo(newRow);
                viewSource1.View.Refresh();

            }
            dataGrid1.Focus();
        }

        private void Удалить_поп_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                результаты tRow = viewSource1.View.CurrentItem as результаты;
                if (tRow.попытка > 1)
                {
                    de.результаты.Remove(tRow);
                    результатыList.Remove(tRow);

                    label1.Visibility = Visibility.Visible;
                    viewSource1.View.Refresh();
                    //  пересчет();
                }
            }
            dataGrid1.Focus();
        }
    }
}
