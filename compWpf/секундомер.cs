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
    public partial class секундомер : Form
    {
        public секундомер()
        {
            InitializeComponent();
        }
        DateTime date1;
        private void секундомер_Load(object sender, EventArgs e)
        {
            date1 = DateTime.Today;
            timer1.Tick += Timer1_Tick;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
          //  date1 = date1.AddMilliseconds(1);
            date1 = date1.AddSeconds(1);
            label1.Text = date1.ToString("mm:ss:fff");
            label2.Text = date1.Second.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            date1 = DateTime.Today;
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }
    }
}
