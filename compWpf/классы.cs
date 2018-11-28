using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Controls;

namespace compWpf
{
    //class клБаза
    //{
    //    public static compWpf.Entities de = new Entities();
    //}
    class клРезультат
    {
        public static Guid результат = Guid.Empty;
        public static результаты deRow = null;
     //   public static string наимен = "";
        public static bool выбран = false;
        public int штраф = 0;
        public static List<Form> formList = new List<Form>();
    }
    class клКалендарь
    {
        public static DateTime? дата = DateTime.Today;
        public static bool isNull = false;
        public static bool выбран = false;
    }

    class клСлет
    {
        public static Guid слет  = Guid.Empty;
        public static слеты deRow  = null;
        public static string наимен  = "";
        public static bool выбран  = false;
      
    }

    class клШкола
    {
        public static Guid школа = Guid.Empty;
        public static школы deRow = null;
        public static string наимен = "";
        public static bool выбран = false;
    }

    class клСудно
    {
        public static Guid судно = Guid.Empty;
        public static суда deRow = null;
        public static string наимен = "";
        public static bool выбран = false;
    }

    class клВид
    {
        public static Guid вид = Guid.Empty;
        public static виды deRow = null;
        public static string наимен = "";
        public static bool выбран = false;
    }

    class клДистанция
    {
        public static Guid вид = Guid.Empty;
        public static Guid дистанция = Guid.Empty;
        public static дистанции deRow = null;
        public static string наимен = "";
        public static bool выбран = false;
     //   public static List<string> открытыЛист;
        public static List<Form> formList ;
      //  public static Form окно_выбора;
    }

    class клТурист
    {
        public static Guid турист = Guid.Empty;
        public static Guid школа = Guid.Empty;
        public static туристы deRow = null;
        public static string фамилия = "";
        public static bool выбран = false;
        public static List<туристы> turList = null;
    }

    class клЗапуск
    {
        public static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);

            //Проходимся по всем процессам тем-же именем, что и у нашего процесса
            foreach (Process process in processes)
            {
                // если идентификатор процесса не равен нашему...
                if (process.Id != current.Id)
                {
                    //Сверяем имя исполняемого файла запущенного процесса и нашего процесса
                    if (Assembly.GetExecutingAssembly().Location.
                    Replace("/", "\\") == current.MainModule.FileName)
                    {
                        //Возвращаем ссылку на процесс
                        return process;

                    }
                }
            }

            return null;
        }
    }

    public class DataGridNumericColumn : DataGridTextColumn
    {
        protected override object PrepareCellForEdit(System.Windows.FrameworkElement editingElement, System.Windows.RoutedEventArgs editingEventArgs)
        {
            System.Windows.Controls.TextBox edit = editingElement as System.Windows.Controls.TextBox;
            edit.PreviewTextInput += OnPreviewTextInput;

            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            try
            {
                Convert.ToInt32(e.Text);
            }
            catch
            {
                // Show some kind of error message if you want

                // Set handled to true
                e.Handled = true;
            }
        }
    }
    public class DataGridString50Column : DataGridTextColumn
    {
        protected override object PrepareCellForEdit(System.Windows.FrameworkElement editingElement, System.Windows.RoutedEventArgs editingEventArgs)
        {
            System.Windows.Controls.TextBox edit = editingElement as System.Windows.Controls.TextBox;
            edit.PreviewTextInput += OnPreviewTextInput2;

            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        void OnPreviewTextInput2(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            System.Windows.Controls.TextBox edit1 = sender as System.Windows.Controls.TextBox;
            if (edit1.Text.Length>49)
            {
                e.Handled = true;
            }
            else
            {
              //  throw new NotImplementedException();
            }
           
        }
    }

    class клKey
    {
        public static void int_KeyPress(object senderT, KeyPressEventArgs eT)
        {

            if (!Char.IsDigit(eT.KeyChar) && !Char.IsControl(eT.KeyChar))
            {
                eT.Handled = true;
                System.Media.SystemSounds.Beep.Play();
            }

        }


        public static void intсоЗнаком(object senderT, KeyPressEventArgs eT)
        {

            if (!Char.IsDigit(eT.KeyChar) && !Char.IsControl(eT.KeyChar) && eT.KeyChar != '-')
            {
                eT.Handled = true;
                System.Media.SystemSounds.Beep.Play();
            }

        }


        public static void decimal_KeyPress(object senderT, KeyPressEventArgs eT)
        {
            char сепаратор = (char)System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToString()[0];

            char[] aChar = { ',', '.', '/', 'б', 'ю', '.', '?' };
            if (aChar.Contains(eT.KeyChar))
            {
                eT.KeyChar = сепаратор;
            }

            if (!Char.IsDigit(eT.KeyChar) && !Char.IsControl(eT.KeyChar))
            {
                System.Windows.Forms.TextBox tb = (System.Windows.Forms.TextBox)senderT;
                if (eT.KeyChar != сепаратор || tb.Text.IndexOf(сепаратор) != -1)
                {
                    eT.Handled = true;
                    System.Media.SystemSounds.Beep.Play();
                }
            }
        }

        public static void decimalсоЗнаком(object senderT, KeyPressEventArgs eT)
        {
            char сепаратор = (char)System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToString()[0];

            char[] aChar = { ',', '.', '/', 'б', 'ю', '.', '?' };
            if (aChar.Contains(eT.KeyChar))
            {
                eT.KeyChar = сепаратор;
            }

            if (!Char.IsDigit(eT.KeyChar) && !Char.IsControl(eT.KeyChar) && eT.KeyChar != '-')
            {
                System.Windows.Forms.TextBox tb = (System.Windows.Forms.TextBox)senderT;
                if (eT.KeyChar != сепаратор || tb.Text.IndexOf(сепаратор) != -1)
                {
                    eT.Handled = true;
                    System.Media.SystemSounds.Beep.Play();
                }
            }
        }


    }

}
