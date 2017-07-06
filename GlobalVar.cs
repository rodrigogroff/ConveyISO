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
    public static FileStream m_log_file = (FileStream) null;
    public static StreamWriter m_Log = (StreamWriter) null;
    public static string nomelog = "";
    public static bool debugApp = false;
    public static string BDbanco = "";
    public static string BDAutorizador = "";
    public static string SocketPort = "2000";
    public static frmMain frmPrincipal = (frmMain) null;
    public static bool finalizar = false;
    public static bool testes = false;
    public static string SocketIPCE = "100.0.0.0";
    public static string SocketPortCE = "1000";
    public static string tipoRoteamento = "Star";
    public static int numThreads = 0;
  }
}
