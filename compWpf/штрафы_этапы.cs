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
    public partial class штрафы_этапы : Form
    {
        public штрафы_этапы()
        {
            InitializeComponent();
         //   кодДистанции = кодД;
        }

        результаты оРезультаты;
      //  Guid кодДистанции;
        Guid кодРезультата;
        int ВсегоШтраф = 0;
        compWpf.Entities de = new Entities();
        List<temp> tempList = new List<temp>();
        private void штрафы_этапы_Load(object sender, EventArgs e)
        {
            оРезультаты = (результаты)this.Tag;
            кодРезультата = оРезультаты.результат;
            //кодДистанции = оРезультаты.экипажи.дистанция;
            try
            {

                de.этапы.Where(n => n.дистанция == клДистанция.дистанция).OrderBy(n => n.порядок).Load();
                de.штрафы.Where(n => n.результат == кодРезультата).Load();
                //    bindingSource1.DataSource = de.штрафы.Local.ToBindingList();
                foreach (этапы eRow in de.этапы.Local)
                {
                    temp newTemp = new temp()
                    {
                        наимен = eRow.наимен,
                        прим = "",
                        секунд = 0,
                        этап = eRow.этап
                    };
                    tempList.Add(newTemp);
                }
                Dictionary<Guid, temp> tempDic = tempList.ToDictionary(n => n.этап);
                foreach (штрафы sRow in de.штрафы.Local)
                {
                    if (tempDic.ContainsKey(sRow.этап))
                    {
                        tempDic[sRow.этап].секунд = sRow.секунд;
                        tempDic[sRow.этап].прим = sRow.прим;
                    }
                }
                пересчет();
                bindingSource1.DataSource = tempList;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки " + ex.Message);
            }
          
         
            bindingSource1.ListChanged += BindingSource1_ListChanged;
            FormClosed += Штрафы1время_FormClosed;
        }

        private void Штрафы1время_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (label1.Visible)
            {
                try
                {
                    de.штрафы.Local.Clear();
                    foreach (temp tRow in tempList.Where(n => n.секунд > 0))
                    {

                        штрафы newRow = new штрафы()
                        {
                            прим = tRow.прим,
                            результат = кодРезультата,
                            секунд = tRow.секунд,
                            штраф = Guid.NewGuid(),
                            этап = tRow.этап
                        };
                        de.штрафы.Local.Add(newRow);
                    }
                    de.SaveChanges();

                  
                    оРезультаты.штраф = ВсегоШтраф;


                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cбой записи " + ex.Message);
                }
            }
            клРезультат.formList.Remove(this);

        }

        private void BindingSource1_ListChanged(object sender, ListChangedEventArgs e)
        {
            label1.Visible = true;
            пересчет();
        }

        void пересчет()
        {
            int ss = 0;
            foreach (temp tRow in tempList)
            {
                ss += tRow.секунд;
                tRow.накопление = ss;
            }
            ВсегоШтраф = ss;
            dataGridView1.Refresh();
           
        }

        class temp
        {
            public Guid этап { get; set; }
            public string наимен { get; set; }
            public int секунд { get; set; }
            public string прим { get; set; }
            public int накопление { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
