
namespace ConveyISO
{
    public partial class SocketConvey
    {
        private string montaCancelamento(ISO8583 regIso, string trilha)
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
                //string registro = "05" + "CE" + "CA" + str.PadLeft(8, '0') + trilha + regIso.bit125.Substring(3) + "00000" + regIso.bit125;

                // ajustado 
                string registro = "05CECA1" + codLoja.TrimStart('0').PadLeft(7, '0') + trilha + regIso.bit125.Substring(3) + "00000" + regIso.bit125;
                registro = registro.PadRight(200, '*') + terminal;

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
