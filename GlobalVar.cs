// Decompiled with JetBrains decompiler
// Type: ConveyISO.GlobalVar
// Assembly: ConveyISO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A1A29DB8-D4AD-4F47-B8FA-3FADEED7E861
// Assembly location: C:\Users\rodrigo.groff\Desktop\ciso\ConveyISO.exe

using System.IO;

namespace ConveyISO
{
    internal class GlobalVar
    {
        public static frmMain frmPrincipal = null;
        public static FileStream m_log_file = null;
        public static StreamWriter m_Log = null;

        public static bool  debugApp = false,
                            finalizar = false,
                            testes = false;

        public static string nomelog = "",
                             BDbanco = "",
                             BDAutorizador = "",
                             SocketPort = "2000",
                             SocketIPCE = "100.0.0.0",
                             SocketPortCE = "1000",
                             tipoRoteamento = "Star";

        public static int numThreads = 0;
    }
}
