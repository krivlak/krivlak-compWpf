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
    /// Логика взаимодействия для календарьWPF.xaml
    /// </summary>
    public partial class календарьWPF : Window
    {
        public календарьWPF()
        {
            InitializeComponent();
        }
        System.Windows.Threading.DispatcherTimer timer1 = new System.Windows.Threading.DispatcherTimer();
        DateTime date1 = DateTime.Today;
        bool отчет = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer1.Interval = new System.TimeSpan(0, 0, 1);
            timer1.Tick += Timer1_Tick;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            date1 = date1.AddSeconds(1);
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            if (отчет)
            {

            }
            else
            {
                start.Name = "Финиш";
                date1 = DateTime.Today;
                timer1.Start();

            }
        }
    }
}
