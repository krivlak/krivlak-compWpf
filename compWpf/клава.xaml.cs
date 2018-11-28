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
    /// Логика взаимодействия для клава.xaml
    /// </summary>
    public partial class клава : Window
    {
        public клава()
        {
            InitializeComponent();
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            textBlock1.Text += e.Key.ToString();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            label1.Content = "изм";
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int val;
            if (!Int32.TryParse(e.Text, out val) && e.Text != "-")
            {
                e.Handled = true; // отклоняем ввод
            }
        }
    }
}
