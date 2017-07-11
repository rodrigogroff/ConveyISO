
namespace ConveyISO
{
    public partial class SocketConvey
    {
        private string montaVendaCE(ISO8583 regIso)
        {
            Util.LOGENTRADA();
            string codLoja = regIso.codLoja;
            Util.LOGDADOS("CodEstabelecimento: " + codLoja);
            string terminal = regIso.terminal;
            Util.LOGDADOS("codigo Terminal : " + terminal);
            string s = terminal.Substring(terminal.Length - 4, 4);

            string str = (int.Parse(codLoja.Substring(codLoja.Length - 4, 4)) +
                          int.Parse(s)).ToString("00000000");

            Util.LOGDADOS("Num terminal atribuido: " + str);

            // original
            /*string registro = "05" + "CE" + "CE" + str.PadLeft(8, '0') + 
                      (regIso.trilha2.Trim().Length != 0 ? (regIso.trilha2.Trim().Length != 27 ? 
                      ("999999" + regIso.trilha2.Substring(17, 6) + 
                      regIso.trilha2.Substring(23, 6) + 
                      regIso.trilha2.Substring(29, 3)).PadLeft(27, '0') : 
                      regIso.trilha2.Trim()) : "999999999999999999999999999") + regIso.senha.PadLeft(16, '0') + 
                      regIso.valor.PadLeft(12, '0') + "01" + 
                      regIso.valor.PadLeft(12, '0');
                      */

            // ajustado
            string registro = "05CECE1" + codLoja.PadLeft(7, '0') +
                    (regIso.trilha2.Trim().Length != 0 ? (regIso.trilha2.Trim().Length != 27 ?
                    ("999999" + regIso.trilha2.Substring(17, 6) +
                    regIso.trilha2.Substring(23, 6) +
                    regIso.trilha2.Substring(29, 3)).PadLeft(27, '0') :
                    regIso.trilha2.Trim()) : "999999999999999999999999999") + regIso.senha.PadLeft(16, '0') +
                    regIso.valor.PadLeft(12, '0') + "01" +
                    regIso.valor.PadLeft(12, '0');

            registro = registro.PadRight(200, '*') + terminal + regIso.nsuOrigem;

            Util.LOGSAIDA(registro);
            return registro;
        }
    }
}
