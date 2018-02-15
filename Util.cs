using ConveyISO;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

public class Util
{
    [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileStringA", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern int GetPrivateProfileString(byte[] lpApplicationName, byte[] lpKeyName, byte[] lpDefault, byte[] pReturnedString, int Integer, byte[] lpFileName);

    [DllImport("kernel32", SetLastError = true)]
    private static extern int WritePrivateProfileString(string szSection, string szKey, string szValue, string szFile);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern int GetPrivateProfileSectionNames(byte[] lpszReturnBuffer, int nSize, byte[] lpFileName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern int GetPrivateProfileSection(byte[] lpAppName, byte[] lpReturnedString, int nSize, byte[] lpFileName);

    public static string[] GetChavesSessao(string sSessao, string sArquivo)
    {
        byte[] numArray = new byte[10000];
        Util.GetPrivateProfileSection(new ASCIIEncoding().GetBytes(sSessao), numArray, 10000, new ASCIIEncoding().GetBytes(sArquivo));
        return Encoding.ASCII.GetString(numArray).Trim().Split(new char[1]);
    }

    public static string GetSessoes(string sArquivo)
    {
        byte[] numArray = new byte[10000];
        Util.GetPrivateProfileSectionNames(numArray, 10000, new ASCIIEncoding().GetBytes(sArquivo));
        return Encoding.ASCII.GetString(numArray).Trim().Replace("\0", " ").Trim();
    }

    public static string GetIni(string sSessao, string sChave, string sArquivo, string sExePath)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        if (sExePath == "")
            sExePath = currentDirectory;
        string str = sExePath + "\\" + sArquivo;
        if (!File.Exists(str))
            str = sArquivo;
        byte[] numArray = new byte[10000];
        Util.GetPrivateProfileString(new ASCIIEncoding().GetBytes(sSessao), new ASCIIEncoding().GetBytes(sChave), new ASCIIEncoding().GetBytes(""), numArray, 10000, new ASCIIEncoding().GetBytes(str));
        return Encoding.ASCII.GetString(numArray).Trim().Replace("\n", " ").Replace("\0", " ").Trim();
    }

    public static void SetIni(string sSessao, string sChave, string sValor, string sArquivo, string sExePath)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        if (sExePath == "")
            sExePath = currentDirectory;
        ASCIIEncoding asciiEncoding = new ASCIIEncoding();
        string str = sExePath + "\\" + sArquivo;
        if (!File.Exists(str))
            str = sArquivo;
        Util.WritePrivateProfileString(sSessao, sChave, sValor, str);
    }

    public static string fixaTamanho(string campo, int tamanho, string FILL, bool alignLeft)
    {
        string str = campo;
        if (campo.Length < tamanho)
        {
            if (alignLeft)
            {
                for (int index = 0; index < tamanho - campo.Length; ++index)
                    str = FILL + str;
            }
            else
            {
                for (int index = 0; index < tamanho - campo.Length; ++index)
                    str += FILL;
            }
        }
        if (str.Length > tamanho)
            str = str.Substring(0, tamanho);
        return str;
    }

    public static string inverteData(string data)
    {
        string str1;
        string str2;
        string str3;
        if (data.Length == 10)
        {
            str1 = data.Substring(0, 2);
            str2 = data.Substring(3, 2);
            str3 = data.Substring(6, 4);
        }
        else if (data.Length == 10)
        {
            str1 = data.Substring(0, 2);
            str2 = data.Substring(2, 2);
            str3 = data.Substring(4, 4);
        }
        else
        {
            str1 = "01";
            str2 = "01";
            str3 = "1900";
        }
        return str3 + str2 + str1;
    }

    public static string inverteData(int dia, int mes, int ano)
    {
        string str1 = dia.ToString("00");
        string str2 = mes.ToString("00");
        return ano.ToString("00") + str2 + str1;
    }

    public static string ddmmaaaa(int dia, int mes, int ano)
    {
        return dia.ToString("00") + mes.ToString("00") + ano.ToString("00");
    }

    public static string ddmmaaaa(int dia, int mes, int ano, bool barra)
    {
        string str1 = dia.ToString("00");
        string str2 = mes.ToString("00");
        return ano.ToString("0000") + "/" + str2 + "/" + str1;
    }

    public static int qtdDias(int diaAnt, int mesAnt, int anoAnt)
    {
        int day = DateTime.Now.Day;
        int month = DateTime.Now.Month;
        int year = DateTime.Now.Year;
        return Util.qtdDias(diaAnt, mesAnt, anoAnt, day, month, year);
    }

    public static int qtdDias(int diaAnt, int mesAnt, int anoAnt, int diaAtu, int mesAtu, int anoAtu)
    {
        DateTime dateTime = new DateTime(anoAnt, mesAnt, diaAnt);
        return (new DateTime(anoAtu, mesAtu, diaAtu) - dateTime).Days;
    }

    public static double arredonda(double valor)
    {
        return double.Parse(Decimal.Round(Decimal.Parse(valor.ToString()), 2).ToString());
    }

    public static int arredonda(int valor)
    {
        return int.Parse(Decimal.Round(Decimal.Parse(valor.ToString()), 2).ToString());
    }

    public static void LOGENTRADA()
    {
        Util.LOGALL(0, "", new StackFrame(1, true));
    }

    public static void LOGENTRADA(string registro)
    {
        StackFrame stackFrame = new StackFrame(1, true);
        Util.LOGALL(0, registro, stackFrame);
    }

    public static void LOGSAIDA()
    {
        Util.LOGALL(1, "", new StackFrame(1, true));
    }

    public static void LOGSAIDA(string registro)
    {
        StackFrame stackFrame = new StackFrame(1, true);
        Util.LOGALL(1, registro, stackFrame);
    }

    public static void LOGCHECK()
    {
        Util.LOGALL(2, "", new StackFrame(1, true));
    }

    public static void LOGCHECK(string registro)
    {
        StackFrame stackFrame = new StackFrame(1, true);
        Util.LOGALL(2, registro, stackFrame);
    }

    public static void LOGDADOS(string registro)
    {
        StackFrame stackFrame = new StackFrame(1, true);
        Util.LOGALL(3, registro, stackFrame);
    }

    public static void LOGSQL(string registro)
    {
        StackFrame stackFrame = new StackFrame(1, true);
        Util.LOGALL(5, registro, stackFrame);
    }

    public static void LOGERRO(string registro)
    {
        StackFrame stackFrame = new StackFrame(1, true);
        string str = stackFrame.GetMethod().ToString();
        string fileName = stackFrame.GetFileName();
        int fileLineNumber = stackFrame.GetFileLineNumber();
        int id = Process.GetCurrentProcess().Id;
        int int32 = Process.GetCurrentProcess().Handle.ToInt32();
        try
        {
            GlobalVar.m_Log.WriteLine("# " + fileName + " L(" + (object)fileLineNumber + ") Pid:" + id.ToString("000000") + " hnd:" + int32.ToString("000000") + " " + DateTime.Now.ToString());
            GlobalVar.m_Log.WriteLine("% " + DateTime.Now.ToString() + "[**** ERRO ****]" + str + ":" + registro);
        }
        catch (Exception ex)
        {
        }
    }

    public static void GRAVALOG(string registro)
    {
        if (!GlobalVar.debugApp)
            return;
        GlobalVar.m_Log.WriteLine(registro);
    }

    private static void LOGALL(int tipo, string registro, StackFrame stackFrame)
    {
        if (!GlobalVar.debugApp)
            return;

        GlobalVar.m_Log.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " [" + registro + "]"); 

        /*
        try
        {
            stackFrame.GetMethod().ToString();
            string fileName = stackFrame.GetFileName();
            string name1 = stackFrame.GetMethod().ReflectedType.Name;
            string name2 = stackFrame.GetMethod().Name;
            int fileLineNumber = stackFrame.GetFileLineNumber();
            int id = Process.GetCurrentProcess().Id;
            int int32 = Process.GetCurrentProcess().Handle.ToInt32();
            switch (tipo)
            {
                case 0:
                    string str1 = "ENTRADA";
                    GlobalVar.m_Log.WriteLine(DateTime.Now.ToString() + "# Pid:" + id.ToString("000000") + fileName + ": L(" + (object)fileLineNumber + ") hnd:" + int32.ToString("000000"));
                    GlobalVar.m_Log.WriteLine("-> [" + str1 + "] " + name1 + ":" + name2 + " " + registro);
                    break;
                case 1:
                    string str2 = " SAIDA ";
                    GlobalVar.m_Log.WriteLine(DateTime.Now.ToString() + "# Pid:" + id.ToString("000000") + fileName + ": L(" + (object)fileLineNumber + ") hnd:" + int32.ToString("000000") + " " + name1 + ":" + name2);
                    GlobalVar.m_Log.WriteLine("-> [" + str2 + "] " + name1 + ":" + name2 + " " + registro);
                    break;
                case 2:
                    string str3 = " CHECK ";
                    GlobalVar.m_Log.WriteLine(DateTime.Now.ToString() + "# Pid:" + id.ToString("000000") + fileName + ": L(" + (object)fileLineNumber + ") hnd:" + int32.ToString("000000") + " " + name1 + ":" + name2);
                    GlobalVar.m_Log.WriteLine("-> [" + str3 + "] " + registro);
                    break;
                case 3:
                    string str4 = " DADOS ";
                    GlobalVar.m_Log.WriteLine(DateTime.Now.ToString() + "# Pid:" + id.ToString("000000") + fileName + ": L(" + (object)fileLineNumber + ") hnd:" + int32.ToString("000000") + " " + name1 + ":" + name2);
                    GlobalVar.m_Log.WriteLine("-> [" + str4 + "] " + registro);
                    break;
                case 4:
                    string str5 = " ERRO  ";
                    GlobalVar.m_Log.WriteLine(DateTime.Now.ToString() + "# Pid:" + id.ToString("000000") + fileName + ": L(" + (object)fileLineNumber + ") hnd:" + int32.ToString("000000") + " " + name1 + ":" + name2);
                    GlobalVar.m_Log.WriteLine("-> [" + str5 + "] " + registro);
                    break;
                case 5:
                    string str6 = " SQL   ";
                    GlobalVar.m_Log.WriteLine(DateTime.Now.ToString() + "# Pid:" + id.ToString("000000") + fileName + ": L(" + (object)fileLineNumber + ") hnd:" + int32.ToString("000000") + " " + name1 + ":" + name2);
                    GlobalVar.m_Log.WriteLine("-> [" + str6 + "] " + registro);
                    break;
            }

            GlobalVar.m_Log.Flush();
        }
        catch (Exception ex)
        {
        }
        */
    }

    public static string DESCript(string dados, byte[] chaveC)
    {
        DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
        byte[] bytes = Encoding.ASCII.GetBytes(dados);
        cryptoServiceProvider.Key = chaveC;
        cryptoServiceProvider.Mode = CipherMode.ECB;
        cryptoServiceProvider.CreateEncryptor().TransformBlock(bytes, 0, 8, bytes, 0);
        string str = "";
        for (int index = 0; index < 8; ++index)
            str += string.Format("{0:X2}", (object)bytes[index]);
        return str;
    }

    public static string DESCript(string dados, string chave)
    {
        byte[] bytes1 = Encoding.ASCII.GetBytes(chave);
        byte[] bytes2 = Encoding.ASCII.GetBytes(dados);
        DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
        cryptoServiceProvider.Key = bytes1;
        cryptoServiceProvider.Mode = CipherMode.ECB;
        cryptoServiceProvider.CreateEncryptor().TransformBlock(bytes2, 0, 8, bytes2, 0);
        string str = "";
        for (int index = 0; index < 8; ++index)
            str += string.Format("{0:X2}", (object)bytes2[index]);
        return str;
    }

    public static string DESdeCript(string dados, byte[] key)
    {
        byte[] numArray = new byte[8];
        for (int index = 0; index < dados.Length / 2; ++index)
            numArray[index] = (byte)Convert.ToInt32(dados.Substring(index * 2, 2), 16);
        DES des = (DES)new DESCryptoServiceProvider();
        des.Key = key;
        des.Mode = CipherMode.ECB;
        ICryptoTransform decryptor = des.CreateDecryptor();
        new CryptoStream((Stream)new MemoryStream(), decryptor, CryptoStreamMode.Write).Write(numArray, 0, numArray.Length);
        decryptor.TransformBlock(numArray, 0, 8, numArray, 0);
        return new ASCIIEncoding().GetString(numArray);
    }

    public static string DESdeCript(string dados, string chave)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(chave);
        byte[] numArray = new byte[8];
        for (int index = 0; index < dados.Length / 2; ++index)
            numArray[index] = (byte)Convert.ToInt32(dados.Substring(index * 2, 2), 16);
        DES des = (DES)new DESCryptoServiceProvider();
        des.Key = bytes;
        des.Mode = CipherMode.ECB;
        ICryptoTransform decryptor = des.CreateDecryptor();
        new CryptoStream((Stream)new MemoryStream(), decryptor, CryptoStreamMode.Write).Write(numArray, 0, numArray.Length);
        decryptor.TransformBlock(numArray, 0, 8, numArray, 0);
        return new ASCIIEncoding().GetString(numArray);
    }
}
