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
using Word = Microsoft.Office.Interop.Word;

namespace compWpf
{
    public partial class по_одному : Form
    {
        public по_одному()
        {
            InitializeComponent();
        }
        Entities de = new Entities();
        BindingList<результаты> binList;
        DataTable dt;

        private void по_одному_Load(object sender, EventArgs e)
        {
            this.Text = "Результаты на " + клДистанция.deRow.наимен + " " + клСлет.наимен;
            label2.Text = this.Text;
            try
            {

                de.экипажи.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.номер).Load();

                de.результаты.Where(n => n.экипажи.дистанция == клДистанция.дистанция)
                    .OrderBy(n => n.порядок).ThenBy(n => n.экипажи.номер).ThenBy(n => n.попытка).Load();

                de.этапы.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.порядок).Load();
                de.штрафы.Where(n => n.результаты.экипажи.дистанция == клДистанция.дистанция).Load();

                //System.Collections.ObjectModel.ObservableCollection<штрафы>  ее = de.штрафы.Local;

                de.суда.OrderBy(n => n.порядок).Load();

                создать_таблицу();

                dataGridView2.AutoGenerateColumns = true;
                bindingSource2.DataSource = dt;
                initGrid2();

                binList = de.результаты.Local.ToBindingList();


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
                            прим = "",
                            плывут = false
                        };


                        binList.Add(newRow);

                    }

                }

                пересчет();

                bindingSource1.DataSource = binList;
                bindingSource1.Sort = " номер, попытка";
                обновитьШтрафы();
                клСетка.задать_ширину(dataGridView1);
                int столбцов = dataGridView2.Columns.Count;
                if (столбцов > 10)
                {
                    клСетка.задать_ширину(dataGridView2);
                }
                numericUpDown1.DataBindings.Add("Value", bindingSource1, "время_мин");
                numericUpDown2.DataBindings.Add("Value", bindingSource1, "время_сек");
                numericUpDown3.DataBindings.Add("Value", bindingSource1, "штраф");
              
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки " + ex.Message);
            }

            timer2.Start();
            timer2.Tick += Timer2_Tick;
            FormClosing += Список_видов_FormClosing;
            bindingSource1.ListChanged += BindingSource1_ListChanged;
            bindingSource1.PositionChanged += BindingSource1_PositionChanged;
            dataGridView2.CellValueChanged += DataGridView2_CellValueChanged;
            dataGridView1.DataError += DataGridView1_DataError;
            dataGridView1.CellPainting += DataGridView1_CellPainting;
            dataGridView1.EditingControlShowing += dataGridView1_EditingControlShowing;
            dataGridView1.CellContentClick += DataGridView1_CellContentClick;
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.CellValidating += DataGridView1_CellValidating;
            dataGridView2.EditingControlShowing += DataGridView2_EditingControlShowing;
            numericUpDown1.ValueChanged += NumericUpDown1_ValueChanged; 
            numericUpDown2.ValueChanged += NumericUpDown1_ValueChanged;
            numericUpDown3.ValueChanged += NumericUpDown1_ValueChanged; 
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            label1.Visible = true;
            if (bindingSource1.Count > 0)
            {
                пересчет();
                dataGridView1.Refresh();
            }
        }

        private void DataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress);
            string CellName = dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name;

      
            e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress);
        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.FormattedValue.ToString() == "")
            {
                DataGridViewTextBoxColumn[] aCol = new DataGridViewTextBoxColumn[3] { секColumn, минColumn, штрафColumn };


                if (aCol.Contains(dataGridView1.Columns[e.ColumnIndex]))
                {
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
                }
                DataGridViewTextBoxColumn[] aCol2 = new DataGridViewTextBoxColumn[1] { примColumn };


                if (aCol2.Contains(dataGridView1.Columns[e.ColumnIndex]))
                {
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                }
            }


            if (dataGridView1.Columns[e.ColumnIndex] == секColumn)
            {
                int секунд = 0;


                if (int.TryParse(e.FormattedValue.ToString(), out секунд))
                {
                    if (секунд > 59)
                    {
                        e.Cancel = true;
                        MessageBox.Show("Введите секунды от 0 до 59");
                    }
                }

            }
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (dataGridView1.Columns[e.ColumnIndex] == примColumn)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                }
            }

        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                if (dataGridView1.Columns[e.ColumnIndex] == стартColumn)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    результаты rRow = bindingSource1.Current as результаты;

                    if (rRow.плывут)
                    {
                        WMPLib.WindowsMediaPlayer wmp = new WMPLib.WindowsMediaPlayer();
                        wmp.URL = "ФинишЖ.wav";

                        wmp.controls.play();
                        rRow.плывут = false;
                        пересчет();
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LawnGreen;
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Blue;
                    }
                    else
                    {
                        if (rRow.забег)
                        {
                            WMPLib.WindowsMediaPlayer wmp = new WMPLib.WindowsMediaPlayer();
                            wmp.URL = "СтартЖ.wav";

                            wmp.controls.play();

                            rRow.старт = DateTime.Now;
                            rRow.финиш = rRow.старт;
                            rRow.плывут = true;
                            rRow.секунд = 0;
                            rRow.время_мин = 0;
                            rRow.время_сек = 0;
                            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightCyan;
                            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Red;
                            dataGridView1.Refresh();
                            rRow.забег = false;
                        }
                        else
                        {
                            MessageBox.Show("Не готов");
                        }
                    }

                }
                if (dataGridView1.Columns[e.ColumnIndex] == забегColumn)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    результаты rRow = bindingSource1.Current as результаты;

                    if (!rRow.плывут)
                    {
                        if (!rRow.забег)
                        {

                            if (rRow.итог > 0)
                            {
                                if (MessageBox.Show("Очистить результаты ?", "Внимание", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    WMPLib.WindowsMediaPlayer wmp = new WMPLib.WindowsMediaPlayer();
                                    wmp.URL = "ВниманиеЖ.wav";

                                    wmp.controls.play();

                                    rRow.время_мин = 0;
                                    rRow.время_сек = 0;
                                    rRow.секунд = 0;
                                    rRow.штраф = 0;
                                    rRow.забег = true;
                                    // dt.Clear();
                                    штрафы[] aRow = rRow.штрафы.ToArray();

                                    foreach (штрафы delRow in aRow)
                                    {
                                        de.штрафы.Local.Remove(delRow);
                                    }
                                    обновитьШтрафы();
                                    пересчет();
                                }
                                else
                                {
                                    rRow.забег = false;
                                }
                                dataGridView1.Refresh();
                                dataGridView2.Refresh();
                            }
                            else
                            {
                                WMPLib.WindowsMediaPlayer wmp = new WMPLib.WindowsMediaPlayer();
                                wmp.URL = "ВниманиеЖ.wav";
                                wmp.controls.play();
                                rRow.забег = true;
                            }
                        }
                        else
                        {
                            rRow.забег = false;
                        }
                    }
                }
                //}
            }
        }

        void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress);
            string CellName = dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name;

            if (CellName == "минColumn" || CellName == "секColumn" || CellName == "штрафColumn")
            {
                e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress);
            }
        }

        void Control_KeyPress(object sender, KeyPressEventArgs pressE)
        {
            клKey.int_KeyPress(sender, pressE);
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            int j = 0;
            foreach (результаты rRow in de.результаты.Local.Where(n => n.плывут))
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
                dataGridView1.Refresh();
                //numericUpDown1.Refresh();
                //numericUpDown2.Refresh();  // не помогает
                обновить_num();

            }


        }
        void обновить_num()
        {
            if(bindingSource1.Count>0)
            {
                результаты rRow = bindingSource1.Current as результаты;
                numericUpDown1.Value = rRow.время_мин;
                numericUpDown2.Value = rRow.время_сек;
                label7.Text = rRow.миллисекунд.ToString();

            }
        }


        private void DataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex > -1 && e.RowIndex > -1)
            {
                if (dataGridView1.Columns[e.ColumnIndex] == стартColumn && e.Button == MouseButtons.Left)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    результаты rRow = bindingSource1.Current as результаты;
                    if (rRow.плывут)
                    {
                        rRow.плывут = false;
                        пересчет();
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LawnGreen;
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Blue;
                    }
                    else
                    {
                        rRow.старт = DateTime.Now;
                        rRow.финиш = rRow.старт;
                        rRow.плывут = true;
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightCyan;
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Red;
                        dataGridView1.Refresh();
                    }
                }
            }
        }

        private void DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex < dataGridView1.Columns.Count)
            {
                e.PaintBackground(e.CellBounds, true);
                e.Graphics.TranslateTransform(e.CellBounds.Left, e.CellBounds.Bottom);
                e.Graphics.RotateTransform(270);
                e.Graphics.DrawString(e.FormattedValue?.ToString(), label3.Font, Brushes.Black, 5, 5);
                e.Graphics.ResetTransform();
                e.Handled = true;
            }
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Ошибка ввода  ");
        }


        private void DataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                результаты rRow = bindingSource1.Current as результаты;
                if (rRow != null)
                {
                    Guid кодЭтапа = (Guid)dt.Columns[e.ColumnIndex].ExtendedProperties["этап"];

                    if (dataGridView2.Rows[0].Cells[e.ColumnIndex].Value == System.DBNull.Value)
                    {
                        dataGridView2.Rows[0].Cells[e.ColumnIndex].Value = 0;
                    }
                    int ss = 0;
                    try
                    {
                        ss = (int)dataGridView2.Rows[0].Cells[e.ColumnIndex].Value;
                    }
                    catch
                    {
                        MessageBox.Show("jjj");
                    }
                    var DicШтрафы = de.штрафы.Local.ToDictionary(n => (n.результат, n.этап));
                    var ключ = (rRow.результат, кодЭтапа);

                    if (DicШтрафы.ContainsKey(ключ))
                    {
                        штрафы sRow = DicШтрафы[ключ];
                        if (ss > 0)
                        {
                            if (sRow.секунд == ss)
                            {

                            }
                            else
                            {
                                sRow.секунд = ss;
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
                        if (ss > 0)
                        {
                            штрафы newRow = new штрафы
                            {
                                прим = "",
                                результат = rRow.результат,
                                секунд = ss,
                                штраф = Guid.NewGuid(),
                                этап = кодЭтапа
                            };
                            de.штрафы.Local.Add(newRow);
                            label1.Visible = true;
                        }
                    }

                }

            }
        }

        private void BindingSource1_PositionChanged(object sender, EventArgs e)
        {
            обновитьШтрафы();

        }



        void обновитьШтрафы()
        {
            if (bindingSource1.Count > 0)
            {
                результаты rRow = bindingSource1.Current as результаты;
                if (rRow != null)
                {

                    dataGridView2.EndEdit();
                    if(rRow.забег)
                    {
                        checkBox1.Enabled = true;
                    }
                    else
                    {
                        checkBox1.Enabled = false;
                    }

                    dt.Clear();

                    DataRow dr = dt.NewRow();
                    var DicШтрафы = de.штрафы.Local.ToDictionary(n => (n.результат, n.этап));
   //                 var DicШтрафы = de.штрафы.ToDictionary(n => (n.результат, n.этап));
                    int j = 0;
                    foreach (этапы eRow in de.этапы.Local)
                    {
                        var ключ = (rRow.результат, eRow.этап);
                        if (DicШтрафы.ContainsKey(ключ))
                        {
                            int ss = DicШтрафы[ключ].секунд;
                            dr.SetField<int>(j, ss);
                        }
                        j++;
                    }
                    dt.Rows.Add(dr);
                    bindingSource2.DataSource = dt;
                    dataGridView2.Refresh();
                }
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
            if (bindingSource1.Count > 0)
            {
                пересчет();
            }
        }

        private void пересчет()
        {
            foreach (результаты rRow in de.результаты.Local)
            {
                rRow.секунд = rRow.время_мин * 60 + rRow.время_сек;
                rRow.итог = rRow.секунд + rRow.штраф;
            }
            foreach (суда dRow in de.суда.Local)
            {

                var query = de.результаты.Local
                    .Where(n => n.экипажи.судно == dRow.судно)
                    .Where(n => n.итог > 0)
                    .GroupBy(n => n.экипаж)
                    .Select(n => new { экипаж = n.Key, лучший = n.Min(p => p.итог) }).ToDictionary(n => n.экипаж);

                foreach (результаты rRow in de.результаты.Local
                    .Where(n => n.экипажи.судно == dRow.судно))
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

                foreach (экипажи uRow in de.экипажи.Local.Where(n => n.судно == dRow.судно))
                {

                    uRow.место = 0;


                }
                int j = 0;
                foreach (экипажи uRow in de.экипажи.Local.Where(n => n.судно == dRow.судно).Where(n => n.итог > 0).OrderBy(n => n.итог))
                {
                    j++;
                    uRow.место = j;


                }
            }
            dataGridView1.Refresh();
            label1.Visible = true;
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

        int GetMaxNum()
        {
            int maxNum = 0;
            if (de.экипажи.Local.Any())
            {
                maxNum = de.экипажи.Local.Max(n => n.номер);

            }
          
            return maxNum;

        }

        int GetMaxPor()
        {
            int maxPor = 0;
            if (de.результаты.Local.Any())
            {
                maxPor = de.результаты.Local.Max(n => n.порядок);
            }
            return maxPor;
        }

        void создать_таблицу()
        {
            dt = new DataTable();
            //DataColumn dc = new DataColumn
            //{
            //    Caption = "код",
            //    DataType = typeof(string),
            //    ColumnName = "col0",
            //    DefaultValue = "",
            //    MaxLength = 36,
            //    ReadOnly = true,
            //    Unique = true
            //};
            //dt.Columns.Add(dc);
            int j = 0;
            foreach (этапы eRow in de.этапы.Local)
            {

                j++;

                DataColumn dc1 = new DataColumn
                {
                    Caption = eRow.наимен.Trim(),
                    DataType = typeof(int),
                    ColumnName = "col" + j.ToString().Trim(),
                    DefaultValue = 0,
                    ReadOnly = false
                };
                dc1.ExtendedProperties.Add("этап", eRow.этап);
                dc1.ExtendedProperties.Add("заголовок", eRow.наимен);
                dt.Columns.Add(dc1);
            }
            DataRow dr = dt.NewRow();

            dt.Rows.Add(dr);
        }
        void initGrid2()
        {

            int j = 0;
            foreach (DataColumn eRow in dt.Columns)
            {
                dataGridView2.Columns[j].HeaderText = eRow.ExtendedProperties["заголовок"].ToString();
                dataGridView2.Columns[j].Tag = (Guid)eRow.ExtendedProperties["этап"];
                j++;
            }
           
        }
        void пересчетШтрафов()
        {
            if (bindingSource1.Count > 0)
            {
                результаты rRow = bindingSource1.Current as результаты;
                int сумма = 0;
                if (de.штрафы.Local.Any(n => n.результат == rRow.результат))
                {
                    сумма = de.штрафы.Local.Where(n => n.результат == rRow.результат).Sum(n => n.секунд);
                }
                rRow.штраф = сумма;
                пересчет();
            }

            dataGridView1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                результаты rRow = bindingSource1.Current as результаты;
                if (checkBox1.Checked)
                {
                   


                    if (rRow.забег)
                    {
                        WMPLib.WindowsMediaPlayer wmp = new WMPLib.WindowsMediaPlayer();
                        wmp.URL = "СтартЖ.wav";

                        wmp.controls.play();

                        rRow.старт = DateTime.Now;
                        rRow.финиш = rRow.старт;
                        rRow.плывут = true;
                        rRow.секунд = 0;
                        rRow.время_мин = 0;
                        rRow.время_сек = 0;
                        //dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightCyan;
                        //dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Red;
                        dataGridView1.Refresh();
                        rRow.забег = false;
                        checkBox1.Text = "Финиш";
                        

                    }
                    else
                    {
                        checkBox1.Checked = false;
                        MessageBox.Show("Не готов");
                    }



                }
                else
                {
                    checkBox1.Text = "Старт";
                    WMPLib.WindowsMediaPlayer wmp = new WMPLib.WindowsMediaPlayer();
                    wmp.URL = "ФинишЖ.wav";

                    wmp.controls.play();
                    rRow.плывут = false;
                    пересчет();
                }
            }
            dataGridView1.Refresh();
            panel2.Refresh();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if(bindingSource1.Count>0)
            {
                результаты rRow = bindingSource1.Current as результаты;
                клРезультат.результат = rRow.результат;
                клРезультат.deRow = rRow;
                подробности1попытка однаПопытка = new подробности1попытка();
                однаПопытка.штрафыСол = de.штрафы.Local;
                однаПопытка.ShowDialog();
                обновитьШтрафы();
                label1.Visible = true;
                dataGridView1.Focus();

            }
        }
    }
}
