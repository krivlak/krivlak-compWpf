using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace compWpf
{
    public partial class выборVдистанции : Form
    {
        public выборVдистанции()
        {
            InitializeComponent();
            this.Top = 0;
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            this.Left = 0;
        }
        Entities de = new Entities();
     //   List<Form> formList = new List<Form>();
        private void выборVдистанции_Load(object sender, EventArgs e)
        {
            try
            {
             //   клДистанция.formList = new List<Form>();

                init_слет();
                this.Text = " Выберите дистанции на " + клСлет.наимен;
                foreach (var gg in de.виды.OrderBy(n => n.порядок))
                {
                    TreeNode node = this.treeView1.Nodes.Add(gg.наимен.ToString());
                    node.Tag = gg;

                    foreach (var mm in de.дистанции.Where(n => n.вид == gg.вид).Where(n => n.слет == клСлет.слет).OrderBy(n => n.порядок))
                    {

                   //     int экипажей = mm.экипажи.Count();
                        TreeNode node1 = node.Nodes.Add(mm.наимен
                            //+ "   экипажей " + экипажей.ToString()
                            );
                        node1.Tag = mm;
                        if (mm.дистанция == клДистанция.дистанция)
                        {
                            treeView1.SelectedNode = node1;
                        }

                    }

                }

      //          клДистанция.окно_выбора = this;
                //плавающее окно = new плавающее();
                
                //клДистанция.formList.Add(окно);
                //окно.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сбой загрузки " + ex.Message);
            }
            клДистанция.formList = new List<Form>();
      //      this.Activated += ВыборVдистанции_Activated;
        }

        private void ВыборVдистанции_Activated(object sender, EventArgs e)
        {
            Console.WriteLine("Астшм");
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

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            button1.Enabled = false;
            if (e.Node.Level == 1)
            {
                клДистанция.deRow = (дистанции)e.Node.Tag;
                клДистанция.дистанция = клДистанция.deRow.дистанция;
                клДистанция.наимен = клДистанция.deRow.наимен;
              

                button1.Enabled = true;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //  клДистанция.открытыЛист.Add(клДистанция.дистанция);
            bool xy = true;
            Form форма= null;
            foreach (Form fr in клДистанция.formList)
            {
                Guid  код = (Guid)fr.Tag;
                if (код == клДистанция.дистанция)
                {
                    xy = false;
                    форма = fr;
                }
            }
            if (xy)
            {

                Cursor = Cursors.WaitCursor;
                окно1результатов списокСлетов = new окно1результатов(this);
                списокСлетов.Text = клДистанция.наимен.Trim();
                списокСлетов.Tag = клДистанция.дистанция;
                списокСлетов.Show();
                клДистанция.formList.Add(списокСлетов);
                Cursor = Cursors.Default;
            }
            else
            {
               // MessageBox.Show("Уже открыта ..");
                if (форма != null)
                {
                    форма.Activate();
                }
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

            Form[] arr = клДистанция.formList.ToArray();
            foreach(Form fr in arr)
            {
                fr.Close();
            }

            //do
            //{
            //    MdiChildren[0].Close();
            //} while (MdiChildren.Count() > 0);


            Close();
        }
    }
}
