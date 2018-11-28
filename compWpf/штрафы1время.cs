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
    public partial class штрафы1время : Form
    {
        public штрафы1время(Guid кодД)
        {
            InitializeComponent();
            кодДистанции = кодД;

        }


        compWpf.Entities de = new Entities();
        List<temp> tempList = new List<temp>();
        результаты оРезультаты;
        Guid кодДистанции ;
        Guid кодРезультата  ;
        int ВсегоШтраф = 0;
        DateTime старт = DateTime.Now;
        DateTime финиш = DateTime.Now;
        int минут = 0;
        int секунд = 0;
        int  миллисекунд = 0;
        int в_секундах = 0;
        int итог = 0;

        private void штрафы1время_Load(object sender, EventArgs e)
        {

            оРезультаты = (результаты)this.Tag;
            кодРезультата = оРезультаты.результат;

            try
            {
                
                de.этапы.Where(n => n.дистанция == кодДистанции).OrderBy(n => n.порядок).Load();
                de.штрафы.Where(n => n.результат == кодРезультата).OrderBy(n => n.этапы.порядок).Load();
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
            timer1.Start();
            timer1.Tick += Timer1_Tick;
            bindingSource1.ListChanged += BindingSource1_ListChanged;
            FormClosed += Штрафы1время_FormClosed;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                финиш = DateTime.Now;
                var rtr = (финиш - старт);
                минут = rtr.Minutes;
                секунд = rtr.Seconds;
                миллисекунд = rtr.Milliseconds;
                минутыBox.Text = минут.ToString("0;#;#");
                секундыBox.Text = секунд.ToString("0;#;#");
                миллиBox.Text = миллисекунд.ToString("0;#;#");
            }
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

                    оРезультаты.время_мин = минут;
                    оРезультаты.время_сек = секунд;
                    оРезультаты.штраф = ВсегоШтраф;
                    оРезультаты.итог = итог;

                   
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

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        class temp
        {
            public Guid этап { get; set; }
            public string наимен { get; set; }
            public int секунд { get; set; }
            public string прим { get; set; }
            public int накопление { get; set; }
        }
        void пересчет()
        {
            int ss = 0;
            foreach(temp tRow in tempList)
            {
                ss += tRow.секунд;
                tRow.накопление = ss;
            }
            ВсегоШтраф = ss;
            dataGridView1.Refresh();
            в_секундах = минут * 60 + секунд;
            итог = в_секундах + ВсегоШтраф;
            textBox1.Text = в_секундах.ToString("0;#;#");
            textBox2.Text = ВсегоШтраф.ToString("0;#;#");
            textBox3.Text = итог.ToString("0;#;#");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                checkBox1.Text = "Финиш";
                секунд = 0;
                минут = 0;
                старт = DateTime.Now;

            }
            else
            {
               
                checkBox1.Text = "Старт";
                пересчет();
            }
        }
    }
}
