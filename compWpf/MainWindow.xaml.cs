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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using System.IO;


namespace compWpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //ImageBrush imageBrush = new ImageBrush()
            //{
            //    ImageSource = @"c:\vs2017\compWpf\compWpf\bin\\Debug\x_75c67d97.jpg"
            //};
            
            //this.Background = imageBrush;
            init_слет();
        }

        private void Слеты_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            список_слетов списокШкол = new список_слетов();
            списокШкол.ShowDialog();
            init_слет();
            Cursor = null;
        }

        private void Виды_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            список_видов списокШкол = new список_видов();
            списокШкол.Выбор.Visibility = Visibility.Hidden;
            списокШкол.ShowDialog();
         //   init_слет();
            Cursor = null;
        }

        private void Школы_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
          //  клБаза.de = new Entities();
         //   клБаза.de.школы.OrderBy(n => n.порядок).Load();
            список_школ списокШкол = new список_школ();
            списокШкол.Выбор.Visibility = Visibility.Hidden;
            списокШкол.наимен_слета.Text = " Список Школ, клубов  " ;
            списокШкол.ShowDialog();
            Cursor = null;
        }

        private void Туристы_Click(object sender, RoutedEventArgs e)
        {
            //клШкола.выбран = false;
            //выбор_школы выборВида = new выбор_школы();
            //выборВида.ShowDialog();

            //if (выборВида.DialogResult == true || клВид.выбран)
            //{

            //    Cursor = Cursors.Wait;
            //    список_туристов списокШкол = new список_туристов();
            //    списокШкол.ShowDialog();
            //    Cursor = null;
            //}
            Cursor = Cursors.Wait;
     //       клБаза.de = new Entities();
          //  клБаза.de.туристы.Where(n => n.слет == клСлет.слет).OrderBy(n => n.фамилия).Load();
            список_участников списокШкол = new список_участников();
         //   списокШкол.выбранColumn.Visibility = Visibility.Collapsed;
            списокШкол.наимен_слета.Text = "Туристы на " + клСлет.наимен;
            списокШкол.Выбор.Visibility = Visibility.Hidden;
            списокШкол.ShowDialog();
            Cursor = null;
        }

        private void Дистанции_Click(object sender, RoutedEventArgs e)
        {
            //клВид.выбран = false;
            //выборListView выборВида = new выборListView();
            //выборВида.ShowDialog();
            //if (выборВида.DialogResult == true || клВид.выбран)
            //{

            Cursor = Cursors.Wait;
        //    клБаза.de = new Entities();
            список_дистанций списокШкол = new список_дистанций();
            списокШкол.Выбор.Visibility = Visibility.Hidden;
            списокШкол.Title = "Дистанции на " + клСлет.наимен;
            списокШкол.наимен_слета.Text = списокШкол.Title;
            списокШкол.ShowDialog();
            Cursor = null;
            //}
        }

        private void Этапы_Click(object sender, RoutedEventArgs e)
        {
            //клВид.выбран = false;
            //Выбор_дистанции выборВида = new Выбор_дистанции() ;
            //выборВида.ShowDialog();

            список_дистанций выборВида = new список_дистанций();
            выборВида.Title = "Выберите дистанцию";
            выборВида.наимен_слета.Text = выборВида.Title;
            выборВида.Выход.Content = "Отмена";
            выборВида.ShowDialog();

            if (выборВида.DialogResult == true || клВид.выбран)
            {

                Cursor = Cursors.Wait;
                список_этапов списокШкол = new список_этапов();
                списокШкол.ShowDialog();
                Cursor = null;
            }
        }

        private void Заявки_Click(object sender, RoutedEventArgs e)
        {
          //  клБаза.de = new Entities();
           // клВид.выбран = false;
            список_дистанций выборВида = new список_дистанций();
            выборВида.Title = "Выберите дистанцию";
            выборВида.наимен_слета.Text = выборВида.Title;
            выборВида.Выход.Content = "Отмена";
            выборВида.ShowDialog();

            if (выборВида.DialogResult == true )
            {

                Cursor = Cursors.Wait;
               
                список_экипажей списокШкол = new список_экипажей();
                списокШкол.ShowDialog();
                Cursor = null;
            }
        }

        private void штрафы_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Результаты_Click(object sender, RoutedEventArgs e)
        {
            клВид.выбран = false;
            Выбор_дистанции выборВида = new Выбор_дистанции();
            выборВида.ShowDialog();

            if (выборВида.DialogResult == true || клВид.выбран)
            {

                Cursor = Cursors.Wait;
                список_результатов списокШкол = new список_результатов();
                списокШкол.ShowDialog();
                Cursor = null;
            }
        }

        private void Все_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Выход_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void init_слет()
        {
            try
            {
                Entities de = new Entities();
                if (de.слеты.Any())
                {
                    клСлет.deRow = de.слеты.OrderBy(n => n.порядок).First();
                    клСлет.слет = клСлет.deRow.слет;
                    клСлет.наимен = клСлет.deRow.наимен;
                    //              this.Text = "Результаты соревнований на " + клСлет.наимен.Trim() + "  " + клСлет.deRow.дата_с.ToLongDateString();
                    this.Title = "Результаты соревнований на " + клСлет.наимен.Trim() + "  " + клСлет.deRow.дата_с.ToLongDateString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сбой " + ex.Message);
            }
        }

        private void проба_Click(object sender, RoutedEventArgs e)
        {
            проба проба22 = new проба();
            проба22.ShowDialog();
        }

        private void источник_Click(object sender, RoutedEventArgs e)
        {
            источник источник55 = new источник();
            источник55.ShowDialog();
        }

        private void winForm_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void заявкиWin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void РезультатыWin_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            выборVдистанции списокШкол = new выборVдистанции();
            списокШкол.ShowDialog();
            Cursor = null;
        }

        private void штрафыWin_Click(object sender, RoutedEventArgs e)
        {
           // клБаза.de = new Entities();
            список_дистанций выборВида = new список_дистанций();
            выборВида.Выход.Content = " Отмена";
            выборВида.Title = " Выберите дистанцию";
            выборВида.наимен_слета.Text = выборВида.Title;
            выборВида.ShowDialog();
            if (выборВида.DialogResult == true)
            {
                Cursor = Cursors.Wait;
                окно2результатов списокСлетов = new окно2результатов();
                списокСлетов.Text = клДистанция.наимен.Trim();
            //    списокСлетов.Tag = клДистанция.дистанция;
                списокСлетов.ShowDialog();
                Cursor = null;
            }

            //Cursor = Cursors.Wait;
            //выбор2Vдистанции списокШкол = new выбор2Vдистанции();
            //списокШкол.ShowDialog();
            //Cursor = null;
        }

        private void дата44_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            проба_календ списокШкол = new проба_календ();
            списокШкол.ShowDialog();
            Cursor = null;
        }

        private void выбор33_Click(object sender, RoutedEventArgs e)
        {
            выборListвида выбор55 = new выборListвида();
            выбор55.ShowDialog();
        }

        private void выбор44_Click(object sender, RoutedEventArgs e)
        {
            выборListView выбор55 = new выборListView();
            выбор55.ShowDialog();
        }

        private void выбор55_Click(object sender, RoutedEventArgs e)
        {
            пробаTab выбор55 = new пробаTab();
            выбор55.ShowDialog();
        }

        private void секундомер55_Click(object sender, RoutedEventArgs e)
        {
            секундомер выбор55 = new секундомер();
            выбор55.ShowDialog();
        }

        private void клава44_Click(object sender, RoutedEventArgs e)
        {
            клава выбор55 = new клава();
            выбор55.ShowDialog();
        }

        private void копировать4_Click(object sender, RoutedEventArgs e)
        {
            команды выбор55 = new команды();
            выбор55.ShowDialog();
        }

        private void все_Click_1(object sender, RoutedEventArgs e)
        {
          //  клБаза.de = new Entities();
            список_дистанций выборВида = new список_дистанций();
            выборВида.ShowDialog();
            if (выборВида.DialogResult == true )
            {
                все_результаты всеРезультаты = new все_результаты();
                всеРезультаты.Text = " Результаты на " + клСлет.наимен;
                всеРезультаты.ShowDialog();
            }
        }

        private void Суда_Click(object sender, RoutedEventArgs e)
        {
            //клВид.выбран = false;
            //Выбор_дистанции выборВида = new Выбор_дистанции();
            //выборВида.ShowDialog();

            //if (выборВида.DialogResult == true || клВид.выбран)
            //{


                Cursor = Cursors.Wait;
              //  клБаза.de = new Entities();
             //   клБаза.de.суда.OrderBy(n => n.порядок).Load();
                список_судов списокШкол = new список_судов();
                списокШкол.Выбор.Visibility = Visibility.Hidden;
                списокШкол.Title = " Список судов   "  ;
                списокШкол.наимен_слета.Text = списокШкол.Title;
                списокШкол.ShowDialog();
                Cursor = null;
            //}
        }

        private void попытки_Click(object sender, RoutedEventArgs e)
        {
            //клБаза.de = new Entities();
            клВид.выбран = false;
            список_дистанций выборВида = new список_дистанций();
            выборВида.Title = "Выберите дистанцию";
            выборВида.наимен_слета.Text = выборВида.Title;
            выборВида.Выход.Content = "Отмена";
            выборВида.ShowDialog();

            if (выборВида.DialogResult == true || клВид.выбран)
            {

                Cursor = Cursors.Wait;

                результаты1дистанция форма28 = new результаты1дистанция();
                форма28.ShowDialog();
                Cursor = null;
            }

           
        }

        private void в_архив_Click(object sender, RoutedEventArgs e)
        {
            string curDir = System.IO.Directory.GetCurrentDirectory();

            string шаблон = curDir + @"\tvt.sdf";
            if (!System.IO.File.Exists(шаблон.ToString()))
            {
                MessageBox.Show("Нет файла " + шаблон.ToString());
                return;
            }
            string каталог = @"E:\\архив\tvt\";
            if(! System.IO.Directory.Exists(каталог))
            {

                System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();

                System.Windows.Forms.DialogResult result = folderBrowser.ShowDialog();

                if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
                {
                    //string[] files = Directory.GetFiles(folderBrowser.SelectedPath);
                    каталог = folderBrowser.SelectedPath+@"\";
                }
                else
                {
                    return;
                }
            }

            try
            {

                DateTime дата = DateTime.Now;
                string ss = дата.Day.ToString() + "_" + дата.Month.ToString() + "_" + дата.Year.ToString()+"-"+дата.Hour.ToString()+"-"+дата.Minute.ToString();
                string файл_архива = каталог+ss+".sdf";
                File.Copy(шаблон, файл_архива);
                MessageBox.Show("Архив создан "+ файл_архива);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Сбой копирования" + ex.Message);
            }

        }

       
    }
}
