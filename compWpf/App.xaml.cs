using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

namespace compWpf
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs args)
        {
            base.OnStartup(args);

            //if (!InstanceCheck())
            //{
            //    // нажаловаться пользователю и завершить процесс
            //    MessageBox.Show("Уже запущена");
            //    Application.Current.Shutdown();
              
            //}
        }

        // держим в переменной, чтобы сохранить владение им до конца пробега программы
        static Mutex InstanceCheckMutex;
        static bool InstanceCheck()
        {
            bool isNew;
            InstanceCheckMutex = new Mutex(true, "compWpf", out isNew);
            return isNew;
        }


    }
}
