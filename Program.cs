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
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            
            new frmMain().ShowDialog();
        }
    }
}
