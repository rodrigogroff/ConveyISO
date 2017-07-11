
namespace ConveyISO
{
    public partial class SocketConvey
    {
        private string montaConfirmacaoCE(ISO8583 regIso)
        {
            Util.LOGENTRADA();
            string codLoja = regIso.codLoja;
            string terminal = regIso.terminal;
            string s = terminal.Substring(terminal.Length - 4, 4);
            string str1 = (int.Parse(codLoja.Substring(codLoja.Length - 4, 4)) + int.Parse(s)).ToString("00000000");
            Util.LOGDADOS("Num terminal atribuido: " + str1);
            string str2 = "";
            string str3 = regIso.bit127;
            if (str3 == "")
                str3 = "000000000";
            int length = regIso.trilha2.Trim().Length;
            string bit62 = regIso.bit62;
            if (regIso.bit62.Trim().Length == 27)
                str2 = regIso.bit62.Trim();
            else if (regIso.bit62.Trim().Length == 37)
                str2 = ("999999" + regIso.bit62.Substring(17, 6) + regIso.bit62.Substring(23, 6) + regIso.bit62.Substring(29, 3)).PadLeft(27, '0');

            // original
            //string registro = "05" + "CE" + "CC" + str1.PadLeft(8, '0') + str2 + str3.Substring(3) + "00000" + str3;

            // ajustado
            string registro = "05CECC1" + codLoja.PadLeft(7, '0') + str2 + str3.Substring(3) + "00000" + str3;
            registro = registro.PadRight(200, '*') + terminal;

            Util.LOGSAIDA(registro);
            return registro;
        }
    }
}
