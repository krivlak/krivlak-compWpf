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
using Excel = Microsoft.Office.Interop.Excel;



namespace compWpf
{
    public partial class штрафыНаЭтапах : Form
    {
        public штрафыНаЭтапах()
        {
            InitializeComponent();
        }
        Entities de = new Entities();

        DataTable dt;
        private void штрафыНаЭтапах_Load(object sender, EventArgs e)
        {
            try
            {
                de.экипажи.Where(n => n.дистанция == клДистанция.дистанция)
                    .Where(n => n.судно == клСудно.судно)
                    .Where(n => n.итог > 0)
                    .OrderBy(n => n.место).Load();
                de.этапы.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.порядок).Load();
                de.штрафы.Where(n => n.результаты.экипажи.дистанция == клДистанция.дистанция).Load();
                de.результаты.Where(n => n.экипажи.дистанция == клДистанция.дистанция).Load();

                //   DicШтрафы = de.штрафы.Local.ToDictionary(n => ( n.результат, n.этап));

                de.суда.OrderBy(n => n.порядок).Load();

                создать_таблицу();
                foreach (экипажи eRow in de.экипажи.Local)
                {


                    foreach (результаты rRow in de.результаты.Local.Where(n => n.экипаж == eRow.экипаж).Where(n => n.итог == eRow.итог))
                    {
                        DataRow dr = dt.NewRow();
                        dr.SetField<int>("номерColumn", eRow.номер);
                        dr.SetField<string>("клубColumn", eRow.школы.наимен);
                        dr.SetField<string>("составColumn", eRow.состав);
                        foreach (штрафы sRow in de.штрафы.Local.Where(n => n.результат == rRow.результат))
                        {
                            foreach (DataColumn dCol in dt.Columns)
                            {
                                Guid kod = (Guid)dCol.ExtendedProperties["этап"];
                                //Console.Write(kod);
                                //Console.WriteLine(sRow.штраф);
                                if (kod == sRow.этап)
                                {
                                    dr.SetField<int>(dCol, sRow.секунд);
                                }
                            }
                        }
                        dr.SetField<int>("минутColumn", rRow.время_мин);
                        dr.SetField<int>("секундColumn", rRow.время_сек);
                        dr.SetField<int>("в_секColumn", rRow.секунд);
                        dr.SetField<int>("штрафColumn", rRow.штраф);
                        dr.SetField<int>("итогColumn", eRow.итог);
                        dr.SetField<int>("местоColumn", eRow.место);


                        dt.Rows.Add(dr);
                    }

                }

                dataGridView1.AutoGenerateColumns = true;
                bindingSource1.DataSource = dt;
                initGrid();
                клСетка.задать_ширину(dataGridView1);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сбой загрузки {ex.Message}");
            }

        }
        void создать_таблицу()
        {
            dt = new DataTable();

            DataColumn dc0 = new DataColumn
            {
                Caption = "номер",
                DataType = typeof(int),
                ColumnName = "номерColumn",
                DefaultValue = 0,
                ReadOnly = true,
                Unique = false
            };
            dc0.ExtendedProperties.Add("этап", Guid.Empty);
            dc0.ExtendedProperties.Add("заголовок", "номер");
            dc0.ExtendedProperties.Add("ширина", 1);
            dt.Columns.Add(dc0);


            DataColumn dc = new DataColumn
            {
                Caption = "клуб",
                DataType = typeof(string),
                ColumnName = "клубColumn",
                DefaultValue = "",
                MaxLength = 50,
                ReadOnly = true,
                Unique = false
            };
            dc.ExtendedProperties.Add("этап", Guid.Empty);
            dc.ExtendedProperties.Add("заголовок", "клуб");
            dc.ExtendedProperties.Add("ширина", 2);
            dt.Columns.Add(dc);

            DataColumn dc2 = new DataColumn
            {
                Caption = "состав",
                DataType = typeof(string),
                ColumnName = "составColumn",
                DefaultValue = "",
                MaxLength = 100,
                ReadOnly = true,
                Unique = false
            };
            dc2.ExtendedProperties.Add("этап", Guid.Empty);
            dc2.ExtendedProperties.Add("заголовок", "состав");
            dc2.ExtendedProperties.Add("ширина", 2);

            dt.Columns.Add(dc2);

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
                dc1.ExtendedProperties.Add("ширина", 1);

                dt.Columns.Add(dc1);
            }

            DataColumn dc3 = new DataColumn
            {
                Caption = "мин",
                DataType = typeof(int),
                ColumnName = "минутColumn",
                DefaultValue = 0,
                ReadOnly = true,
                Unique = false
            };
            dc3.ExtendedProperties.Add("этап", Guid.Empty);
            dc3.ExtendedProperties.Add("заголовок", "мин");
            dc3.ExtendedProperties.Add("ширина", 1);

            dt.Columns.Add(dc3);


            DataColumn dc4 = new DataColumn
            {
                Caption = "сек",
                DataType = typeof(int),
                ColumnName = "секундColumn",
                DefaultValue = 0,
                ReadOnly = true,
                Unique = false
            };
            dc4.ExtendedProperties.Add("этап", Guid.Empty);
            dc4.ExtendedProperties.Add("заголовок", "сек");
            dc4.ExtendedProperties.Add("ширина", 1);

            dt.Columns.Add(dc4);

            DataColumn dc5 = new DataColumn
            {
                Caption = "в сек",
                DataType = typeof(int),
                ColumnName = "в_секColumn",
                DefaultValue = 0,
                ReadOnly = true,
                Unique = false
            };
            dc5.ExtendedProperties.Add("этап", Guid.Empty);
            dc5.ExtendedProperties.Add("заголовок", "в сек");
            dc5.ExtendedProperties.Add("ширина", 1);

            dt.Columns.Add(dc5);

            DataColumn dc6 = new DataColumn
            {
                Caption = "штраф",
                DataType = typeof(int),
                ColumnName = "штрафColumn",
                DefaultValue = 0,
                ReadOnly = true,
                Unique = false
            };
            dc6.ExtendedProperties.Add("этап", Guid.Empty);
            dc6.ExtendedProperties.Add("заголовок", "штраф");
            dc6.ExtendedProperties.Add("ширина", 1);

            dt.Columns.Add(dc6);

            DataColumn dc7 = new DataColumn
            {
                Caption = "итог",
                DataType = typeof(int),
                ColumnName = "итогColumn",
                DefaultValue = 0,
                ReadOnly = true,
                Unique = false
            };
            dc7.ExtendedProperties.Add("этап", Guid.Empty);
            dc7.ExtendedProperties.Add("заголовок", "итог");
            dc7.ExtendedProperties.Add("ширина", 1);

            dt.Columns.Add(dc7);

            DataColumn dc8 = new DataColumn
            {
                Caption = "место",
                DataType = typeof(int),
                ColumnName = "местоColumn",
                DefaultValue = 0,
                ReadOnly = true,
                Unique = false
            };
            dc8.ExtendedProperties.Add("этап", Guid.Empty);
            dc8.ExtendedProperties.Add("заголовок", "место");
            dc8.ExtendedProperties.Add("ширина", 1);

            dt.Columns.Add(dc8);

            //   DataRow dr = dt.NewRow();

            // dt.Rows.Add(dr);

        }
        void initGrid()
        {

            int j = 0;
            foreach (DataColumn eRow in dt.Columns)
            {
                dataGridView1.Columns[j].HeaderText = eRow.ExtendedProperties["заголовок"].ToString();
                dataGridView1.Columns[j].Tag = (Guid)eRow.ExtendedProperties["этап"];
                int ширина = (int)eRow.ExtendedProperties["ширина"];
                dataGridView1.Columns[j].Width = ширина * 50;

                j++;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            Word.Application oWord = new Word.Application();

            string curDir = System.IO.Directory.GetCurrentDirectory();

            object шаблон = curDir + @"\штрафыНаЭтапах.dot";
            if (!System.IO.File.Exists(шаблон.ToString()))
            {
                MessageBox.Show("Нет файла " + шаблон.ToString());
                return;
            }



            //string наименФилиала = de.филиалы
            //    .OrderBy(n => n.порядок)
            //    .First().наимен;


            Word.Document o = oWord.Documents.Add(Template: шаблон);
            oWord.Application.Visible = true;
            //o.Bookmarks["услуга"].Range.Text = клУслуга.наимен;
            //o.Bookmarks["улица"].Range.Text = клУлица.наимен;
            //o.Bookmarks["филиал"].Range.Text = наименФилиала;
            //o.Bookmarks["дата"].Range.Text = DateTime.Today.ToShortDateString();

            o.Tables[1].Rows[1].Cells[2].Range.Text = клСлет.наимен;
            o.Tables[1].Rows[2].Cells[2].Range.Text = клДистанция.наимен;
            o.Tables[1].Rows[3].Cells[2].Range.Text = клСудно.наимен;


            o.Tables[2].Rows[1].Cells[2].Range.Text = dt.Columns[1].Caption;

            int i = 0;
            foreach (DataColumn nCol in dt.Columns)
            {
                i++;
                if (nCol.Ordinal > 3)
                {
                    o.Tables[2].Columns.Add();
                }
                if (nCol.Ordinal > 2)
                {
                    o.Tables[2].Rows[1].Cells[i].Range.Text = nCol.Caption;
                }
            }

            int j = 1;
            //  string dd = "";
            foreach (DataRow uRow in dt.Rows)
            {
                j++;
                o.Tables[2].Rows[j].Cells[1].Range.Text = uRow.Field<int>("номерColumn").ToString("0;#;#").Trim();
                o.Tables[2].Rows[j].Cells[2].Range.Text = uRow.Field<string>("клубColumn").Trim();
                o.Tables[2].Rows[j].Cells[3].Range.Text = uRow.Field<string>("составColumn").Trim();
                int r = 3;
                int p = 0;
                foreach (этапы eRow in de.этапы.Local)
                {
                    r++;
                    p++;
                    string поле = $"col{p}";
                    o.Tables[2].Rows[j].Cells[r].Range.Text = uRow.Field<int>(поле).ToString("0;#;#");
                }

                o.Tables[2].Rows[j].Cells[r + 1].Range.Text = uRow.Field<int>("минутColumn").ToString("0;#;#").Trim();
                o.Tables[2].Rows[j].Cells[r + 2].Range.Text = uRow.Field<int>("секундColumn").ToString("0;#;#").Trim();
                o.Tables[2].Rows[j].Cells[r + 3].Range.Text = uRow.Field<int>("в_секColumn").ToString("0;#;#").Trim();
                o.Tables[2].Rows[j].Cells[r + 4].Range.Text = uRow.Field<int>("штрафColumn").ToString("0;#;#").Trim();
                o.Tables[2].Rows[j].Cells[r + 5].Range.Text = uRow.Field<int>("итогColumn").ToString("0;#;#").Trim();
                o.Tables[2].Rows[j].Cells[r + 6].Range.Text = uRow.Field<int>("местоColumn").ToString("0;#;#").Trim();
                if (r > 1)
                {
                    o.Tables[2].Rows.Add();
                }
            }
            oWord.Application.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        //dr.SetField<int>("минутColumn", rRow.время_мин);
        //                dr.SetField<int>("секундColumn", rRow.время_сек);
        //                dr.SetField<int>("в_секColumn", rRow.секунд);
        //                dr.SetField<int>("штрафColumn", rRow.штраф);
        //                dr.SetField<int>("итогColumn", eRow.итог);
        //                dr.SetField<int>("местоColumn", eRow.место);

        //private void button1_Click(object sender, EventArgs e)
        //{


        //    int строк = архив.Rows.Count + 1;
        //    int столбцов = архив.Columns.Count;
        //    object[,] aRow = new object[строк, столбцов];

        //    aRow[0, 0] = "дом\n кв.";
        //    for (int i = 1; i < столбцов; i++)
        //    {
        //        aRow[0, i] = архив.Columns[i].Caption.Trim();
        //    }

        //    for (int j = 1; j < строк - 1; j++)
        //    {

        //        aRow[j, 0] = архив.Rows[j].Field<int>(0).ToString();

        //        for (int i = 1; i < столбцов; i++)
        //        {
        //            aRow[j, i] = архив.Rows[j].Field<string>(i);
        //        }
        //    }

        //    массив_excel(aRow, строк, столбцов);

        //}

        //private void массив_excel(object[,] массив, int строк, int столбцов)
        //{


        //    Excel.Application oExcel = new Microsoft.Office.Interop.Excel.Application();

        //    string curDir = System.IO.Directory.GetCurrentDirectory();

        //    object шаблон = curDir + @"\платежи_улица.xlt";
        //    if (!System.IO.File.Exists(шаблон.ToString()))
        //    {
        //        MessageBox.Show("Нет файла " + шаблон.ToString());
        //        return;
        //    }

        //    Microsoft.Office.Interop.Excel.Workbook o = oExcel.Workbooks.Add(Template: шаблон);

        //    Excel.Worksheet eList = (Excel.Worksheet)o.Worksheets[1];
        //    //  Excel.ListObject tab1 = eList.ListObjects[1];
        //    oExcel.Application.Visible = true;


        //    eList.Cells[1, 2] = this.Text + "   " + DateTime.Today.ToShortDateString();
        //    eList.Cells[3, 1].Select();
        //    eList.Cells[3, 1].Copy();

        //    Excel.Range rg = eList.get_Range("A3", Type.Missing);
        //    rg = rg.get_Resize(строк - 1, столбцов);
        //    rg.PasteSpecial(Excel.XlPasteType.xlPasteFormats);
        //    rg.NumberFormat = "@";
        //    rg.HorizontalAlignment = Excel.Constants.xlRight;
        //    rg.VerticalAlignment = Excel.Constants.xlTop;
        //    //            eList.Cells[3, 1].Paste();
        //    //    o.ActiveSheet.Paste();
        //    rg.Value2 = массив;

        //    eList.Cells[3, 1].Select();
        //    oExcel.Application.Visible = true;

        //}

    }
}
