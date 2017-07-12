
namespace ConveyISO
{
    public partial class SocketConvey
    {
        private string montaDesfazimento(ISO8583 regIso)
        {
            try
            {
                Util.LOGENTRADA();
                string codLoja = regIso.codLoja;
                string terminal = regIso.terminal;
                string s = terminal.Substring(terminal.Length - 4, 4);
                string str = (int.Parse(codLoja.Substring(codLoja.Length - 4, 4)) + int.Parse(s)).ToString("00000000");
                Util.LOGDADOS("Num terminal atribuido: " + str);

                // original
                //string registro = "05" + "CE" + "DF" + str.PadLeft(8, '0') + "0".PadLeft(43, '0') + regIso.valor.PadLeft(12, '0');

                // ajustado
                string registro = "05CEDF1" + codLoja.TrimStart('0').PadLeft(7, '0') + "0".PadLeft(43, '0') + regIso.valor.PadLeft(12, '0');
                registro = registro.PadRight(200, '*') + str + regIso.nsuOrigem.TrimStart('0').PadLeft(8, '0');

                Util.LOGSAIDA(registro);
                return registro;
            }
            catch (System.Exception ex)
            {
                Util.LOGSAIDA(ex.ToString());
                return "";
            }
        }
    }
}
