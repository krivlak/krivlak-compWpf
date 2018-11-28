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
    public partial class все_результаты: Form
    {
        public все_результаты()
        {
            InitializeComponent();
        }


        //   Form окно_выбора;
     //   Entities de = клБаза.de;
        Entities de = new Entities();
        BindingList <результаты> binList;
        //дистанции deДистанция;
        //Guid кодДистанции;
        //DataTable dt;
        private void все_результаты_Load(object sender, EventArgs e)
        {
            //deДистанция = клДистанция.deRow;
            //кодДистанции = deДистанция.дистанция;
            this.Text ="Дистанции на "+клДистанция.наимен+" " +клСлет.наимен;
            label2.Text = this.Text;
            try
            {
             //   de.дистанции.Where(n=>n.вид==клВид.вид).Where(n => n.слет == клСлет.слет).OrderBy(n=>n.порядок).Load();
                de.экипажи.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.номер).Load();
                de.результаты.Where(n => n.экипажи.дистанция == клДистанция.дистанция).OrderBy(n => n.порядок).ThenBy(n => n.экипажи.номер).ThenBy(n => n.попытка).Load();

                de.этапы.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.порядок).Load();

                de.штрафы.Where(n => n.результаты.экипажи.дистанция == клДистанция.дистанция).Load();
                de.суда.OrderBy(n => n.порядок).Load();

          //      создать_таблицу();

         //       dataGridView2.AutoGenerateColumns = true;
         //       bindingSource2.DataSource = dt;
          //      initGrid2();

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
                            финиш = DateTime.Today
                        };


                        binList.Add(newRow);

                    }

                }

                пересчет();

                bindingSource1.DataSource = binList;
                bindingSource1.Sort = " номер, попытка";
            //    обновитьШтрафы();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки " + ex.Message);
            }
            bindingSource1.ListChanged += BindingSource1_ListChanged;
            timer2.Start();
            timer2.Tick += Timer2_Tick;
            FormClosing += Список_видов_FormClosing;
      
            //this.Text = deДистанция.наимен;
            label2.Text = this.Text;
            //bindingSource1.PositionChanged += BindingSource1_PositionChanged;
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            ////            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.DataError += DataGridView1_DataError;
            dataGridView1.CellPainting += DataGridView1_CellPainting;
            dataGridView1.CellMouseClick += DataGridView1_CellMouseClick;
            dataGridView1.EditingControlShowing += dataGridView1_EditingControlShowing;

        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
          if(dataGridView1.Columns[e.ColumnIndex]==примColumn)
            {
                if(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value== null )
                {
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                }
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

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;

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

        private void DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex < dataGridView1.Columns.Count)
            {
                e.PaintBackground(e.CellBounds, true);
                e.Graphics.TranslateTransform(e.CellBounds.Left, e.CellBounds.Bottom);
                e.Graphics.RotateTransform(270);
                e.Graphics.DrawString(e.FormattedValue?.ToString(), e.CellStyle.Font, Brushes.Black, 5, 5);
                e.Graphics.ResetTransform();
                e.Handled = true;
                //dataGridView1.ColumnHeadersHeight = 100;
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
                 //   клДистанция.formList.Remove(this);
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
                пересчет();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            список_судов выборДистанции = new список_судов();
            выборДистанции.Выход.Content = "Отмена";
            выборДистанции.Title = " Выберите судно ";
            выборДистанции.наимен_слета.Text = выборДистанции.Title;
            выборДистанции.ShowDialog();
            if(выборДистанции.DialogResult==true)
            {
                список_школ выборШколы = new список_школ();
                выборШколы.Выход.Content = "Отмена";
                выборШколы.Title = " Выберите школу";
                выборШколы.наимен_слета.Text = выборШколы.Title;
                выборШколы.ShowDialog();
                if (выборШколы.DialogResult == true)
                {
                    школы выбр_школа= de.школы.Single(n => n.школа == клШкола.школа);
                    суда выбр_судно = de.суда.Single(n => n.судно == клСудно.судно);
                    int maxNum = 0;
                    int maxPor = 0;
                    if (de.экипажи.Local.Any())
                    {
                        maxNum = de.экипажи.Local.Max(n => n.номер);

                    }
                    if (de.результаты.Local.Any())
                    {
                        maxPor = de.результаты.Local.Max(n => n.порядок);
                    }
                    экипажи newЭкипаж = new экипажи()
                    {
                        экипаж = Guid.NewGuid(),
                        прим = "",
                        номер = maxNum + 1,
                        судно = клСудно.судно,
                        место = 0,
                        итог = 0,
                        школа = клШкола.школа,
                        школы = выбр_школа,
                        суда = выбр_судно,
                        дистанция = клДистанция.дистанция
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
                        прим = "",
                        плывут = false
                    };

                   
                    int stroka = bindingSource1.Add(newRow);
                    bindingSource1.Position = stroka;
                    
                    //bindingSource1.Add(newRow2);

                    dataGridView1.Refresh();
                }
                
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                результаты tRow = bindingSource1.Current as результаты;
                Guid кодЭкипажа = tRow.экипаж;
                результаты[] ar = de.результаты.Local.Where(n => n.экипаж == tRow.экипаж).ToArray();
                foreach (результаты delRow in ar)
                {
                    de.результаты.Local.Remove(delRow);
                }
                экипажи dRow = de.экипажи.Local.Single(n => n.экипаж == кодЭкипажа);
                de.экипажи.Local.Remove(dRow);
                label1.Visible = true;
                dataGridView1.Focus();

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                int maxPor = de.результаты.Local.Max(n => n.порядок);

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
                    старт = DateTime.Today,
                    финиш = DateTime.Today,
                    прим = "",
                    плывут = false
                };
                //  bindingSource1.Add(newRow);
                binList.Insert(индекс + 1, newRow);
                label1.Visible = true;

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
                dataGridView1.Focus();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                результаты eRow = bindingSource1.Current as результаты;
                клТурист.выбран = false;
                список_участников выборТуриста = new список_участников();
                выборТуриста.Выход.Content = "Отмена";
                выборТуриста.Title = " Выберите участника";
                выборТуриста.наимен_слета.Text = выборТуриста.Title;
                выборТуриста.ShowDialog();
                if (выборТуриста.DialogResult == true)
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

                    //  de.SaveChanges();
                    label1.Visible = true;
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
                выборТуриста.Выход.Content = "Отмена";
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
                        судно = клСудно.судно,
                        место = 0,
                        итог = 0,
                        школа = код_школы,
                        школы = выбр_школа,
                         суда=выбр_судно,
                          дистанция=клДистанция.дистанция
                    };

                    туристы tRow = de.туристы.Single(n => n.турист == клТурист.турист);

                    newRow.туристы.Add(tRow);


                    int maxPor = 0;
                    if (de.результаты.Local.Any())
                    {
                        maxPor = de.результаты.Local.Max(n => n.порядок);
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
                        финиш = DateTime.Today,
                        прим = "",
                        плывут = false

                    };

                    int строка = bindingSource1.Add(newRos);
                    bindingSource1.Position = строка;
                    dataGridView1.Refresh();

                    //    de.SaveChanges();
                    dataGridView1.Refresh();
                }
            }

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
                выборVтуристов выборТуриста = new выборVтуристов();
                выборТуриста.ShowDialog();
                if (выборТуриста.DialogResult == true)
                {
                    int строка = 0;
                    foreach (туристы tur in клТурист.turList)
                    {
                        клШкола.школа = tur.школа;
                        школы выбр_школа = de.школы.Single(n => n.школа == клШкола.школа);
                        суда выбр_судно = de.суда.Single(n => n.судно == клСудно.судно);
                        int maxPor = 0;
                        if (de.экипажи.Local.Any())
                        {
                            maxPor = de.экипажи.Local.Max(n => n.номер);
                        }
                        экипажи newRow = new экипажи()
                        {
                            экипаж = Guid.NewGuid(),
                            прим = "",
                            номер = maxPor + 1,
                            //дистанция = клДистанция.дистанция,
                            место = 0,
                            итог = 0,
                            школа = клШкола.школа,
                            школы = выбр_школа,
                             суда=выбр_судно,
                             дистанция=клДистанция.дистанция,
                              судно= клСудно.судно
                        };

                        туристы tRow = de.туристы.Single(n => n.турист == tur.турист);
                        //матросы newTur = new матросы()
                        //{
                        //    турист = tur.турист,
                        //    экипаж = newRow.экипаж
                        //    //       туристы =tRow
                        //    // экипажи=eRow
                        //};

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
                }
            }


        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                результаты oldRow = bindingSource1.Current as результаты;
                int oldIndex = bindingSource1.Position;

                int oldPor = oldRow.порядок;
                if (bindingSource1.Position > 0)
                {
                    bindingSource1.MovePrevious();

                    результаты lastRow = bindingSource1.Current as результаты;
                    int lastPor = lastRow.порядок;
                    oldRow.порядок = lastPor;
                    lastRow.порядок = oldPor;
                    //       дистанции_деталейЛист.Sort((a, b) => a.порядок.CompareTo(b.порядок));
                    bindingSource1.Sort = "порядок";
                    dataGridView1.Refresh();

                    label1.Visible = true;
                }
            }
            dataGridView1.Focus();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                результаты oldRow = bindingSource1.Current as результаты;

                int oldPor = oldRow.порядок;
                if (bindingSource1.Position < bindingSource1.Count - 1)
                {
                    bindingSource1.MoveNext();
                    результаты lastRow = bindingSource1.Current as результаты;
                    int lastPor = lastRow.порядок;
                    oldRow.порядок = lastPor;
                    lastRow.порядок = oldPor;
                    bindingSource1.Sort = "порядок";

                    //   результаты_деталейЛист.Sort((a, b) => a.порядок.CompareTo(b.порядок));
                    dataGridView1.Refresh();
                    label1.Visible = true;

                }
            }
            dataGridView1.Focus();
        }

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


                tab1.Rows[1].Cells[1].Range.Text = "Протоколы результатов по виду "+клВид.наимен+ " на "+ клСлет.наимен;
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
                        .Where(n=>n.экипажи.судно==dRow.судно)
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
                    .Where(n=>n.экипажи.судно==dRow.судно)
                    .Where(n => n.итог > 0)
                    .GroupBy(n => n.экипаж)
                    .Select(n => new { экипаж = n.Key, лучший = n.Min(p => p.итог) }).ToDictionary(n => n.экипаж);

                //            foreach (результаты rRow in de.результаты.Local.Where(n => n.итог > 0).OrderBy(n => n.номер).ThenByDescending(n => n.итог))
                foreach (результаты rRow in de.результаты.Local
                    .Where(n => n.экипажи.судно == dRow.судно)  )
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

                foreach (экипажи uRow in de.экипажи.Local.Where(n => n.судно == dRow.судно)       )
                {

                    uRow.место = 0;


                }
                int j = 0;
                foreach (экипажи uRow in de.экипажи.Local.Where(n=>n.судно==dRow.судно)  .Where(n => n.итог > 0).OrderBy(n => n.итог))
                {
                    j++;
                    uRow.место = j;


                }
            }
            dataGridView1.Refresh();
            label1.Visible = true;
        }
        private void Timer2_Tick(object sender, EventArgs e)
        {
            int j = 0;
            foreach (результаты rRow in de.результаты.Local.Where(n => n.плывут))
            {
                j++;
                rRow.финиш = DateTime.Now;
                //int секунд77 = (rRow.финиш.Value.Hour * 60 * 60 + rRow.финиш.Value.Minute * 60 + rRow.финиш.Value.Second) - (rRow.старт.Value.Hour * 60 * 60 + rRow.старт.Value.Minute * 60 + rRow.старт.Value.Second);
                //rRow.время = DateTime.Today.AddSeconds(секунд77);
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

        private void button12_Click(object sender, EventArgs e)
        {

        }
    }
}
