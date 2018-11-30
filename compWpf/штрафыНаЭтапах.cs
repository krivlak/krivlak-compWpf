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
            de.этапы.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.порядок).Load();
            de.штрафы.Where(n => n.результаты.экипажи.дистанция == клДистанция.дистанция).Load();
            //   DicШтрафы = de.штрафы.Local.ToDictionary(n => ( n.результат, n.этап));

            de.суда.OrderBy(n => n.порядок).Load();

            создать_таблицу();

            dataGridView1.AutoGenerateColumns = true;
            bindingSource1.DataSource = dt;
            initGrid2();
            клСетка.задать_ширину(dataGridView1);

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
                dataGridView1.Columns[j].HeaderText = eRow.ExtendedProperties["заголовок"].ToString();
                dataGridView1.Columns[j].Tag = (Guid)eRow.ExtendedProperties["этап"];
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

    }
}
