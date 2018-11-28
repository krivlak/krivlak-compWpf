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
    public partial class окно2результатов : Form
    {
        public окно2результатов()
        {
            InitializeComponent();
          
        }

    //    Form окно_выбора;
        Entities de = new Entities();
        BindingList<результаты> binList;
        //дистанции deДистанция;
        //Guid кодДистанции;
        DataTable dt;
      //  Dictionary<(Guid, Guid), штрафы> DicШтрафы;
        private void окно1результатов_Load(object sender, EventArgs e)
        {
            //deДистанция = клДистанция.deRow;
            //кодДистанции = deДистанция.дистанция;
            this.Text ="Результаты на "+ клДистанция.deRow.наимен+" "+клСлет.наимен;
            label2.Text = this.Text;
            try
            {

                de.экипажи.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.номер).Load();

                de.результаты.Where(n => n.экипажи.дистанция == клДистанция.дистанция)
                    .OrderBy(n => n.порядок).ThenBy(n => n.экипажи.номер).ThenBy(n => n.попытка).Load();

                de.этапы.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.порядок).Load();
                de.штрафы.Where(n => n.результаты.экипажи.дистанция == клДистанция.дистанция).Load();
             //   DicШтрафы = de.штрафы.Local.ToDictionary(n => ( n.результат, n.этап));

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
                задать_ширину();
                задать2ширину();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки " + ex.Message);
            }

            timer2.Start();
            timer2.Tick += Timer2_Tick;
            FormClosing += Список_видов_FormClosing;
            bindingSource1.ListChanged += BindingSource1_ListChanged;
            //this.Text = deДистанция.наимен;
            //label2.Text = this.Text;
            bindingSource1.PositionChanged += BindingSource1_PositionChanged;
            dataGridView2.CellValueChanged += DataGridView2_CellValueChanged;
            //            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.DataError += DataGridView1_DataError;
            dataGridView1.CellPainting += DataGridView1_CellPainting;
           // dataGridView1.CellMouseClick += DataGridView1_CellMouseClick;
            dataGridView1.EditingControlShowing += dataGridView1_EditingControlShowing;
            dataGridView1.CellContentClick += DataGridView1_CellContentClick;
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
        //    dataGridView1.CellMouseClick += DataGridView1_CellMouseClick1;
         //   dataGridView1.CellValidating += DataGridView1_CellValidating;
            
        }

        //private void DataGridView1_CellMouseClick1(object sender, DataGridViewCellMouseEventArgs e)
        //{
          
        //}

        //private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        //{
        //    if (dataGridView1.Columns[e.ColumnIndex] == забегColumn)
        //    {
                
        //        bool xy = (bool)e.FormattedValue;
        //        if (xy)
        //        {
        //            результаты rRow = bindingSource1.Current as результаты;
        //            if(rRow.итог>0 )
        //            {
        //                if(MessageBox.Show("Очистить результаты ?", "Внимание", MessageBoxButtons.YesNo)== DialogResult.Yes)
        //                {
        //                    WMPLib.WindowsMediaPlayer wmp = new WMPLib.WindowsMediaPlayer();
        //                    wmp.URL = "ВниманиеЖ.wav";
        //                    //wmp.URL = "старт.mp3";
        //                    //wmp.URL = "старт.mp3";
        //                    wmp.controls.play();
        //                    rRow.время_мин = 0;
        //                    rRow.время_сек = 0;
        //                    rRow.штраф = 0;
        //                }
        //                else
        //                {
        //                    e.Cancel = true;
        //                }

        //            }
        //        }
        //    }
        //}

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            
           if(dataGridView1.Columns[e.ColumnIndex]== примColumn)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value==null)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                }
            }

            //if (dataGridView1.Columns[e.ColumnIndex] == забегColumn)
            //{

            //    bool xy = (bool)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            //    if (xy)
            //    {
            //        результаты rRow = bindingSource1.Current as результаты;
            //        if (rRow.итог > 0)
            //        {
            //            if (MessageBox.Show("Очистить результаты ?", "Внимание", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //            {
            //                WMPLib.WindowsMediaPlayer wmp = new WMPLib.WindowsMediaPlayer();
            //                wmp.URL = "ВниманиеЖ.wav";
            //                //wmp.URL = "старт.mp3";
            //                //wmp.URL = "старт.mp3";
            //                wmp.controls.play();
            //                rRow.время_мин = 0;
            //                rRow.время_сек = 0;
            //                rRow.штраф = 0;
            //            }
            //            else
            //            {
            //                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
            //               // e.Cancel = true;
            //            }

            //        }
            //    }
            //}

        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                //if (e.ColumnIndex > -1 && e.RowIndex > -1)
                //{
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
                    
                    // Console.WriteLine(rRow.время_сек.ToString());
                }
                if (dataGridView1.Columns[e.ColumnIndex] == забегColumn  )
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

                                    foreach (штрафы  delRow in aRow)
                                    {
                                        de.штрафы.Local.Remove(delRow);
                                        //var ключ2 = (rRow.результат, delRow.штраф);
                                        //DicШтрафы.Remove(ключ2);
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
            //if (CellName == "раб_днейColumn")
            //{
            //    e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress);
            //}
        }

        void Control_KeyPress(object sender, KeyPressEventArgs pressE)
        {
            //            клKey.decimal_KeyPress(sender, pressE);
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
                        //rRow.время_мин = 0;
                        //rRow.время_сек = 0;
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightCyan;
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Red;
                        dataGridView1.Refresh();
                    }
                    // Console.WriteLine(rRow.время_сек.ToString());
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
             //   e.Graphics.DrawString(e.FormattedValue?.ToString(), e.CellStyle.Font, Brushes.Black, 5, 5);
                e.Graphics.DrawString(e.FormattedValue?.ToString(), label3.Font, Brushes.Black, 5, 5);
                e.Graphics.ResetTransform();
                e.Handled = true;
                //dataGridView1.ColumnHeadersHeight = 100;
            }
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == System.DBNull.Value)
            //{
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
            //}
            //throw new NotImplementedException();
        }

        //private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == System.DBNull.Value)
        //    {
        //        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
        //    }
        //}

        private void DataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                результаты rRow = bindingSource1.Current as результаты;
                if (rRow != null)
                {
                    // как определить код этапа?
               //     Guid кодЭтапа = (Guid)dataGridView2.Columns[e.ColumnIndex].Tag;
                    Guid кодЭтапа =(Guid) dt.Columns[e.ColumnIndex].ExtendedProperties["этап"];
                    //MessageBox.Show(кодЭтапа.ToString() + "==" + кодЭтапа2.ToString());
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
                          //  DicШтрафы.Remove(ключ);
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
                          //  var новКлюч = (newRow.результат, newRow.штраф);
                        //    DicШтрафы.Add(новКлюч,newRow);
                            de.штрафы.Local.Add(newRow);
                            label1.Visible = true;
                        }
                    }

                    //if (de.штрафы.Local.Where(n => n.результат == rRow.результат).Count(n => n.этап == кодЭтапа) == 1)
                    //{
                    //    штрафы sRow = de.штрафы.Local.Where(n => n.результат == rRow.результат).Single(n => n.этап == кодЭтапа);

                    //    if (ss > 0)
                    //    {
                    //        if (sRow.секунд == ss)
                    //        {

                    //        }
                    //        else
                    //        {
                    //            sRow.секунд = ss;
                    //            label1.Visible = true;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        de.штрафы.Local.Remove(sRow);
                    //        label1.Visible = true;
                    //    }

                    //}
                    //else
                    //{
                    //    if (ss > 0)
                    //    {
                    //        штрафы newRow = new штрафы
                    //        {
                    //            прим = "",
                    //            результат = rRow.результат,
                    //            секунд = ss,
                    //            штраф = Guid.NewGuid(),
                    //            этап = кодЭтапа
                    //        };
                    //        de.штрафы.Local.Add(newRow);
                    //        label1.Visible = true;
                    //    }
                 //   }
                }
             
            }
        }

        private void BindingSource1_PositionChanged(object sender, EventArgs e)
        {
            обновитьШтрафы();
            //if (bindingSource1.Count > 0)
            //{
            //    результаты rRow = bindingSource1.Current as результаты;
            //    if (rRow != null)
            //    {

            //        dt.Clear();

            //        DataRow dr = dt.NewRow();
            //        int j = 0;
            //        foreach (этапы eRow in de.этапы.Local)
            //        {
            //            if (de.штрафы.Local.Where(n=>n.этап==eRow.этап).Count(n => n.результат == rRow.результат) == 1)
            //            {
            //                int ss = de.штрафы.Local.Where(n => n.этап == eRow.этап).Single(n => n.результат == rRow.результат).секунд;
            //                dr.SetField<int>(j, ss);
            //            }
            //            j++;
            //        }
            //        dt.Rows.Add(dr);
            //        bindingSource2.DataSource = dt;
            //        dataGridView2.Refresh();
            //    }
            //}
        }



        void обновитьШтрафы()
        {
            if (bindingSource1.Count > 0)
            {
                результаты rRow = bindingSource1.Current as результаты;
                if (rRow != null)
                {
                    dataGridView2.EndEdit();

                    dt.Clear();

                    DataRow dr = dt.NewRow();
                  var  DicШтрафы = de.штрафы.Local.ToDictionary(n => (n.результат, n.этап));
                    int j = 0;
                    foreach (этапы eRow in de.этапы.Local)
                    {
                        var ключ = (rRow.результат, eRow.этап);
                        if (DicШтрафы.ContainsKey(ключ))
                        {
                            int ss = DicШтрафы[ключ].секунд;
                            dr.SetField<int>(j, ss);
                        }
                        //if (de.штрафы.Local.Where(n => n.этап == eRow.этап).Count(n => n.результат == rRow.результат) == 1)
                        //{
                        //    int ss = de.штрафы.Local.Where(n => n.этап == eRow.этап).Single(n => n.результат == rRow.результат).секунд;
                        //    dr.SetField<int>(j, ss);
                        //}
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
                    //var ar = de.результаты.Local.Where(n => n.итог < 1).ToArray();
                    //foreach(результаты delRow in ar)
                    //{
                    //    de.результаты.Local.Remove(delRow);
                    //}
                    de.SaveChanges();
                    //    клДистанция.открытыЛист.Remove(кодДистанции);
                  //  клДистанция.formList.Remove(this);
                    //string[] arr = клДистанция.открытыЛист.Where(n => n == клДистанция.дистанция).ToArray();
                    //foreach(string sRow in arr)
                    //{
                    //    клДистанция.открытыЛист.Remove(sRow);
                    //}
                    //Console.WriteLine("закрыт " + клДистанция.deRow.наимен);



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
                //результаты rRow = bindingSource1.Current as результаты;
                //rRow.секунд = rRow.время_мин * 60 + rRow.время_сек;
                //rRow.итог = rRow.секунд + rRow.штраф;
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

                //            foreach (результаты rRow in de.результаты.Local.Where(n => n.итог > 0).OrderBy(n => n.номер).ThenByDescending(n => n.итог))
                foreach (результаты rRow in de.результаты.Local
                    .Where(n => n.экипажи.судно == dRow.судно))
                {
                    //foreach (результаты uRow in de.результаты.Local.Where(n => n.экипаж == rRow.экипаж))
                    //{
                    rRow.зачетный = false;
                    //}

                    //   rRow.лучший = 0;
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
                    //  rRow.экипажи.итог = rRow.итог;
                    // rRow.зачетный = true;
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

        //private void пересчет()
        //{
        //    foreach (результаты rRow in de.результаты.Local)
        //    {
        //        rRow.секунд = rRow.время_мин * 60 + rRow.время_сек;
        //        rRow.итог = rRow.секунд + rRow.штраф;
        //    }

        //    var query = de.результаты.Local
        //        .Where(n => n.итог > 0)
        //        .GroupBy(n => n.экипаж)
        //        .Select(n => new { экипаж = n.Key, лучший = n.Min(p => p.итог) }).ToDictionary(n => n.экипаж);

        //    //            foreach (результаты rRow in de.результаты.Local.Where(n => n.итог > 0).OrderBy(n => n.номер).ThenByDescending(n => n.итог))
        //    foreach (результаты rRow in de.результаты.Local)
        //    {
        //        //foreach (результаты uRow in de.результаты.Local.Where(n => n.экипаж == rRow.экипаж))
        //        //{
        //        rRow.зачетный = false;
        //        //}

        //        //   rRow.лучший = 0;
        //        rRow.экипажи.итог = 0;
        //        rRow.экипажи.место = 0;
        //        if (query.ContainsKey(rRow.экипаж))
        //        {
        //            rRow.экипажи.итог = query[rRow.экипаж].лучший;
        //            if (rRow.итог == query[rRow.экипаж].лучший)
        //            {
        //                rRow.зачетный = true;
        //            }
        //        }
        //        //  rRow.экипажи.итог = rRow.итог;
        //        // rRow.зачетный = true;
        //    }

        //    foreach (экипажи uRow in de.экипажи.Local)
        //    {
        //        uRow.место = 0;
        //    }

        //    int j = 0;
        //    foreach (экипажи uRow in de.экипажи.Local.Where(n => n.итог > 0).OrderBy(n => n.итог))
        //    {
        //        j++;
        //        uRow.место = j;
        //    }
        //    dataGridView1.Refresh();
        //    label1.Visible = true;
        //}


        public void init_слет()
        {
            if (de.слеты.Any())
            {
                клСлет.deRow = de.слеты.OrderBy(n => n.порядок).First();
                клСлет.слет = клСлет.deRow.слет;
                клСлет.наимен = клСлет.deRow.наимен;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    Word.Application oWord = new Word.Application();

        //    string curDir = System.IO.Directory.GetCurrentDirectory();

        //    object шаблон = curDir + @"\протокол.dot";
        //    if (!System.IO.File.Exists(шаблон.ToString()))
        //    {
        //        MessageBox.Show("Нет файла " + шаблон.ToString());
        //        return;
        //    }

        //    try
        //    {

        //        Word.Document o = oWord.Documents.Add(Template: шаблон);


        //        Word.Table tab1 = o.Tables[1];
        //        Word.Table tab2 = o.Tables[2];
        //        oWord.Visible = true;


        //        tab1.Rows[1].Cells[1].Range.Text = this.Text;
        //        int j = 1;
        //        foreach (результаты pRow in de.результаты.Local.Where(n => n.лучший > 0).Where(n => n.зачетный).OrderBy(n => n.место))
        //        {
        //            j++;
        //            tab2.Rows[j].Cells[1].Range.Text = pRow.номер.ToString();
        //            tab2.Rows[j].Cells[2].Range.Text = pRow.клуб;
        //            tab2.Rows[j].Cells[3].Range.Text = pRow.состав;
        //            tab2.Rows[j].Cells[4].Range.Text = pRow.время_мин.ToString();
        //            tab2.Rows[j].Cells[5].Range.Text = pRow.время_сек.ToString();
        //            tab2.Rows[j].Cells[6].Range.Text = pRow.секунд.ToString();
        //            tab2.Rows[j].Cells[7].Range.Text = pRow.штраф.ToString();
        //            tab2.Rows[j].Cells[8].Range.Text = pRow.итог.ToString();
        //            tab2.Rows[j].Cells[9].Range.Text = pRow.место.ToString();



        //            tab2.Rows.Add();

        //        }


        //        oWord.Visible = true;
        //    }
        //    catch (Exception ex)
        //    {

        //        oWord.DisplayAlerts = Word.WdAlertLevel.wdAlertsNone;
        //        oWord.Application.Quit(SaveChanges: false);
        //        MessageBox.Show("Сбой Word " + ex.Message);
        //    }

        //}

        private void button4_Click(object sender, EventArgs e)
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


                tab1.Rows[1].Cells[1].Range.Text = "Протоколы результатов на дистанции " + клДистанция.наимен + " на " + клСлет.наимен;
                int j = 1;
                foreach (суда dRow in de.суда.Local)
                {
                    if (de.результаты.Local.Where(n => n.экипажи.судно == dRow.судно).Where(n => n.лучший > 0).Count(n => n.зачетный) > 0)
                    {
                        j++;
                        tab2.Rows.Add();
                        j++;
                        tab2.Rows[j].Cells[2].Range.Text = dRow.наимен;
                        tab2.Rows.Add();
                    }
                    foreach (результаты pRow in de.результаты.Local
                        .Where(n => n.экипажи.судно == dRow.судно)
                        .Where(n => n.лучший > 0).Where(n => n.зачетный).OrderBy(n => n.место))
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


        private void button1_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                результаты tRow = bindingSource1.Current as результаты;
                Guid кодЭкипажа = tRow.экипаж;
                var ar = de.результаты.Local.Where(n => n.экипаж == tRow.экипаж).ToArray();
                foreach (результаты delRow in ar)
                {
                    de.результаты.Local.Remove(delRow);
                }
                экипажи dRow = de.экипажи.Local.Single(n => n.экипаж == кодЭкипажа);
                de.экипажи.Local.Remove(dRow);
                label1.Visible = true;

            }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                результаты oldRow = bindingSource1.Current as результаты;

              //  int oldPor = oldRow.порядок;
                if (bindingSource1.Position < bindingSource1.Count - 1)
                {
                    bindingSource1.MoveNext();
                    результаты lastRow = bindingSource1.Current as результаты;
                    //int lastPor = lastRow.порядок;
                    //oldRow.порядок = lastPor;
                    //lastRow.порядок = oldPor;
                    (oldRow.порядок, lastRow.порядок) = (lastRow.порядок, oldRow.порядок);
                    bindingSource1.Sort = "порядок";

                    //   результаты_деталейЛист.Sort((a, b) => a.порядок.CompareTo(b.порядок));
                    dataGridView1.Refresh();
                    label1.Visible = true;

                }
            }
            dataGridView1.Focus();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                результаты oldRow = bindingSource1.Current as результаты;
              //  int oldIndex = bindingSource1.Position;

                int oldPor = oldRow.порядок;
                if (bindingSource1.Position > 0)
                {
                    bindingSource1.MovePrevious();

                    результаты lastRow = bindingSource1.Current as результаты;
                    //int lastPor = lastRow.порядок;
                    //oldRow.порядок = lastPor;
                    //lastRow.порядок = oldPor;
                    (oldRow.порядок, lastRow.порядок) = (lastRow.порядок, oldRow.порядок);
                    //       дистанции_деталейЛист.Sort((a, b) => a.порядок.CompareTo(b.порядок));
                    bindingSource1.Sort = "порядок";
                    dataGridView1.Refresh();

                    label1.Visible = true;
                }
            }
            dataGridView1.Focus();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                результаты eRow = bindingSource1.Current as результаты;
                клТурист.выбран = false;
                список_участников выборТуриста = new список_участников();
                выборТуриста.Выход.Content = " Отмена";
                выборТуриста.Title = " Выберите участника";
                выборТуриста.наимен_слета.Text = выборТуриста.Title;
                выборТуриста.ShowDialog();
                if (выборТуриста.DialogResult ==true)
                {
                    туристы tRow = de.туристы.Single(n => n.турист == клТурист.турист);
                   
                    if (eRow.экипажи.туристы.Any(n => n.турист == tRow.турист))
                    {
                        MessageBox.Show("Уже в экипаже...");
                    }
                    else
                    {
                        eRow.экипажи.туристы.Add(tRow);
                    }
                    
                    de.SaveChanges();
                    dataGridView1.Refresh();
                }


            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                результаты eRow = bindingSource1.Current as результаты;
                if (eRow.экипажи.туристы.Any())
                {
                    eRow.экипажи.туристы.Clear();
                    label1.Visible = true;
                }
            }
            dataGridView1.Refresh();
            dataGridView1.Focus();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                //int maxPor = de.результаты.Local.Max(n => n.порядок);
                int maxPor = GetMaxPor();
                результаты tRow = bindingSource1.Current as результаты;
                Guid кодЭкипажа = tRow.экипаж;
                int maxPop = de.результаты.Local.Where(n => n.экипаж == tRow.экипаж).Max(n => n.попытка);

                экипажи dRow = de.экипажи.Local.Single(n => n.экипаж == кодЭкипажа);
                результаты нижняя = de.результаты.Local.Single(n => n.экипаж == tRow.экипаж && n.попытка == maxPop);
                int индекс = binList.IndexOf(нижняя);

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
                     старт=DateTime.Today,
                      финиш=DateTime.Today,
                       прим=""
                };
                //  bindingSource1.Add(newRow);
                binList.Insert(индекс + 1, newRow);
                label1.Visible = true;
                bindingSource1.Position = индекс + 1;

            }
            dataGridView1.Focus();

        }

        int GetMaxNum()
        {
            int maxNum = 0;
            if (de.экипажи.Local.Any())
            {
                maxNum = de.экипажи.Local.Max(n => n.номер);

            }
            //if (de.экипажи.Where(n => n.дистанции.слет == клСлет.слет)
            //    .Any(/*n => n.дистанции.вид == клВид.вид)*/))
            //{
            //    int max2 = de.экипажи
            //        //.Where(n => n.дистанции.слет == клСлет.слет)
            //        //.Where(n => n.дистанции.вид == клВид.вид)
            //        .Max(n => n.номер);
            //    if (max2 > maxNum)
            //    {
            //        maxNum = max2;
            //    }

            //}
            return maxNum;

        }

        int GetMaxPor()
        {
            int maxPor = 0;
            if (de.результаты.Local.Any())
            {
                maxPor = de.результаты.Local.Max(n => n.порядок);
            }

            //if (de.результаты
            //    //.Where(n => n.экипажи.дистанции.слет == клСлет.слет)
            //    //.Where(n => n.экипажи.дистанции.вид == клВид.вид)
            //    .Any())
            //{
            //    int max3 = de.результаты
            //        //.Where(n => n.экипажи.дистанции.слет == клСлет.слет)
            //        //.Where(n => n.экипажи.дистанции.вид == клВид.вид)
            //        .Max(n => n.порядок);
            //    if (max3 > maxPor)
            //    {
            //        maxPor = max3;
            //    }
            //}
            return maxPor;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            список_судов выборДистанции = new список_судов();
            выборДистанции.Выход.Content = "Отмена";
            выборДистанции.Title = " Выберите судно ";
            выборДистанции.наимен_слета.Text = выборДистанции.Title;
            выборДистанции.ShowDialog();
            if (выборДистанции.DialogResult == true)
            {
                список_школ выборШколы = new список_школ();
                выборШколы.Выход.Content = "Отмена";
                выборШколы.Title = " Выберите школу";
                выборШколы.наимен_слета.Text = выборШколы.Title;
                выборШколы.ShowDialog();
                if (выборШколы.DialogResult == true)
                {
                    школы выбр_школа = de.школы.Single(n => n.школа == клШкола.школа);
                    суда выбр_судно = de.суда.Single(n => n.судно == клСудно.судно);

                    int maxNum = GetMaxNum();
                    int maxPor = GetMaxPor();
                   

                    экипажи newЭкипаж = new экипажи()
                    {
                        экипаж = Guid.NewGuid(),
                        прим = "",
                        номер = maxNum + 1,
                        дистанция = клДистанция.дистанция,
                        место = 0,
                        итог = 0,
                        школа = клШкола.школа,
                        школы = выбр_школа,
                        судно = клСудно.судно,
                        суда = выбр_судно
                    };
                    de.экипажи.Local.Add(newЭкипаж);

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
                        финиш = DateTime.Today,
                         прим=""
                    };

                    
                    int stroka = bindingSource1.Add(newRow);
                    bindingSource1.Position = stroka;
                  

                    dataGridView1.Refresh();
                }
            }
            dataGridView1.Focus();

        }

        private void button10_Click(object sender, EventArgs e)
        {
            список_судов выборДистанции = new список_судов();
            выборДистанции.Выход.Content = "Отмена";
            выборДистанции.Title = " Выберите судно ";
            выборДистанции.наимен_слета.Text = выборДистанции.Title;
            выборДистанции.ShowDialog();
            if (выборДистанции.DialogResult == true)
            {
                список_участников выборТуриста = new список_участников();
                выборТуриста.Выход.Content = " Отмена";
                выборТуриста.Title = " Выберите участника";
                выборТуриста.наимен_слета.Text = выборТуриста.Title;
                выборТуриста.ShowDialog();
                if (выборТуриста.DialogResult == true)
                {
                    Guid код_школы = клТурист.deRow.школа;
                    школы выбр_школа = de.школы.Single(n => n.школа == код_школы);
                    суда выбр_судно = de.суда.Single(n => n.судно == клСудно.судно);
                    int maxNum = 0;
                    if (de.экипажи.Local.Any())
                    {
                        maxNum = de.экипажи.Local.Max(n => n.номер);
                    }
                    экипажи newRow = new экипажи()
                    {
                        экипаж = Guid.NewGuid(),
                        прим = "",
                        номер = maxNum + 1,
                        дистанция = клДистанция.дистанция,
                        место = 0,
                        итог = 0,
                        школа = код_школы,
                        школы = выбр_школа,
                        судно = клСудно.судно,
                        суда = выбр_судно
                    };

                    туристы tRow = de.туристы.Single(n => n.турист == клТурист.турист);




                    newRow.туристы.Add(tRow);


                    int maxPor = GetMaxPor();


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
                        финиш = DateTime.Today,
                        прим = ""
                    };

                    int строка = bindingSource1.Add(newRos);
                    bindingSource1.Position = строка;
                    dataGridView1.Refresh();
                    label1.Visible = true;
                    //    de.SaveChanges();

                }
            }
            dataGridView1.Focus();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                результаты tRow = bindingSource1.Current as результаты;
                Guid кодЭкипажа = tRow.экипаж;
                var ar = de.результаты.Local.Where(n => n.экипаж == tRow.экипаж).ToArray();
                foreach (результаты delRow in ar)
                {
                    de.результаты.Local.Remove(delRow);
                }
                экипажи dRow = de.экипажи.Local.Single(n => n.экипаж == кодЭкипажа);
                de.экипажи.Local.Remove(dRow);
                label1.Visible = true;
               

            }
            dataGridView1.Focus();
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
                dataGridView2.Columns[j].Tag =(Guid) eRow.ExtendedProperties["этап"];
                j++;
            }
            //int j = 0;
            //foreach (этапы eRow in de.этапы.Local)
            //{
            //    dataGridView2.Columns[j].HeaderText = eRow.наимен.Trim();
            //    dataGridView2.Columns[j].Tag = eRow.этап;
            //    j++;
            //}
        }
        void пересчетШтрафов()
        {
            if (bindingSource1.Count > 0)
            {
                результаты rRow = bindingSource1.Current as результаты;
                int сумма = 0;
                if (de.штрафы.Local.Any(n=>n.результат==rRow.результат))
                {
                    сумма = de.штрафы.Local.Where(n => n.результат == rRow.результат).Sum(n => n.секунд);
                }
                rRow.штраф = сумма;
                пересчет();
            }
            //var query = de.штрафы.Local
            //    .GroupBy(n => n.результат)
            //    .Select(n => new { результат = n.Key, сумма = n.Sum(p => p.секунд) }).ToDictionary(n => n.результат);

            ////var query = DicШтрафы
            ////    .GroupBy(n => n.Value.результат)
            ////    .Select(n => new { результат = n.Key, сумма = n.Sum(p => p.Value.секунд) }).ToDictionary(n => n.результат);


            //foreach (результаты rRow in de.результаты.Local)
            //{
            //    rRow.штраф = 0;
            //    if(query.ContainsKey(rRow.результат))
            //    {
            //        rRow.штраф = query[rRow.результат].сумма;
            //    }
            //}
         
            dataGridView1.Refresh();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            пересчетШтрафов();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            список_судов выборДистанции = new список_судов();
            выборДистанции.Выход.Content = "Отмена";
            выборДистанции.Title = " Выберите судно ";
            выборДистанции.наимен_слета.Text = выборДистанции.Title;
            выборДистанции.ShowDialog();
            if (выборДистанции.DialogResult == true)
            {

                       
                выборVучастников выборТуриста = new выборVучастников();
                выборТуриста.ShowDialog();
                if (выборТуриста.DialogResult == true)
                {
                    суда выбр_судно = de.суда.Single(n => n.судно == клСудно.судно);

                    int строка = 0;
                    foreach (туристы tur in клТурист.turList)
                    {
                        клШкола.школа = tur.школа;
                        школы выбр_школа = de.школы.Single(n => n.школа == клШкола.школа);
                        int maxPor = GetMaxPor();
                       
                        экипажи newRow = new экипажи()
                        {
                            экипаж = Guid.NewGuid(),
                            прим = "",
                            номер = maxPor + 1,
                            дистанция = клДистанция.дистанция,
                            место = 0,
                            итог = 0,
                            школа = клШкола.школа,
                            школы = выбр_школа,
                            судно = клСудно.судно,
                            суда = выбр_судно
                        };

                        туристы tRow = de.туристы.Single(n => n.турист == tur.турист);
                       

                        newRow.туристы.Add(tRow);

                        de.экипажи.Add(newRow);

                        int махПор = 0;
                        if (de.результаты.Local.Any())
                        {
                            махПор = de.результаты.Local.Max(n => n.порядок);
                        }
                        результаты новРез = new результаты
                        {
                            время_мин = 0,
                            время_сек = 0,
                            зачетный = false,
                            итог = 0,
                            попытка = 1,
                            порядок = махПор + 1,
                            результат = Guid.NewGuid(),
                            секунд = 0,
                            штраф = 0,
                            экипаж = newRow.экипаж,
                            экипажи = newRow,
                            старт = DateTime.Today,
                            финиш = DateTime.Today,
                            прим = ""
                        };

                        строка = bindingSource1.Add(новРез);
                        bindingSource1.Position = строка;
                        dataGridView1.Refresh();
                        //Console.WriteLine(tRow.фамилия);
                    }
                    bindingSource1.Position = строка;
                    dataGridView1.Refresh();
                    label1.Visible = true;
                }
            }
            dataGridView1.Focus();

        }
        void задать_ширину()
        {
            int столбцов = dataGridView1.Columns.Count;
            int[] aW = new int[столбцов];
            int i = 0;
            foreach(DataGridViewColumn tCol in dataGridView1.Columns)
            {
                aW[i] = tCol.Width;
                i++;
            }
            double сумма = aW.Sum();
            double ширина = Screen.PrimaryScreen.WorkingArea.Width-60;
            double поправка = ширина/сумма;
            i = 0;
            foreach (DataGridViewColumn tCol in dataGridView1.Columns)
            {
                double  ss = aW[i] * поправка;
                tCol.Width=(int) ss;
                i++;
            }
            dataGridView1.Refresh();
        }

        void задать2ширину()
        {
            int столбцов = dataGridView2.Columns.Count;
            if (столбцов > 10)
            {
                int[] aW = new int[столбцов];
                int i = 0;
                foreach (DataGridViewColumn tCol in dataGridView2.Columns)
                {
                    aW[i] = tCol.Width;
                    i++;
                }
                double сумма = aW.Sum();
                double ширина = Screen.PrimaryScreen.WorkingArea.Width - 60;
                double поправка = ширина / сумма;
                i = 0;
                foreach (DataGridViewColumn tCol in dataGridView2.Columns)
                {
                    double ss = aW[i] * поправка;
                    tCol.Width = (int)ss;
                    i++;
                }
                dataGridView2.Refresh();
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                результаты tRow = bindingSource1.Current as результаты;
                if (tRow.попытка > 1)
                {
                    bindingSource1.RemoveCurrent();

                    label1.Visible = true;
                  //  пересчет();
                }
               
               

            }
            dataGridView1.Focus();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (binList.Any(n => n.забег))
            {
               
                foreach (результаты rRow in binList.Where(n => n.забег))
                {
                    rRow.старт = DateTime.Now;
                    rRow.финиш = rRow.старт;
                    rRow.плывут = true;
                    //rRow.время_мин = 0;
                    //rRow.время_сек = 0;
                    //dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightCyan;
                    //dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Red;
                    //  dataGridView1.Refresh();
                    rRow.забег = false;
                }
                WMPLib.WindowsMediaPlayer wmp = new WMPLib.WindowsMediaPlayer();
                wmp.URL = "СтартМ.wav";
                wmp.controls.play();
            }
            else
            {
                MessageBox.Show("Отметьте экипажи...");
                // как не испортить результаты? Если время больше нуля вторую попытку..
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {


        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            var query = de.штрафы.Local
                .GroupBy(n => n.результат)
                .Select(n => new { результат = n.Key, сумма = n.Sum(p => p.секунд) }).ToDictionary(n => n.результат);

            foreach (результаты rRow in de.результаты.Local)
            {
                rRow.штраф = 0;
                if (query.ContainsKey(rRow.результат))
                {
                    rRow.штраф = query[rRow.результат].сумма;
                }
            }
            пересчет();
            dataGridView1.Refresh();
        }
    }
}
