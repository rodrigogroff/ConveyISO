using System;
using System.IO;
using System.Text;
using System.Threading;
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

            while (true)
            {
                try
                {
                    new frmMain().ShowDialog();
                }
                catch (SystemException ex)
                {
                    ex.ToString();

                    using (var x = new StreamWriter(DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.Default))
                    {
                        x.WriteLine(ex.ToString());
                    }

                    Thread.Sleep(1000);
                }
            }            
        }
    }
}
