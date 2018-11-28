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
    /// Логика взаимодействия для выборVучастников.xaml
    /// </summary>
    public partial class выборVучастников : Window
    {
        public выборVучастников()
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
                de.туристы.Where(n => n.слет == клСлет.слет).OrderBy(n => n.фамилия).Load();

                viewSource1.Source = de.туристы.Local.ToBindingList();

                dataGrid1.ItemsSource = viewSource1.View;

                dataGrid1.Focus();
            }
            catch (Exception ex)
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

            if (e.Column == фамилияColumn)
            {

                var editedTextbox = e.EditingElement as TextBox;

                if (editedTextbox != null)
                {
                    string ss = editedTextbox.Text;
                    if (ss == String.Empty)
                    {

                        MessageBox.Show("Фамилия  не может быть пустой");
                        //   e.Cancel = true;
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
            foreach (туристы sRow in de.туристы.Local)
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
           
            список_школ выборШколы = new список_школ();
            выборШколы.Выход.Content = "Отмена";
            выборШколы.Title = " Выберите школу";
            выборШколы.наимен_слета.Text = выборШколы.Title;

            выборШколы.ShowDialog();
            if (выборШколы.DialogResult == true)
            {
                школы выб_школа = de.школы.Single(n => n.школа == клШкола.школа);
                туристы newRow = new туристы
                {
                    муж = true,
                    младший = false,
                    имя = "",
                    прим = "",
                    слет = клСлет.слет,
                    турист = Guid.NewGuid(),
                    фамилия = "Новый",
                    школа = клШкола.школа,
                    школы = выб_школа
                };
                de.туристы.Local.Add(newRow);
              
                viewSource1.View.Refresh();
                viewSource1.View.MoveCurrentTo(newRow);

            }
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
                туристы delRow = viewSource1.View.CurrentItem as туристы;
                if (delRow != null)
                {
                  
                    if (delRow.экипажей == 0)
                    {
                        de.туристы.Local.Remove(delRow);
                     

                        label1.Visibility = Visibility.Visible;
                        viewSource1.View.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("Предварительно удалите туриста из экипажей");
                    }
                }

            }

        }

        private void Выбор_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {

                клТурист.turList = new List<туристы>();
                foreach (туристы tRow in de.туристы.Local.Where(n => n.выбран))
                {
                    клТурист.turList.Add(tRow);
                }

                this.DialogResult = true;


            }
            Close();
        }

        private void выбрать_всех_Click(object sender, RoutedEventArgs e)
        {
            foreach(туристы tRow in de.туристы.Local)
            {
                tRow.выбран = true;
            }
            viewSource1.View.Refresh();
        }
    }
}
