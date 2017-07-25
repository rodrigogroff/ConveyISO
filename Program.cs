using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ConveyISO
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);

            while (true)
            {                
                Application.Run(new frmMain());

                StreamWriter sw = new StreamWriter("Re-Start" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".txt", false, Encoding.Default);
                sw.WriteLine("Exited!");
                sw.Close();
            }            
        }
    }
}
