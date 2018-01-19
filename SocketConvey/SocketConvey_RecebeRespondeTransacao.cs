using System;
using System.Net.Sockets;
using System.Threading;

namespace ConveyISO
{
    public partial class SocketConvey
    {
        public void RecebeRespondeTransacao()
        {
            Util.LOGENTRADA();
            Util.LOGCHECK("Esperando Conexões....");
            ++GlobalVar.numThreads;
            TcpClient client = new TcpClient();
            NetworkStream stream = (NetworkStream)null;
            this.esperaConectar(ref client, ref stream);
            if (!GlobalVar.finalizar)
            {
                this.workThread2[GlobalVar.numThreads] = new Thread(new ThreadStart(this.RecebeRespondeTransacao));
                this.workThread2[GlobalVar.numThreads].Start();
            }
            bool flag = true;
            do
            {
                if (GlobalVar.finalizar)
                    flag = false;
                string str1 = this.esperaDados(ref client, ref stream);
                if (!GlobalVar.finalizar)
                {
                    if (str1 != null)
                    {
                        if (str1.StartsWith("?"))
                            str1 = str1.Substring(2);

                        if (str1.Length == 0)
                            Util.LOGCHECK("Encerrada Conexão");
                        else if (str1.Length < 20)
                        {
                            GlobalVar.frmPrincipal.AtualizaTela("Registro recebido invalido");
                            Util.LOGCHECK("Registro recebido invalido - muito pequeno");
                            Util.LOGDADOS(str1);
                        }
                        else if (str1.Substring(0, 4) != "0200" && str1.Substring(0, 4) != "0202" && str1.Substring(0, 4) != "0400" && str1.Substring(0, 4) != "0420")
                        {
                            GlobalVar.frmPrincipal.AtualizaTela("Registro recebido não formato ISO8583 esperado  - Ignorado");
                            Util.LOGCHECK("Registro recebido não formato ISO8583 esperado - Ignorado");
                            Util.LOGDADOS(str1);
                        }
                        else
                        {
                            ISO8583 regIso = new ISO8583(str1);
                            this.logISO(ref regIso);
                            if (regIso.erro)
                            {
                                GlobalVar.frmPrincipal.AtualizaTela("Registro ISO com erro");
                                Util.LOGCHECK("Registro ISO com erro");
                            }
                            else
                            {
                                frmMain frmPrincipal = GlobalVar.frmPrincipal;
                                object[] objArray1 = new object[6];
                                objArray1[0] = (object)"Registro ISO Recebido: ";
                                object[] objArray2 = objArray1;
                                int index1 = 1;
                                DateTime now = DateTime.Now;
                                // ISSUE: variable of a boxed type
                                var hour = (ValueType)now.Hour;
                                objArray2[index1] = (object)hour;
                                objArray1[2] = (object)":";
                                object[] objArray3 = objArray1;
                                int index2 = 3;
                                now = DateTime.Now;
                                // ISSUE: variable of a boxed type
                                var minute = (ValueType)now.Minute;
                                objArray3[index2] = (object)minute;
                                objArray1[4] = (object)":";
                                object[] objArray4 = objArray1;
                                int index3 = 5;
                                now = DateTime.Now;
                                // ISSUE: variable of a boxed type
                                var second = (ValueType)now.Second;
                                objArray4[index3] = (object)second;
                                string t = string.Concat(objArray1);
                                frmPrincipal.AtualizaTela(t);
                                GlobalVar.frmPrincipal.AtualizaTela("Empresa: " + regIso.codLoja + " terminal: " + regIso.terminal + " tipo: " + str1.Substring(0, 4));
                                string str2;
                                ISO8583 isoRegistro;
                                if (str1.Substring(0, 4) == "0200" && (regIso.codProcessamento == "002000" || regIso.codProcessamento == "002800"))
                                {
                                    #region - 0200 - 

                                    str2 = "";
                                    string registro1 = !(regIso.codProcessamento == "002000") ? this.montaVendaCEparcelada(ref regIso) : this.montaVendaCE(regIso);
                                    Socket s = SocketConvey.connectSocket(GlobalVar.SocketIPCE, int.Parse(GlobalVar.SocketPortCE));
                                    if (s == null)
                                    {
                                        Util.LOGCHECK("Conexao com Servidor FALHOU");
                                        Util.LOGSAIDA();
                                        return;
                                    }
                                    this.socketEnvia(s, registro1);
                                    string str3 = this.socketRecebe(s);
                                    isoRegistro = new ISO8583();
                                    isoRegistro.codResposta = str3.Substring(2, 2);
                                    if (regIso.codProcessamento != "002000")
                                        isoRegistro.bit63 = regIso.bit62;
                                    isoRegistro.bit127 = "000" + str3.Substring(7, 6);
                                    isoRegistro.nsuOrigem = regIso.nsuOrigem;
                                    isoRegistro.codProcessamento = regIso.codProcessamento;
                                    isoRegistro.codigo = "0210";
                                    isoRegistro.valor = regIso.valor;
                                    isoRegistro.codResposta = str3.Substring(2, 2);
                                    isoRegistro.terminal = regIso.terminal;
                                    isoRegistro.codLoja = regIso.codLoja;
                                    string str4;
                                    string str5;
                                    string str6;
                                    if (regIso.trilha2.Trim().Length == 0)
                                    {
                                        str4 = "999999999999999999999999999";
                                        str5 = "999999";
                                        str6 = "999999";
                                    }
                                    else if (regIso.trilha2.Trim().Length == 27)
                                    {
                                        str4 = regIso.trilha2.Trim();
                                        str5 = regIso.trilha2.Trim().Substring(6, 6);
                                        str6 = regIso.trilha2.Trim().Substring(12, 6);
                                    }
                                    else
                                    {
                                        str5 = regIso.trilha2.Substring(17, 6);
                                        str6 = regIso.trilha2.Substring(23, 6);
                                        str4 = ("999999" + str5 + str6 + regIso.trilha2.Substring(29, 3)).PadLeft(27, '0');
                                    }
                                    isoRegistro.bit62 = !(str3.Substring(2, 2) == "00") ? str3.Substring(73, 20) : str5 + str6 + str4.Substring(18, 3) + str3.Substring(27, 40);
                                    string registro2 = isoRegistro.registro;
                                    this.logISO(ref isoRegistro);
                                    this.enviaDados(registro2, stream);

                                    #endregion
                                }
                                else if (str1.Substring(0, 4) == "0202")
                                {
                                    #region - 0202 - 

                                    GlobalVar.frmPrincipal.AtualizaTela("Registro ISO Recebido - Confirmacao ");
                                    string registro = this.montaConfirmacaoCE(regIso);
                                    Socket s = SocketConvey.connectSocket(GlobalVar.SocketIPCE, int.Parse(GlobalVar.SocketPortCE));
                                    if (s == null)
                                    {
                                        Util.LOGCHECK("Conexao com Servidor FALHOU");
                                        Util.LOGSAIDA();
                                    }
                                    else
                                        this.socketEnvia(s, registro);

                                    #endregion
                                }
                                else if (str1.Substring(0, 4) == "0400" || str1.Substring(0, 4) == "0420")
                                {
                                    #region - 0400 - 

                                    str2 = "";
                                    string str3;
                                    string registro1;
                                    if (str1.Substring(0, 4) == "0400")
                                    {
                                        str3 = "0410";
                                        registro1 = this.montaCancelamento(regIso, "012345678901234567890123456");
                                    }
                                    else
                                    {
                                        str3 = "0430";
                                        registro1 = this.montaDesfazimento(regIso);
                                    }
                                    Socket s = SocketConvey.connectSocket(GlobalVar.SocketIPCE, int.Parse(GlobalVar.SocketPortCE));
                                    if (s == null)
                                    {
                                        Util.LOGCHECK("Conexao com Servidor FALHOU");
                                        Util.LOGSAIDA();
                                        return;
                                    }
                                    this.socketEnvia(s, registro1);
                                    string str4 = this.socketRecebe(s);
                                    isoRegistro = new ISO8583();
                                    isoRegistro.codResposta = str4.Substring(2, 2);
                                    isoRegistro.bit127 = "000" + str4.Substring(21, 6);
                                    isoRegistro.nsuOrigem = regIso.nsuOrigem;
                                    isoRegistro.codProcessamento = regIso.codProcessamento;
                                    isoRegistro.codigo = str3;
                                    isoRegistro.codLoja = regIso.codLoja;
                                    isoRegistro.terminal = regIso.terminal;
                                    isoRegistro.bit62 = !(str1.Substring(0, 4) == "0400") ? str4.Substring(7, 6) + regIso.valor : regIso.bit125.Substring(3, 6) + regIso.valor;
                                    string registro2 = isoRegistro.registro;
                                    Util.LOGDADOS("RESPOSTA ISO = " + registro2);
                                    this.logISO(ref isoRegistro);
                                    this.enviaDados(registro2, stream);

                                    #endregion
                                }
                            }
                        }
                    }
                }
                else
                    goto label_6;
            }
            while (flag);
            goto label_40;
            label_6:
            return;
            label_40:
            this.closeCliente(client);
            Util.LOGSAIDA();
        }
    }
}
