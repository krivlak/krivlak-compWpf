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
    public partial class подробности1попытка : Form
    {
        public подробности1попытка()
        {
            InitializeComponent();
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;
        }
        Entities de = new Entities();
        List<temp> tempList = new List<temp>();
        DateTime старт = DateTime.Now;
        bool плывут = false;
        public  System.Collections.ObjectModel.ObservableCollection<штрафы> штрафыСол ;
        private void подробности1попытка_Load(object sender, EventArgs e)
        {
            de.этапы.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.порядок).Load();
        //    de.штрафы.Where(n => n.результат == клРезультат.результат).Load();

            tempList = de.этапы.Local
                 .Select(n => new temp
                 {
                     наимен = n.наимен,
                     штраф = 0,
                     этап = n.этап
                 }).ToList();

            //     foreach(штрафы sRow in de.штрафы.Local)
            foreach (штрафы sRow in штрафыСол)
            {
                tempList.Find(n => n.этап == sRow.этап).штраф = sRow.секунд;
            }
            bindingSource1.DataSource = tempList;
            timer1.Start();
            dataGridView1.EditingControlShowing += dataGridView1_EditingControlShowing;
            dataGridView1.CellValidating += DataGridView1_CellValidating;
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            timer1.Tick += Timer1_Tick;
       //     FormClosing += Список_видов_FormClosing;

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (плывут)
            {
                DateTime финиш = DateTime.Now;
                //  DateTime старт = DateTime.Now;

                TimeSpan rtr = (финиш - старт);
                int время_мин = rtr.Minutes;
                int время_сек = rtr.Seconds;
                int миллисекунд = rtr.Milliseconds;
                label2.Text = миллисекунд.ToString();
                numericUpDown1.Value = время_мин;
                numericUpDown2.Value = время_сек;
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

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                temp rRow = bindingSource1.Current as temp;
                
               // var DicШтрафы = de.штрафы.Local.ToDictionary(n => n.этап);
                var DicШтрафы = штрафыСол.ToDictionary(n => n.этап);

                if (DicШтрафы.ContainsKey(rRow.этап))
                {
                    штрафы sRow = DicШтрафы[rRow.этап];
                    if (rRow.штраф > 0)
                    {
                        if (sRow.секунд == rRow.штраф)
                        {

                        }
                        else
                        {
                            sRow.секунд = rRow.штраф;
                            label1.Visible = true;
                        }
                    }
                    else
                    {
                        de.штрафы.Local.Remove(sRow);
                        label1.Visible = true;
                    }
                }
                else
                {
                    if (rRow.штраф > 0)
                    {
                        штрафы newRow = new штрафы
                        {
                            прим = "",
                            результат = клРезультат.результат,
                            секунд = rRow.штраф,
                            штраф = Guid.NewGuid(),
                            этап = rRow.этап
                        };
                        de.штрафы.Local.Add(newRow);
                        label1.Visible = true;
                    }
                }
            }
        }


        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if(dataGridView1.Columns[e.ColumnIndex]==штрафColumn)
            {
                if(e.FormattedValue.ToString()=="")
                {
                    dataGridView1.Rows[e.RowIndex].Cells["штрафColumn"].Value = 0;
                }

            }
        }

        class temp
        {
            public Guid этап { get; set; }
            public string наимен { get; set; }
            public int штраф { get; set; } = 0;

        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress);
            string CellName = dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name;


            e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress);
        }
        void Control_KeyPress(object sender, KeyPressEventArgs pressE)
        {
            клKey.int_KeyPress(sender, pressE);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                плывут = true;
                старт = DateTime.Now;
                checkBox1.Text = "Финиш";
            }
            else
            {
                плывут = false;
            
                checkBox1.Text = "Старт";
            }
        }
    }
}
