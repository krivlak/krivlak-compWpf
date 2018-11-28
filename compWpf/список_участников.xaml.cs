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
    /// Логика взаимодействия для список_участников.xaml
    /// </summary>
    public partial class список_участников : Window
    {
        public список_участников()
        {
            InitializeComponent();
        }
        compWpf.Entities de = new Entities();
        System.Windows.Data.CollectionViewSource viewSource1;
      //  List<туристы> туристыList;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                viewSource1 = new CollectionViewSource();
                de.туристы.Where(n => n.слет == клСлет.слет).OrderBy(n => n.школы.порядок).ThenBy(n=>n.фамилия).Load();
               // туристыList = de.туристы.Where(n => n.слет == клСлет.слет).OrderBy(n => n.фамилия).ToList();

                viewSource1.Source = de.туристы.Local.ToBindingList();
             //   viewSource1.Source = туристыList;

                dataGrid1.ItemsSource = viewSource1.View;

                //наимен_слета.Text = "Туристы " + клСлет.наимен ;
                dataGrid1.Focus();
            }
            catch (Exception ex)
            {
               // Clipboard.SetText(ex.Message);
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
            //int maxPor = 0;
            //if (de.туристы.Local.Any())
            //{
            //    maxPor = de.туристы.Local.Max(n => n.порядок);
            //}
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
                     школы= выб_школа
                };
                de.туристы.Local.Add(newRow);
             //   туристыList.Add(newRow);
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
                    // Guid кодТуриста = delRow.турист;
                    //int всегоТуристов = de.туристы.Count(n => n.школа == кодтуристы);
                    //int всего = de.экипажи.Count(n => n. == кодтуристы);

                    if (delRow.экипажей == 0)
                    {
                        de.туристы.Local.Remove(delRow);
                       // туристыList.Remove(delRow);

                        label1.Visibility = Visibility.Visible;
                        viewSource1.View.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("Предварительно удалите туриста из экипажей");
                    }
                }

            }
            dataGrid1.Focus();

        }

        private void Выбор_Click(object sender, RoutedEventArgs e)
        {
            if (!viewSource1.View.IsEmpty)
            {
                туристы dRow = viewSource1.View.CurrentItem as туристы;
                клТурист.deRow = dRow;
                клТурист.турист = dRow.турист;
                клТурист.фамилия = dRow.фамилия;
                this.DialogResult = true;
                
            }
            Close();
        }
    }
}
