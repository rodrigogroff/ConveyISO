﻿
namespace ConveyISO
{
    public partial class SocketConvey
    {
        private string montaVendaCEparcelada(ref ISO8583 regIso)
        {
            Util.LOGENTRADA();
            string codLoja = regIso.codLoja;
            string terminal = regIso.terminal;
            string s = terminal.Substring(terminal.Length - 4, 4);
            string str1 = (int.Parse(codLoja.Substring(codLoja.Length - 4, 4)) + int.Parse(s)).ToString("00000000");

            Util.LOGDADOS("Num terminal atribuido: " + str1);

            // original
            //string str2 = "05" + "CE" + "CE" + str1 + (regIso.trilha2.Trim().Length != 27 ? ("999999" + regIso.trilha2.Substring(17, 6) + regIso.trilha2.Substring(23, 6) + regIso.trilha2.Substring(29, 3)).PadLeft(27, '0') : regIso.trilha2.Trim()) + regIso.senha.PadLeft(16, '0') + regIso.valor.PadLeft(12, '0');

            // ajustado
            string str2 = "05CECE1" + codLoja.PadLeft(7, '0') + (regIso.trilha2.Trim().Length != 27 ? ("999999" + regIso.trilha2.Substring(17, 6) + regIso.trilha2.Substring(23, 6) + regIso.trilha2.Substring(29, 3)).PadLeft(27, '0') : regIso.trilha2.Trim()) + regIso.senha.PadLeft(16, '0') + regIso.valor.PadLeft(12, '0');

            if (regIso.bit62.Substring(0, 2) == "00")
                return "";

            int num1 = int.Parse(regIso.valor);
            int num2 = int.Parse(regIso.bit62.Substring(0, 2));
            int num3 = num1 / num2;
            int num4 = num2 * num3;
            int num5 = num1 - num4;
            int num6 = num3 + num5;
            string str3 = num2.ToString("00");
            for (int index = 0; index < num2; ++index)
                str3 = index != 0 ? str3 + num3.ToString("000000000000") : str3 + num6.ToString("000000000000");
            string registro = str2 + str3;

            // ajustado
            registro = registro.PadRight(200, '*') + terminal + regIso.nsuOrigem;

            regIso.bit62 = str3;
            Util.LOGSAIDA(registro);
            return registro;
        }
    }
}
