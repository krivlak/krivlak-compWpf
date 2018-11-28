using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace compWpf
{
    /// <summary>
    /// Логика взаимодействия для проба.xaml
    /// </summary>
    public partial class проба : Window
    {
         public суда байдарка { get; set; }
       
     //   public суда байдарка;
        public проба()
        {
            InitializeComponent();
            //байдарка = new суда
            //{
            //    наимен = "Беда",
            //    порядок = 1
            //};
            //this.DataContext = байдарка;
            this.DataContext = клСлет.deRow;
        }
        public class суда
        {
            public string наимен { get; set; }
            public int порядок { get; set; }
        }
    }
}
