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
    public partial class окно1результатов : Form
    {
        public окно1результатов(Form fr)
        {
            InitializeComponent();
            окно_выбора = fr;
        }

        Form окно_выбора;
        Entities de = new Entities();
        BindingList<результаты> binList;
        дистанции deДистанция ;
        Guid кодДистанции ;
        DateTime date1=DateTime.Today;
        private void окно1результатов_Load(object sender, EventArgs e)
        {
            deДистанция = клДистанция.deRow;
            кодДистанции = deДистанция.дистанция;
            this.Text = deДистанция.наимен;
            label2.Text = this.Text;
            try
            {
                de.экипажи
                    //.Where(n => n.дистанция == deДистанция.дистанция)
                    .OrderBy(n => n.номер).Load();
                de.результаты
                    //.Where(n => n.экипажи.дистанция == deДистанция.дистанция)
                    .OrderBy(n => n.порядок).ThenBy(n => n.экипажи.номер).ThenBy(n => n.попытка).Load();
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
               

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки " + ex.Message);
            }
            timer2.Start();
            FormClosing += Список_видов_FormClosing;
            bindingSource1.ListChanged += BindingSource1_ListChanged;
            this.Text = deДистанция.наимен;
            label2.Text = this.Text;
            dataGridView1.DataError += DataGridView1_DataError;
         //   dataGridView1.CellMouseDown += DataGridView1_CellMouseDown;
          //  dataGridView1.CellMouseClick += DataGridView1_CellMouseClick;
            timer2.Tick += Timer2_Tick;
            dataGridView1.CellContextMenuStripNeeded += DataGridView1_CellContextMenuStripNeeded;
            dataGridView1.CellMouseClick += DataGridView1_CellMouseClick;
            dataGridView1.CellPainting += DataGridView1_CellPainting;
          
        }

        private void DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            Font font = label2.Font;
            if (e.RowIndex == -1 && e.ColumnIndex < dataGridView1.Columns.Count)
            {
                e.PaintBackground(e.CellBounds, true);
                e.Graphics.TranslateTransform(e.CellBounds.Left, e.CellBounds.Bottom);
                e.Graphics.RotateTransform(270);
                e.Graphics.DrawString(e.FormattedValue?.ToString(), e.CellStyle.Font, Brushes.Black, 5, 5);
                e.Graphics.ResetTransform();
                e.Handled = true;
               // dataGridView1.ColumnHeadersHeight = 50;
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

        private void DataGridView1_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
        }

    

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
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
                    клДистанция.formList.Remove(this);
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
                if (rRow.старт != null && rRow.финиш != null)
                {
                    if (rRow.финиш > rRow.старт)
                    {
                        //rRow.набежало = rRow.финиш.Hour * 60 * 60 + rRow.финиш.Minute * 60 + rRow.финиш.Second - rRow.старт.Hour * 60 * 60 - rRow.старт.Minute * 60 - rRow.старт.Second;
                    }
                }

                rRow.секунд = rRow.время_мин * 60 + rRow.время_сек;
                rRow.итог = rRow.секунд + rRow.штраф;
            }

            var query = de.результаты.Local
                .Where(n => n.итог > 0)
                .GroupBy(n => n.экипаж)
                .Select(n => new { экипаж = n.Key, лучший = n.Min(p => p.итог) }).ToDictionary(n => n.экипаж);

            //            foreach (результаты rRow in de.результаты.Local.Where(n => n.итог > 0).OrderBy(n => n.номер).ThenByDescending(n => n.итог))
            foreach (результаты rRow in de.результаты.Local)
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

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
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


                tab1.Rows[1].Cells[1].Range.Text = this.Text;
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

        private void button6_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {
                результаты eRow = bindingSource1.Current as результаты;
                клТурист.выбран = false;
                выбор_туриста выборТуриста = new выбор_туриста();
                выборТуриста.ShowDialog();
                if (выборТуриста.DialogResult == true)
                {
                    туристы tRow = de.туристы.Single(n => n.турист == клТурист.турист);
                    //матросы newRow = new матросы()
                    //{
                    //    турист = клТурист.турист,
                    //    экипаж = eRow.экипаж
                    //    //туристы =клТурист.deRow,
                    //    // экипажи=eRow
                    //};

                    if (eRow.экипажи.туристы.Any(n => n.турист == tRow.турист))
                    {
                        MessageBox.Show("Уже в экипаже...");
                    }
                    else
                    {
                        eRow.экипажи.туристы.Add(tRow);
                    }
                    //      заполнить_состав(eRow);
                    //                 label1.Visible = true;
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
                     старт=DateTime.Today,
                      финиш=DateTime.Today
                };
                //  bindingSource1.Add(newRow);
                binList.Insert(индекс + 1, newRow);
                label1.Visible = true;

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            выбор_школы выборШколы = new выбор_школы();
            выборШколы.ShowDialog();
            if (выборШколы.DialogResult == true)
            {
                школы sRow = de.школы.Single(n => n.школа == клШкола.школа);
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
                    //дистанция = deДистанция.дистанция,
                    место = 0,
                    итог = 0,
                    школа = клШкола.школа,
                    школы = sRow


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
                     старт=DateTime.Today,
                      финиш = DateTime.Today
                };

                //результаты newRow2 = new результаты
                //{
                //    итог = 0,
                //    время_сек = 0,
                //    время_мин = 0,
                //    попытка = 2,
                //    результат = Guid.NewGuid().ToString(),
                //    секунд = 0,
                //    штраф = 0,
                //    экипаж = newЭкипаж.экипаж,
                //    экипажи = newЭкипаж,
                //     зачетный=false
                //};
             int stroka=   bindingSource1.Add(newRow);
                bindingSource1.Position = stroka;
                //bindingSource1.Add(newRow2);

                dataGridView1.Refresh();
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            выбор_туриста выборТуриста = new выбор_туриста();
            выборТуриста.ShowDialog();
            if (выборТуриста.DialogResult == true)
            {

                школы sRow = de.школы.Single(n => n.школа == клШкола.школа);
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
                    //дистанция = deДистанция.дистанция,
                    место = 0,
                    итог = 0,
                    школа = клШкола.школа,
                    школы = sRow
                };

                туристы tRow = de.туристы.Single(n => n.турист == клТурист.турист);
                //матросы newTur = new матросы()
                //{
                //    турист = клТурист.турист,
                //    экипаж = newRow.экипаж
                //    //туристы =клТурист.deRow,
                //    // экипажи=eRow
                //};



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
                    финиш = DateTime.Today
                };

                int строка = bindingSource1.Add(newRos);
                bindingSource1.Position = строка;
                dataGridView1.Refresh();

                //    de.SaveChanges();
                dataGridView1.Refresh();
            }

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
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //плавающее окно = new плавающее();
            //окно.ShowDialog();
            окно_выбора.Activate();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            выборVтуристов выборТуриста = new выборVтуристов();
            выборТуриста.ShowDialog();
            if (выборТуриста.DialogResult == true)
            {
                int строка = 0;
                foreach (туристы tur in клТурист.turList)
                {
                    клШкола.школа = tur.школа;
                    школы sRow = de.школы.Single(n => n.школа == клШкола.школа);
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
                        школы = sRow
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
                        финиш = DateTime.Today

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


        private void Timer2_Tick(object sender, EventArgs e)
        {
            int j = 0;
            foreach (результаты rRow in de.результаты.Local.Where(n => n.плывут))
            {
                j++;
                rRow.финиш = DateTime.Now;
                //int секунд77 = (rRow.финиш.Value.Hour * 60 * 60 + rRow.финиш.Value.Minute * 60 + rRow.финиш.Value.Second) - (rRow.старт.Value.Hour * 60 * 60 + rRow.старт.Value.Minute * 60 + rRow.старт.Value.Second);
                //rRow.время = DateTime.Today.AddSeconds(секунд77);
                var rtr =(rRow.финиш-rRow.старт);
                rRow.время_мин = rtr.Minutes;
                rRow.время_сек = rtr.Seconds;
                rRow.миллисекунд = rtr.Milliseconds;
            }
            if (j > 0)
            {
                dataGridView1.Refresh();
            }
        }
      

        private void стартToolStripMenuItem_Click(object sender, EventArgs e)
        {
            результаты rRow = bindingSource1.Current as результаты;
           
                rRow.старт = DateTime.Now;
                rRow.финиш = rRow.старт;
                rRow.плывут = true;

           // dataGridView1.CurrentCell.Style.SelectionForeColor = Color.Red;

            dataGridView1.Refresh();
        }

        private void финишToolStripMenuItem_Click(object sender, EventArgs e)
        {
            результаты rRow = bindingSource1.Current as результаты;
            rRow.плывут = false;
            пересчет(); 
       //     MessageBox.Show((rRow.финиш - rRow.старт).Value.ToString());
           // dataGridView1.CurrentCell.Style.SelectionForeColor = Color.Green;
            //    dataGridView1.CurrentCell.Style.ForeColor = Color.;

        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count > 0)
            {

                результаты tRow = bindingSource1.Current as результаты;
                клРезультат.deRow = tRow;
                клРезультат.результат = tRow.результат;

                Cursor = Cursors.WaitCursor;
                //штрафы1время форма33 = new штрафы1время(кодДистанции);
                штрафы_этапы форма33 = new штрафы_этапы();
                форма33.Text = "Штрафы и время № " + tRow.номер.ToString() + " клуб " + tRow.клуб + " экипаж " + tRow.состав;
                форма33.Tag = tRow;
                клРезультат.formList.Add(форма33);
                форма33.ShowDialog();
                пересчет();
                dataGridView1.Refresh();
                label1.Visible = true;
                Cursor = Cursors.Default;

                //bool xy = true;
                //Form форма = null;
                //foreach (Form fr in клРезультат.formList)
                //{
                //    результаты rRow = (результаты)fr.Tag;
                //    Guid код = rRow.результат;
                //    if (код == tRow.результат)
                //    {
                //        xy = false;
                //        форма = fr;
                //    }
                //}
                //if (xy)
                //{

                //    Cursor = Cursors.WaitCursor;
                //    //штрафы1время форма33 = new штрафы1время(кодДистанции);
                //    штрафы_этапы форма33 = new штрафы_этапы();
                //    форма33.Text = "Штрафы и время № " + tRow.номер.ToString() + " клуб " + tRow.клуб + " экипаж " + tRow.состав;
                //    форма33.Tag = tRow;
                //    клРезультат.formList.Add(форма33);
                //    форма33.Show();
                //    пересчет();
                //    dataGridView1.Refresh();
                //    label1.Visible = true;
                //    Cursor = Cursors.Default;
                //}
                //else
                //{
                //    // MessageBox.Show("Уже открыта ..");
                //    if (форма != null)
                //    {
                //        форма.Activate();
                //    }
                //}





                //штрафы1время форма33 = new штрафы1время(кодДистанции);
                //форма33.Text = "Штрафы и время № " + tRow.номер.ToString() + " клуб " + tRow.клуб + " экипаж " + tRow.состав;
                //форма33.Show();

            }
        }
    }
}
