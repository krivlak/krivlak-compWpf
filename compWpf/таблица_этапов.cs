using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;

namespace compWpf
{
    public partial class таблица_этапов : Form
    {
        public таблица_этапов()
        {
            InitializeComponent();
            this.Top = 0;
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            this.Left = 0;
        }
        Entities de = new Entities();
        BindingList<этапы> этапыList;
        private void таблица_этапов_Load(object sender, EventArgs e)
        {
            try
            {
                init_слет();
                //  this.Text = " Дистанции  " + клВид.наимен + " на " + клСлет.наимен;
                de.этапы.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.порядок).Load();

                этапыList= de.этапы.Local.ToBindingList();

                bindingSource1.DataSource = этапыList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки " + ex.Message);
            }

            label2.Text = клДистанция.наимен + "  " + клСлет.наимен;
            FormClosing += Список_видов_FormClosing;
            bindingSource1.ListChanged += BindingSource1_ListChanged;
            //dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            //  dataGridView1.CellMouseClick += DataGridView1_CellMouseClick;
            label2.Text = this.Text;
        }

        public void init_слет()
        {
            if (de.слеты.Any())
            {
                клСлет.deRow = de.слеты.OrderBy(n => n.порядок).First();
                клСлет.слет = клСлет.deRow.слет;
                клСлет.наимен = клСлет.deRow.наимен;

            }
        }

        private void Список_видов_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (label1.Visible)
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

        private void BindingSource1_ListChanged(object sender, ListChangedEventArgs e)
        {
            label1.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                этапы tRow = bindingSource1.Current as этапы;
                if (tRow.штрафы.Count == 0)
                {
                    bindingSource1.RemoveCurrent();
                }
                else
                {
                    MessageBox.Show("Преварительно удалите штрафы этого этапа");
                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int maxPor = 0;
            if (de.этапы.Local.Any())
            {
                maxPor = de.этапы.Local.Max(n => n.порядок);
            }
            этапы newRow = new этапы()
            {
                дистанция = клДистанция.дистанция,
                наимен = "Новый этап",
                порядок = maxPor + 1,
                этап = Guid.NewGuid(),
                судья = ""

            };
            int строка = bindingSource1.Add(newRow);
            bindingSource1.Position = строка;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                этапы oldRow = bindingSource1.Current as этапы;
                int oldIndex = bindingSource1.Position;

                int oldPor = oldRow.порядок;
                if (bindingSource1.Position > 0)
                {
                    bindingSource1.MovePrevious();

                    этапы lastRow = bindingSource1.Current as этапы;
                    int lastPor = lastRow.порядок;
                    oldRow.порядок = lastPor;
                    lastRow.порядок = oldPor;
                    //       этапы_деталейЛист.Sort((a, b) => a.порядок.CompareTo(b.порядок));
                    bindingSource1.Sort = "порядок";
                    dataGridView1.Refresh();

                    label1.Visible = true;
                }
            }
            dataGridView1.Focus();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                этапы oldRow = bindingSource1.Current as этапы;

                int oldPor = oldRow.порядок;
                if (bindingSource1.Position < bindingSource1.Count - 1)
                {
                    bindingSource1.MoveNext();
                    этапы lastRow = bindingSource1.Current as этапы;
                    int lastPor = lastRow.порядок;
                    oldRow.порядок = lastPor;
                    lastRow.порядок = oldPor;
                    bindingSource1.Sort = "порядок";

                    //   этапы_деталейЛист.Sort((a, b) => a.порядок.CompareTo(b.порядок));
                    dataGridView1.Refresh();
                    label1.Visible = true;

                }
            }
            dataGridView1.Focus();
        }

        private void button6_Click(object sender, EventArgs e)
        {
         //   дистанции oldДистанции = клДистанция.deRow;
            клДистанция.выбран = false;
            выбор_кода_дистанции выборДистанции = new выбор_кода_дистанции();
            выборДистанции.ShowDialog();



            if (клДистанция.выбран || выборДистанции.DialogResult == true)
            {
                Guid кодНов = (Guid)выборДистанции.Tag;
                string sqlString = "select наимен, судья, порядок from этапы where дистанция = @p0  order by порядок";
                List<temp> tempList = de.Database.SqlQuery<temp>(sqlString, кодНов).ToList();

            //    Guid кодНов = dRow.дистанция;
                //востановление 
                //клДистанция.deRow = oldДистанции;
                //клДистанция.дистанция = клДистанция.deRow.дистанция;
                //клДистанция.наимен = клДистанция.deRow.наимен;
                //клДистанция.вид = клДистанция.deRow.вид;
                этапыList.Clear();
//                de.этапы.Local.Clear();
                //de.SaveChanges();
                //viewSource1.View.Refresh();
        //        var query = de.этапы.Where(n => n.дистанция == кодНов).OrderBy(n => n.порядок).ToList();
        // тоже становятся локальными

                foreach (temp eRow in tempList)
                {
                    этапы newRow = new этапы()
                    {
                        дистанция = клДистанция.дистанция,
                        наимен = eRow.наимен,
                        порядок = eRow.порядок,
                        судья = eRow.судья,
                        этап = Guid.NewGuid()
                    };
                    этапыList.Add(newRow);
//                    de.этапы.Local.Add(newRow);

//                    bindingSource1.Add(newRow);
                    //   Console.WriteLine(newRow.наимен);
                }
                //de.SaveChanges();
                //de.этапы.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.порядок).Load();
                //viewSource1.Source = de.этапы.Local.ToBindingList();
                //dataGrid1.ItemsSource = viewSource1.View;

                label1.Visible = true;
                //   viewSource1.View.Refresh();
                //  viewSource1.Source = null;
                //viewSource1.Source = de.этапы.Local.ToBindingList();
                //dataGrid1.ItemsSource = viewSource1.View;
              //  Close();
            }
        }

        class temp
        {
            public string наимен { get; set; }
            public string судья { get; set; }
            public int порядок { get; set; }

        }
    }
}
