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
    public partial class проба1 : Form
    {
        public проба1()
        {
            InitializeComponent();
        }
        compWpf.Entities de = new Entities();
        Dictionary<Guid,byte[]> dic1 = new Dictionary<Guid, byte[]>();
        private void проба1_Load(object sender, EventArgs e)
        {
            de.суда.OrderBy(n => n.порядок).Load();
           
            var query = de.суда.Local.Select(n => new { n.судно, n.версия }).ToArray().ToDictionary(n => n.судно);
            bindingSource1.DataSource = de.суда.Local.ToBindingList();

            textBox1.DataBindings.Add("Text", bindingSource1, "наимен");
            textBox1.Validating += TextBox1_Validating;
            dataGridView1.CellValidating += DataGridView1_CellValidating;

        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex] == наименColumn)
            {
                var obj2 = e.FormattedValue;
                
                 if(obj2  == null || obj2.ToString() == String.Empty)
                {
                    e.Cancel = true;
                    MessageBox.Show("Наименование не должно быть пустым");
                }

             //   dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
            }
        }

        private void TextBox1_Validating(object sender, CancelEventArgs e)
        {
            if(string.IsNullOrEmpty(textBox1.Text))
            {
                e.Cancel = true;
           //     errorProvider1.SetError(textBox1, "Наименование не должно быть пустым");
               // MessageBox.Show("Наименование не должно быть пустым");
            }
            else
            {
                errorProvider1.SetError(textBox1, null);
            }

            //throw new NotImplementedException();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //foreach (Control c in panel1.Controls)
            //{
            //    foreach (Binding b in c.DataBindings)
            //    {
            //        b.WriteValue();
            //    }
            //}
            foreach(суда tRow in de.суда.Local)
            {
              //  de.Entry(tRow).State== 
                byte[] версия0 = tRow.версия;
                //if(dic1.ContainsKey(tRow.судно))
                //{
                    byte[] версия1 = de.суда.Single(n=>n.судно==tRow.судно).версия;
                if (версия0 != версия1)
                {
                    MessageBox.Show("Разные");
                }
                //}
            }
         //   de.SaveChanges();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            de.SaveChanges();
            //var list1 = bindingSource1.List;
            //int строк = bindingSource1.List.Count;
            //bindingSource1.EndEdit();
          //  bindingSource1.CancelEdit();
        }
    }
}
