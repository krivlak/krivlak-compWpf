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
using Word = Microsoft.Office.Interop.Word;

namespace compWpf
{
    /// <summary>
    /// Логика взаимодействия для список_результатов.xaml
    /// </summary>
    public partial class список_результатов : Window
    {
        public список_результатов()
        {
            InitializeComponent();
        }

        compWpf.Entities de = new Entities();
        System.Windows.Data.CollectionViewSource viewSource1 = new CollectionViewSource();
    //    BindingList<результаты> binList;
        //дистанции deДистанция;
        //Guid кодДистанции;
        //      System.Windows.Threading.DispatcherTimer timer1 = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
   //     System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();

        // System.Threading.Timer timer1;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
            //    deДистанция = клДистанция.deRow;
            //кодДистанции = deДистанция.дистанция;
         //   viewSource1 = new CollectionViewSource();
                de.экипажи.Where(n => n.дистанция == клДистанция.дистанция)
                    .OrderBy(n => n.номер).Load();
                de.результаты.Where(n => n.экипажи.дистанция == клДистанция.дистанция)
                    .OrderBy(n => n.порядок).ThenBy(n => n.экипажи.номер).ThenBy(n => n.попытка).Load();

             //   binList = de.результаты.Local.ToBindingList();

                foreach (экипажи eRow in de.экипажи.Local)
                {

                    if (de.результаты.Local.Where(n => n.экипаж == eRow.экипаж).Count() == 0)
                    {
                        int maxPor = 0;
                        if (de.результаты.Local.Any())
                        {
                            maxPor = de.результаты.Local.Max(n => n.порядок);
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
                            финиш = DateTime.Today,
                             прим=""
                        };


                    de.результаты.Local.Add(newRow);

                    }

                }

         
         //       timer1 = new System.Threading.Timer()

                viewSource1.Source = de.результаты.Local.ToBindingList();
                dataGrid1.ItemsSource = viewSource1.View;
                наимен_слета.Text ="Результаты "+клДистанция.наимен + "   " + клСлет.наимен;

                пересчет();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки " + ex.Message);
            }



            //   TimeSpan ts = new TimeSpan(0,0,0,0,100);
    //        System.Threading.TimerCallback timerCallback = new System.Threading.TimerCallback(Timer1_Tick());
            timer1.Interval = 1000;
            timer1.Start();

            timer1.Tick += Timer1_Tick;

        //    timer2.Interval = 100;
        ////    timer2.Start();
        //    timer2.Tick += Timer2_Tick;

            this.Closed += Список_школ_Closed;
            viewSource1.View.CollectionChanged += View_CollectionChanged;
            //   dataGrid1.CellEditEnding += DataGrid1_CellEditEnding;
            //    de.результаты.Local.CollectionChanged += Local_CollectionChanged;
            //      viewSource1.View.CurrentChanging += View_CurrentChanging;

            dataGrid1.CellEditEnding += DataGrid1_CellEditEnding;
            dataGrid1.MouseLeftButtonDown += DataGrid1_MouseLeftButtonDown;
          //  this.стартColumn.
           
          //  результаты.Moving += Результаты_Moving;

        }

        private void DataGrid1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        //private void Результаты_Moving(результаты obj)
        //{
        //    label1.Visibility = Visibility.Visible;
        //}

        //private void Timer2_Tick(object sender, EventArgs e)
        //{
        //    viewSource1.View.Refresh();
        //}

        private void DataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                label1.Visibility = Visibility.Visible;
            }
        }

        private void View_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            результаты oldRow = viewSource1.View.CurrentItem as результаты;
            if (de.Entry(oldRow).State != EntityState.Unchanged)
            {
                label1.Visibility = Visibility.Visible;
       //         пересчет();

            }
        }

        //private void Local_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    label1.Visibility = Visibility.Visible;
        //    пересчет();
        //}

        //private void DataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    результаты  oldRow = viewSource1.View.CurrentItem as результаты;
        //    if (de.Entry(oldRow).State != EntityState.Unchanged)
        //    {
        //        label1.Visibility = Visibility.Visible;
        //       // пересчет();

        //    }
        //}

        private void пересчет()
        {
            foreach (результаты rRow in de.результаты.Local)
            {
                rRow.секунд = rRow.время_мин * 60 + rRow.время_сек;
                rRow.итог = rRow.секунд + rRow.штраф;
            }

            var query = de.результаты.Local
                .Where(n => n.итог > 0)
                .GroupBy(n => n.экипаж)
                .Select(n => new { экипаж = n.Key, лучший = n.Min(p => p.итог) }).ToDictionary(n => n.экипаж);

            //           
            foreach (результаты rRow in de.результаты.Local)
            {
                rRow.зачетный = false;
                rRow.экипажи.итог = 0;
                rRow.экипажи.место = 0;
                if (query.ContainsKey(rRow.экипаж))
                {
                    rRow.экипажи.итог = query[rRow.экипаж].лучший;
                    if (rRow.итог == query[rRow.экипаж].лучший)
                    {
                        rRow.зачетный = true;
                    }
                }
                
            }

            foreach (экипажи uRow in de.экипажи.Local)
            {
                uRow.место = 0;
            }
            int j = 0;
            foreach (экипажи uRow in de.экипажи.Local.Where(n => n.итог > 0).OrderBy(n => n.итог))
            {
                j++;
                uRow.место = j;


            }
        //    viewSource1.Source = de.результаты.Local.ToBindingList();
            viewSource1.View.Refresh();
            label1.Visibility = Visibility.Visible;
           
        }


        private void View_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            label1.Visibility = Visibility.Visible;
         //   пересчет();
        }

        private void Список_школ_Closed(object sender, EventArgs e)
        {
            foreach(экипажи eRow in de.экипажи.Local)
            {
                if(de.Entry(eRow).State != EntityState.Unchanged)
                {
                    label1.Visibility = Visibility.Visible;
                }
            }

            foreach (результаты кRow in de.результаты.Local)
            {
                if (de.Entry(кRow).State != EntityState.Unchanged)
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
            клШкола.выбран = false;
            список_школ выборШколы = new список_школ();
            выборШколы.ShowDialog();
            if (выборШколы.DialogResult == true)
            {
                список_судов выборСудов = new список_судов();
                выборСудов.ShowDialog();
                if(выборСудов.DialogResult==true)
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
                        дистанция = клДистанция.дистанция,
                        школы = выбр_школа,
                         судно=клСудно.судно,
                          суда= выбр_судно

                    };

                    int maxPor3 = 0;
                    if (de.результаты.Local.Any())
                    {
                        maxPor3 = de.результаты.Local.Max(n => n.номер);
                    }

                    de.экипажи.Local.Add(newRow);
                    результаты newRez = new результаты()
                    {
                        зачетный = false,
                        итог = 0,
                        время_сек = 0,
                        время_мин = 0,
                        попытка = 1,
                        порядок = maxPor3,
                        результат = Guid.NewGuid(),
                        секунд = 0,
                        старт = DateTime.Today,
                        финиш = DateTime.Today,
                        штраф = 0,
                        экипаж = newRow.экипаж,
                         прим=""
                    };
                    de.результаты.Local.Add(newRez);
                    label1.Visibility = Visibility.Visible;
                }
            }

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

                        label1.Visibility = Visibility;
                    }
                    else
                    {
                        MessageBox.Show("Предварительно удалите попытки этого экипажа");
                    }
                }

            }

        }

        private void Вверх_Click(object sender, RoutedEventArgs e)
        {

            if (!viewSource1.View.IsEmpty)
            {
                экипажи oldRow = viewSource1.View.CurrentItem as экипажи;

            //    int oldPor = oldRow.номер;
                if (viewSource1.View.CurrentPosition > 0)
                {
                    viewSource1.View.MoveCurrentToPrevious();

                    экипажи lastRow = viewSource1.View.CurrentItem as экипажи;
                    (oldRow.номер, lastRow.номер) = (lastRow.номер, oldRow.номер);
                    //int lastPor = lastRow.номер;
                    //oldRow.номер = lastPor;
                    //lastRow.номер = oldPor;

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

               // int oldPor = oldRow.номер;
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
            if (de.экипажи.Local.Any())
            {
                экипажи eRow = viewSource1.View.CurrentItem as экипажи;
                клТурист.выбран = false;
                выбор_туриста выборТуриста = new выбор_туриста();
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
        }

        private void список_Click(object sender, RoutedEventArgs e)
        {

            if (de.экипажи.Local.Any())
            {
                //  экипажи eRow = viewSource1.View.CurrentItem as экипажи;
                клТурист.выбран = false;
                выборVтуристов выборТуриста = new выборVтуристов();
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
                            экипажи newRow = new экипажи()
                            {
                                итог = 0,
                                //дистанция = клДистанция.дистанция,
                                место = 0,
                                номер = maxPor + 1,
                                прим = "",
                                школа = tRow.школа,
                                экипаж = Guid.NewGuid()
                            };

                            туристы newTur = de.туристы.Single(n => n.турист == tRow.турист);
                            newRow.туристы.Add(newTur);
                            de.экипажи.Local.Add(newRow);
                            label1.Visibility = Visibility;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Отметьте участников");
                    }


                }
            }
        }

        private void одиночка_Click(object sender, RoutedEventArgs e)
        {
            if (de.экипажи.Local.Any())
            {
                //  экипажи eRow = viewSource1.View.CurrentItem as экипажи;
                клТурист.выбран = false;
                выбор_туриста выборТуриста = new выбор_туриста();
                выборТуриста.ShowDialog();
                if (выборТуриста.DialogResult == true)
                {
                    int maxPor = 0;
                    if (de.экипажи.Local.Any())
                    {
                        maxPor = de.экипажи.Local.Max(n => n.номер);
                    }
                    экипажи newRow = new экипажи()
                    {
                        итог = 0,
                        //дистанция = клДистанция.дистанция,
                        место = 0,
                        номер = maxPor + 1,
                        прим = "",
                        школа = клТурист.deRow.школа,
                        экипаж = Guid.NewGuid()

                    };

                    туристы newTur = de.туристы.Single(n => n.турист == клТурист.турист);
                    newRow.туристы.Add(newTur);
                    de.экипажи.Local.Add(newRow);
                    //      viewSource1.View.MoveCurrentTo(newTur);

                    label1.Visibility = Visibility;

                }
            }
            dataGrid1.Focus();
        }

        private void пересчет_Click(object sender, RoutedEventArgs e)
        {
            пересчет();
        }
        private void NumbersOnly(object sender, TextCompositionEventArgs e)
        {
            int val;
            if (!Int32.TryParse(e.Text, out val) && e.Text != "-")
            {
                e.Handled = true; // отклоняем ввод
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        //    пересчет();
        }

        private void word44_Click(object sender, RoutedEventArgs e)
        {
            Word.Application oWord = new Word.Application();

            string curDir = System.IO.Directory.GetCurrentDirectory();

            object шаблон = curDir + @"\протокол.dot";
            if (!System.IO.File.Exists(шаблон.ToString()))
            {
                MessageBox.Show("Нет файла " + шаблон.ToString());
                return;
            }

            try
            {

                Word.Document o = oWord.Documents.Add(Template: шаблон);


                Word.Table tab1 = o.Tables[1];
                Word.Table tab2 = o.Tables[2];
                oWord.Visible = true;


                tab1.Rows[1].Cells[1].Range.Text = наимен_слета.Text;
                int j = 1;
                foreach (результаты pRow in de.результаты.Local.Where(n => n.лучший > 0).Where(n => n.зачетный).OrderBy(n => n.место))
                {
                    j++;
                    tab2.Rows[j].Cells[1].Range.Text = pRow.номер.ToString();
                    tab2.Rows[j].Cells[2].Range.Text = pRow.клуб;
                    tab2.Rows[j].Cells[3].Range.Text = pRow.состав;
                    tab2.Rows[j].Cells[4].Range.Text = pRow.время_мин.ToString();
                    tab2.Rows[j].Cells[5].Range.Text = pRow.время_сек.ToString();
                    tab2.Rows[j].Cells[6].Range.Text = pRow.секунд.ToString();
                    tab2.Rows[j].Cells[7].Range.Text = pRow.штраф.ToString();
                    tab2.Rows[j].Cells[8].Range.Text = pRow.итог.ToString();
                    tab2.Rows[j].Cells[9].Range.Text = pRow.место.ToString();



                    tab2.Rows.Add();

                }


                oWord.Visible = true;
            }
            catch (Exception ex)
            {

                oWord.DisplayAlerts = Word.WdAlertLevel.wdAlertsNone;
                oWord.Application.Quit(SaveChanges: false);
                MessageBox.Show("Сбой Word " + ex.Message);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            результаты rRow = viewSource1.View.CurrentItem as результаты;
            Button bt = (Button)sender;
         //   bt.Content = "Финиш";

            if (rRow.плывут)
            {
                rRow.плывут = false;
                пересчет();
                bt.Content = "Старт";
                viewSource1.View.Refresh();
            }
            else
            {
                rRow.старт = DateTime.Now;
                rRow.финиш = rRow.старт;
                rRow.плывут = true;
                bt.Content = "Финиш"+rRow.миллисекунд.ToString();
            }
            viewSource1.View.Refresh();


        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                int j = 0;
                foreach (результаты rRow in de.результаты.Local.Where(n=>n.плывут))
                {
                    j++;
                        rRow.финиш = DateTime.Now;
                        var rtr = (rRow.финиш - rRow.старт);
                        rRow.время_мин = rtr.Minutes;
                        rRow.время_сек = rtr.Seconds;
                        rRow.миллисекунд = rtr.Milliseconds;
                   
                }
                if (j > 0)
                {
                  //  viewSource1.View.Refresh();
                }
            }
        }

        private void старт77_Click(object sender, RoutedEventArgs e)
        {
         //   if (!viewSource1.View.IsEmpty)
         //   {
         //       результаты rRow = viewSource1.View.CurrentItem as результаты;
         //       if (rRow.плывут)
         //       {
         //           rRow.плывут = false;
         //           пересчет();
         ////           viewSource1.View.Refresh();
         //       }
         //       else
         //       {
         //           rRow.старт = DateTime.Now;
         //           rRow.финиш = rRow.старт;
         //           rRow.плывут = true;
         //       }
         //   }
        }
    }
}
