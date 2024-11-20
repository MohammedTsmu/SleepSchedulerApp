using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SleepSchedulerApp
{
    internal static class Program
    {
        ///// <summary>
        ///// The main entry point for the application.
        ///// </summary>
        //[STAThread]
        //static void Main()
        //{
        //    Application.EnableVisualStyles();
        //    Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new Form1());
        //}

        private static Mutex _mutex = null;

        [STAThread]
        static void Main()
        {
            bool createdNew;
            _mutex = new Mutex(true, "SleepSchedulerAppMutex", out createdNew);

            if (!createdNew)
            {
                // Application is already running.
                MessageBox.Show("The application is already running.");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

    }
}
