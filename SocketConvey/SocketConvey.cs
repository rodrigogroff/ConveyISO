using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConveyISO
{
    public partial class SocketConvey
    {
        Thread mainThread;
                
        public IPAddress localAddr;
        public TcpListener server;

        public int porta = 2000;

        public string data = null,
                      respondido;

        public byte[] bytes = new byte[900000],
                      bytesTamanho = new byte[1000];

        public int port
        {
            get
            {
                return this.porta;
            }
            set
            {
                this.porta = value;
            }
        }

        public void start()
        {
            this.localAddr = IPAddress.Parse("127.0.0.1");
            this.server = new TcpListener(IPAddress.Any, this.porta);
            this.server.Start();

            GlobalVar.frmPrincipal.AtualizaTela("Esperando Conexões");
            mainThread = new Thread(new ThreadStart(this.RecebeRespondeTransacao))
            {
                Name = "ConveySocket"
            };
            mainThread.Start();
        }

        public void RecebeRespondeTransacao()
        {
            Util.LOGDADOS("RecebeRespondeTransacao START");

            while (true)
            {
                using (TcpClient client = server.AcceptTcpClient())
                {
                    try
                    {
                        if (GlobalVar.finalizar)
                            break;

                        string dadosRecebidos = esperaDadosEXPRESS(client);

                        #region - code - 
                        
                        if (dadosRecebidos != null)
                        {
                            if (dadosRecebidos.Length == 0)
                            {
                                Util.LOGCHECK("Encerrada Conexão");
                            }
                            else if (dadosRecebidos.Length <= 20)
                            {
                                GlobalVar.frmPrincipal.AtualizaTela("Registro recebido invalido");
                                Util.LOGCHECK("Registro recebido invalido - muito pequeno");
                                Util.LOGDADOS(dadosRecebidos);
                            }
                            else
                            {
                                Util.LOGDADOS("pacote válido " + dadosRecebidos);

                                if (dadosRecebidos.Substring(0, 4) != "0200" &&
                                    dadosRecebidos.Substring(0, 4) != "0202" &&
                                    dadosRecebidos.Substring(0, 4) != "0400" &&
                                    dadosRecebidos.Substring(0, 4) != "0420")
                                {
                                    GlobalVar.frmPrincipal.AtualizaTela("Registro recebido não formato ISO8583 esperado  - Ignorado");
                                    Util.LOGCHECK("Registro recebido não formato ISO8583 esperado - Ignorado");
                                    Util.LOGDADOS(dadosRecebidos);
                                }
                                else
                                {
                                    ISO8583 regIso = new ISO8583(dadosRecebidos);

                                    logISO(ref regIso);

                                    if (regIso.erro)
                                    {
                                        GlobalVar.frmPrincipal.AtualizaTela("Registro ISO com erro");
                                        Util.LOGCHECK("Registro ISO com erro");
                                    }
                                    else
                                    {
                                        GlobalVar.frmPrincipal.AtualizaTela("Registro ISO Recebido! " + dadosRecebidos.Substring(0, 4));
                                        GlobalVar.frmPrincipal.AtualizaTela("Loja: " + regIso.codLoja.TrimStart('0') + " terminal: " + regIso.terminal);

                                        ISO8583 isoRegistro;

                                        if (dadosRecebidos.Substring(0, 4) == "0200" &&
                                            (regIso.codProcessamento == "002000" ||
                                                regIso.codProcessamento == "002800"))
                                        {
                                            #region - 200 e 202 - 

                                            string registro1 = !(regIso.codProcessamento == "002000") ?
                                                montaVendaCEparcelada(ref regIso) :
                                                montaVendaCE(regIso);

                                            if (registro1 == "")
                                            {
                                                Util.LOGCHECK("Falhas na desmontagem da iso enviada pelo SITEF");
                                            }
                                            else
                                            {
                                                using (Socket s = connectSocketCNET(GlobalVar.SocketIPCE, int.Parse(GlobalVar.SocketPortCE)))
                                                {
                                                    if (s == null)
                                                    {
                                                        Util.LOGCHECK("Conexao com Servidor FALHOU");
                                                    }
                                                    else
                                                    {
                                                        socketEnviaCNET(s, registro1);

                                                        string dadosSocket = socketRecebeCNET(s);

                                                        Util.LOGCHECK("dadosSocket 0200: >" + dadosSocket + "<");

                                                        if (string.IsNullOrEmpty(dadosSocket))
                                                        {
                                                            Util.LOGCHECK("Recebeu ISO vazio");
                                                        }
                                                        else if (dadosSocket.Length <= 20)
                                                        {
                                                            Util.LOGCHECK(" 200 e 202 Recebeu ISO tamanho incorreto");
                                                        }
                                                        else
                                                        {
                                                            dadosSocket = dadosSocket.PadRight(200, ' ');

                                                            isoRegistro = new ISO8583();

                                                            isoRegistro.codResposta = dadosSocket.Substring(2, 2);
                                                            if (regIso.codProcessamento != "002000")
                                                                isoRegistro.bit63 = regIso.bit62;
                                                            isoRegistro.bit127 = "000" + dadosSocket.Substring(7, 6);
                                                            isoRegistro.nsuOrigem = regIso.nsuOrigem;
                                                            isoRegistro.codProcessamento = regIso.codProcessamento;
                                                            isoRegistro.codigo = "0210";
                                                            isoRegistro.valor = regIso.valor;
                                                            isoRegistro.codResposta = dadosSocket.Substring(2, 2);
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

                                                            isoRegistro.bit62 = !(dadosSocket.Substring(2, 2) == "00") ?
                                                                dadosSocket.Substring(73, 20) :
                                                                str5 + str6 + str4.Substring(18, 3) + dadosSocket.Substring(27, 40);

                                                            string registro2 = isoRegistro.registro;

                                                            logISO(ref isoRegistro);
                                                            enviaDadosEXPRESS(registro2, client);

                                                            dadosRecebidos = esperaDadosEXPRESS(client);

                                                            if (dadosRecebidos.Substring(0, 4) == "0202")
                                                            {
                                                                #region - 202 - 

                                                                GlobalVar.frmPrincipal.AtualizaTela("Registro ISO Recebido - Confirmacao ");

                                                                string registro = montaConfirmacaoCE(regIso);

                                                                if (string.IsNullOrEmpty(registro))
                                                                    Util.LOGCHECK("Falha na desmontagem!");
                                                                else
                                                                    socketEnviaCNET(s, registro);

                                                                #endregion
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            #endregion
                                        }
                                        else if (dadosRecebidos.Substring(0, 4) == "0202")
                                        {
                                            #region - 202 - 

                                            GlobalVar.frmPrincipal.AtualizaTela("Registro ISO Recebido - Confirmacao ");

                                            string registro = montaConfirmacaoCE(regIso);

                                            if (string.IsNullOrEmpty(registro))
                                            {
                                                Util.LOGCHECK("Falha na desmontagem!");
                                            }
                                            else
                                            {
                                                using (Socket s = connectSocketCNET(GlobalVar.SocketIPCE, int.Parse(GlobalVar.SocketPortCE)))
                                                {
                                                    if (s == null)
                                                        Util.LOGCHECK("Conexao com Servidor FALHOU");
                                                    else
                                                        socketEnviaCNET(s, registro);
                                                }
                                            }

                                            #endregion
                                        }
                                        else if (dadosRecebidos.Substring(0, 4) == "0400" ||
                                                    dadosRecebidos.Substring(0, 4) == "0420")
                                        {
                                            #region - 400 || 420 - 

                                            string codigoIso, strRegIso;

                                            if (dadosRecebidos.Substring(0, 4) == "0400")
                                            {
                                                codigoIso = "0410";
                                                strRegIso = montaCancelamento(regIso, "012345678901234567890123456");
                                            }
                                            else
                                            {
                                                codigoIso = "0430";
                                                strRegIso = montaDesfazimento(regIso);
                                            }

                                            if (string.IsNullOrEmpty(strRegIso))
                                            {
                                                Util.LOGCHECK("Falha na desmontagem!");
                                            }
                                            else if (strRegIso.Length < 20)
                                            {
                                                Util.LOGCHECK("Falha na desmontagem! 2");
                                            }
                                            else
                                            {
                                                using (Socket s = connectSocketCNET(GlobalVar.SocketIPCE, int.Parse(GlobalVar.SocketPortCE)))
                                                {
                                                    if (s == null)
                                                    {
                                                        Util.LOGCHECK("Conexao com Servidor CNET FALHOU");
                                                    }
                                                    else
                                                    {
                                                        socketEnviaCNET(s, strRegIso);
                                                        string dadosRec400 = socketRecebeCNET(s);

                                                        Util.LOGCHECK("dadosRecebidos 0400: >" + dadosRec400 + "<");

                                                        if (dadosRec400 == "")
                                                        {
                                                            Util.LOGCHECK("Recebeu ISO vazio");
                                                        }
                                                        else if (dadosRec400.Length < 27)
                                                        {
                                                            Util.LOGCHECK("Recebeu ISO tamanho incorreto");
                                                        }
                                                        else
                                                        {
                                                            dadosRec400 = dadosRec400.PadRight(200, ' ');

                                                            isoRegistro = new ISO8583();
                                                            isoRegistro.codResposta = dadosRec400.Substring(2, 2);
                                                            isoRegistro.bit127 = "000" + dadosRec400.Substring(21, 6);
                                                            isoRegistro.nsuOrigem = regIso.nsuOrigem;
                                                            isoRegistro.codProcessamento = regIso.codProcessamento;
                                                            isoRegistro.codigo = codigoIso;
                                                            isoRegistro.codLoja = regIso.codLoja;
                                                            isoRegistro.terminal = regIso.terminal;

                                                            Util.LOGCHECK("Montagem Bit 62");

                                                            isoRegistro.bit62 = !(dadosRec400.Substring(0, 4) == "0400") ? dadosRec400.Substring(7, 6) + regIso.valor : regIso.bit125.Substring(3, 6) + regIso.valor;

                                                            string registro2 = isoRegistro.registro;

                                                            Util.LOGDADOS("RESPOSTA ISO = " + registro2);

                                                            logISO(ref isoRegistro);
                                                            enviaDadosEXPRESS(registro2, client);
                                                        }
                                                    }
                                                }
                                            }

                                            #endregion
                                        }
                                    }
                                }
                            }
                        }

                        #endregion
                    }
                    catch (SocketException ex)
                    {
                        Util.LOGDADOS("RecebeRespondeTransacao *ERR " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Util.LOGDADOS("RecebeRespondeTransacao *ERR " + ex.ToString());
                    }
                }            
            }

            Util.LOGDADOS("*ERR - CAINDO FORA!");            
        }

        public string esperaDadosEXPRESS(TcpClient client)
        {
            string str = (string)null;
            bool flag1 = false;
            
            try
            {
                this.data = (string)null;
                var stream = client.GetStream();

                Util.LOGDADOS("esperaDadosEXPRESS - Waiting for a connection...");

                if (!client.Connected)
                {
                    Util.LOGDADOS("SAINDO (disconectado)");
                    return "";
                }

                for (int t = 0; t < 1000; ++t)
                {
                    if (stream.DataAvailable)
                        return this.leDadosSocketEXPRESS(client);

                    Thread.Sleep(100);
                }
                                    
                Util.LOGDADOS("esperaDadosEXPRESS SAINDO GlobalVar.finalizar");
                return "";
                
            }
            catch (SocketException ex)
            {
                Util.LOGDADOS("esperaDadosEXPRESS *ERR " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Util.LOGDADOS("esperaDadosEXPRESS *ERR " + ex.ToString());
                return null;
            }
        }

        public string leDadosSocketEXPRESS(TcpClient client)
        {
            Util.LOGENTRADA("leDadosSocketEXPRESS");

            var stream = client.GetStream();

            this.data = "";
            try
            {
                Util.LOGCHECK("Vou ler stream do socket");
                stream.Read(this.bytesTamanho, 0, 2);
                int count1 = (int)this.bytesTamanho[0] + (int)this.bytesTamanho[1] * 256;
                int num2 = (int)this.bytesTamanho[0];
                int num3 = (int)this.bytesTamanho[1];
                Thread.Sleep(100);
                int count2 = stream.Read(this.bytes, 0, count1);
                Thread.Sleep(100);
                this.data += Encoding.ASCII.GetString(this.bytes, 0, count2);
                Util.LOGDADOS(" i = " + count2.ToString());
                Util.LOGDADOS(" data = " + this.data);
            }
            catch (SocketException ex)
            {
                Util.LOGERRO("leDadosSocketEXPRESS EXCEPTION = Message : " + ex.Message);
            }
            catch (Exception ex)
            {
                Util.LOGERRO("leDadosSocketEXPRESS = Message : " + ex.ToString());
            }

            string str;
            if (this.data == null)
            {
                GlobalVar.frmPrincipal.AtualizaTela("Nulls recebido - Salvo no Log");
                Util.LOGERRO(" Nulls recebido ");
                str = "ERRO";
            }
            else
            {
                //Console.WriteLine(string.Format("Received: {0}", (object)this.data));
                this.data = this.data.ToUpper();
                str = this.data;
            }
            Util.LOGSAIDA();
            return str;
        }
        
        public void abort()
        {
            server.Stop();
            mainThread.Abort();            
        }

        public void enviaDadosEXPRESS(string Dados, TcpClient client)
        {
            try
            {
                var stream = client.GetStream();

                Util.LOGENTRADA("enviaDadosEXPRESS");
                Util.LOGDADOS("enviaDados >>" + Dados + "<<");

                string str = string.Format("{0:X2}", (object)Dados.Length).PadLeft(4, '0');
                byte[] numArray = new byte[2];
                for (int index = 0; index < str.Length / 2; ++index)
                    numArray[index] = (byte)Convert.ToInt32(str.Substring(index * 2, 2), 16);
                byte[] bytes = Encoding.ASCII.GetBytes("00" + Dados);
                bytes[0] = numArray[1];
                bytes[1] = numArray[0];
                Util.LOGDADOS("tamanho a ser enviado em hexa:" + str.Substring(2) + str.Substring(0, 2));

                stream.Write(bytes, 0, bytes.Length);
            }
            catch (SocketException ex)
            {
                Util.LOGERRO("leDadosSocketEXPRESS EXCEPTION = Message : " + ex.Message);
            }
            catch (Exception ex)
            {
                Util.LOGERRO("leDadosSocketEXPRESS = Message : " + ex.ToString());
            }

            Util.LOGSAIDA();
        }

        private static Socket connectSocketCNET(string server, int port)
        {
            Socket socket1 = null;

            try
            {
                Util.LOGENTRADA("connectSocketCNET");

                foreach (IPAddress address in Dns.Resolve(server).AddressList)
                {
                    IPEndPoint ipEndPoint = new IPEndPoint(address, port);
                    Socket socket2 = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    socket2.Connect((EndPoint)ipEndPoint);
                    if (socket2.Connected)
                    {
                        socket1 = socket2;
                        break;
                    }
                }

                Util.LOGSAIDA();
            }
            catch (SocketException ex)
            {
                Util.LOGERRO("connectSocketCNET *ERR " + ex.Message);
            }
            catch (System.Exception ex)
            {
                Util.LOGERRO("connectSocketCNET *ERR " + ex.ToString());
            }

            return socket1;
        }

        public void socketEnviaCNET(Socket s, string registro)
        {
            try
            {
                Util.LOGENTRADA("socketEnviaCNET");
                Util.LOGDADOS("socketEnvia " + registro);

                byte[] bytes = Encoding.ASCII.GetBytes(registro);

                s.Send(bytes, bytes.Length, SocketFlags.None);

                Util.LOGSAIDA();
            }
            catch (SocketException ex)
            {
                Util.LOGERRO("socketEnviaCNET *ERR " + ex.Message);
            }
            catch (System.Exception ex)
            {
                Util.LOGERRO("socketEnviaCNET *ERR " + ex.ToString());
            }
        }

        public string socketRecebeCNET(Socket s)
        {
            Util.LOGENTRADA("socketRecebeCNET");

            try
            {                
                Encoding ascii = Encoding.ASCII;
                byte[] numArray = new byte[99999];
                int bytes = s.Receive(numArray, numArray.Length, SocketFlags.None);
                string registro = this.ConvertBytetoString(numArray, bytes);
                Util.LOGDADOS("socketRecebe >>" + registro + "<<");
                Util.LOGSAIDA();
                return registro;
            }
            catch (SocketException ex)
            {
                Util.LOGERRO("socketRecebeCNET *ERR " + ex.Message);
            }
            catch (System.Exception ex)
            {
                Util.LOGERRO("socketRecebeCNET *ERR " + ex.ToString());
            }

            return "";
        }

        public string ConvertBytetoString(byte[] recebido, int bytes)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int index = 0; index < recebido.Length; ++index)
            {
                char ch = Convert.ToChar(recebido[index]);
                stringBuilder.Append(string.Format("{0:s}", ch.ToString()));
            }

            return stringBuilder.ToString().Substring(0, bytes);
        }

        public void logISO(ref ISO8583 isoRegistro)
        {
            Util.LOGDADOS(" ISO8583-DETALHES DO REGISTRO \r\n         ======================================================== \r\n         Registro Iso : codigo       =" + isoRegistro.codigo + "\r\n         Bits preenchidos :          =" + isoRegistro.relacaoBits + "\r\n         bit( 3  ) - Codigo Proc.    =" + isoRegistro.codProcessamento + "\r\n         bit( 4  ) - valor           =" + isoRegistro.valor + "\r\n         bit( 7  ) - datahora        =" + isoRegistro.datetime + "\r\n         bit( 11 ) - NSU Origem      =" + isoRegistro.nsuOrigem + "\r\n         bit( 13 ) - data            =" + isoRegistro.Date + "\r\n         bit( 22 ) - modo captura    =" + isoRegistro.bit22 + "\r\n         bit( 35 ) - trilha          =" + isoRegistro.trilha2 + "\r\n         bit( 37 ) - nsu alternativo =" + isoRegistro.nsu + "\r\n         bit( 39 ) - codResposta     =" + isoRegistro.codResposta + "\r\n         bit( 41 ) - terminal        =" + isoRegistro.terminal + "\r\n         bit( 42 ) - codigoLoja      =" + isoRegistro.codLoja + "\r\n         bit( 49 ) - codigo moeda    =" + isoRegistro.bit49 + "\r\n         bit( 52 ) - Senha           =" + isoRegistro.senha + "\r\n         bit( 62 ) - Dados transacao =" + isoRegistro.bit62 + "\r\n         bit( 63 ) - Dados transacao =" + isoRegistro.bit63 + "\r\n         bit( 64 ) - Dados transacao =" + isoRegistro.bit64 + "\r\n         bit( 90 ) - dados original  =" + isoRegistro.bit90 + "\r\n         bit( 125 )- NSU original    =" + isoRegistro.bit125 + "\r\n         bit( 127 )- NSU             =" + isoRegistro.bit127 + "\r\n         ======================================================== \r\n");
        }
    }
}
