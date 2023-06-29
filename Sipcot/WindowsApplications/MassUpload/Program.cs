using System;
using System.Windows.Forms;

namespace MassUpload
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UserLogin());             

            if (SingleApplication.Run(new UserLogin()) == false)
            {
                MessageBox.Show("Application is already running in system tray.. ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                SingleApplication.Run(new UserLogin());
            }
        }
    }
}
