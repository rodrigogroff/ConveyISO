using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConveyISO
{
    public partial class SocketConvey
    {
        private Thread[] arrayThread = new Thread[100000];
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
            this.arrayThread[GlobalVar.numThreads] = new Thread(new ThreadStart(this.RecebeRespondeTransacao))
            {
                Name = "ConveySocket"
            };
            this.arrayThread[GlobalVar.numThreads].Start();
        }

        public void RecebeRespondeTransacao()
        {
            try
            {
                Util.LOGENTRADA();

                Util.LOGCHECK("Esperando Conexões.... >> " + GlobalVar.numThreads + "<<");
                ++GlobalVar.numThreads;

                if (GlobalVar.numThreads == 100000)
                    GlobalVar.numThreads = 0;

                {
                    NetworkStream stream = (NetworkStream)null;

                    TcpClient client = this.server.AcceptTcpClient();

                    if (!GlobalVar.finalizar)
                    {
                        this.arrayThread[GlobalVar.numThreads] = new Thread(new ThreadStart(this.RecebeRespondeTransacao));
                        this.arrayThread[GlobalVar.numThreads].Start();
                    }

                    bool flag = true;
                    do
                    {
                        if (GlobalVar.finalizar)
                            flag = false;

                        string dadosRecebidos = esperaDados(ref client, ref stream);

                        if (!GlobalVar.finalizar)
                        {
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
                                        this.logISO(ref regIso);
                                        if (regIso.erro)
                                        {
                                            GlobalVar.frmPrincipal.AtualizaTela("Registro ISO com erro");
                                            Util.LOGCHECK("Registro ISO com erro");
                                        }
                                        else
                                        {
                                            frmMain frmPrincipal = GlobalVar.frmPrincipal;

                                            frmPrincipal.AtualizaTela("Registro ISO Recebido! ");

                                            GlobalVar.frmPrincipal.AtualizaTela("Empresa: " + regIso.codLoja + " terminal: " + regIso.terminal + " tipo: " + dadosRecebidos.Substring(0, 4));

                                            ISO8583 isoRegistro;

                                            if (dadosRecebidos.Substring(0, 4) == "0200" &&
                                                (regIso.codProcessamento == "002000" ||
                                                 regIso.codProcessamento == "002800"))
                                            {
                                                string registro1 = !(regIso.codProcessamento == "002000") ?
                                                    this.montaVendaCEparcelada(ref regIso) : this.montaVendaCE(regIso);

                                                if (registro1 == "")
                                                {
                                                    Util.LOGCHECK("Falhas na desmontagem da iso enviada pelo SITEF");
                                                    Util.LOGSAIDA();
                                                }
                                                else
                                                {
                                                    Socket s = SocketConvey.connectSocket(GlobalVar.SocketIPCE, int.Parse(GlobalVar.SocketPortCE));

                                                    if (s == null)
                                                    {
                                                        Util.LOGCHECK("Conexao com Servidor FALHOU");
                                                        Util.LOGSAIDA();
                                                        return;
                                                    }

                                                    this.socketEnvia(s, registro1);

                                                    string dadosSocket = this.socketRecebe(s);

                                                    Util.LOGCHECK("dadosSocket 0200: >" + dadosSocket + "<");

                                                    if (string.IsNullOrEmpty(dadosSocket))
                                                    {
                                                        Util.LOGCHECK("Recebeu ISO vazio");
                                                        Util.LOGSAIDA();
                                                    }
                                                    else if (dadosSocket.Length <= 20)
                                                    {
                                                        Util.LOGCHECK("Recebeu ISO tamanho incorreto");
                                                        Util.LOGSAIDA();
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

                                                        this.logISO(ref isoRegistro);
                                                        this.enviaDados(registro2, stream);
                                                    }
                                                }
                                            }
                                            else if (dadosRecebidos.Substring(0, 4) == "0202")
                                            {
                                                GlobalVar.frmPrincipal.AtualizaTela("Registro ISO Recebido - Confirmacao ");

                                                string registro = this.montaConfirmacaoCE(regIso);

                                                if (string.IsNullOrEmpty(registro))
                                                {
                                                    Util.LOGCHECK("Falha na desmontagem!");
                                                    Util.LOGSAIDA();
                                                }
                                                else
                                                {
                                                    Socket s = SocketConvey.connectSocket(GlobalVar.SocketIPCE, int.Parse(GlobalVar.SocketPortCE));

                                                    if (s == null)
                                                    {
                                                        Util.LOGCHECK("Conexao com Servidor FALHOU");
                                                        Util.LOGSAIDA();
                                                    }
                                                    else
                                                        this.socketEnvia(s, registro);
                                                }
                                            }
                                            else if (dadosRecebidos.Substring(0, 4) == "0400" ||
                                                     dadosRecebidos.Substring(0, 4) == "0420")
                                            {
                                                string str3;
                                                string registro1;

                                                if (dadosRecebidos.Substring(0, 4) == "0400")
                                                {
                                                    str3 = "0410";
                                                    registro1 = this.montaCancelamento(regIso, "012345678901234567890123456");
                                                }
                                                else
                                                {
                                                    str3 = "0430";
                                                    registro1 = this.montaDesfazimento(regIso);
                                                }

                                                if (string.IsNullOrEmpty(registro1))
                                                {
                                                    Util.LOGCHECK("Falha na desmontagem!");
                                                    Util.LOGSAIDA();
                                                }
                                                else if (registro1.Length < 20)
                                                {
                                                    Util.LOGCHECK("Falha na desmontagem! 2");
                                                    Util.LOGSAIDA();
                                                }
                                                else
                                                {
                                                    Socket s = SocketConvey.connectSocket(GlobalVar.SocketIPCE, int.Parse(GlobalVar.SocketPortCE));
                                                    if (s == null)
                                                    {
                                                        Util.LOGCHECK("Conexao com Servidor FALHOU");
                                                        Util.LOGSAIDA();
                                                        return;
                                                    }
                                                    this.socketEnvia(s, registro1);
                                                    string dadosRec400 = this.socketRecebe(s);

                                                    Util.LOGCHECK("dadosRecebidos 0400: >" + dadosRec400 + "<");

                                                    if (dadosRec400 == "")
                                                    {
                                                        Util.LOGCHECK("Recebeu ISO vazio");
                                                        Util.LOGSAIDA();
                                                    }
                                                    else if (dadosRec400.Length < 27)
                                                    {
                                                        Util.LOGCHECK("Recebeu ISO tamanho incorreto");
                                                        Util.LOGSAIDA();
                                                    }
                                                    else
                                                    {
                                                        dadosRec400 = dadosRec400.PadRight(200, ' ');

                                                        isoRegistro = new ISO8583();
                                                        isoRegistro.codResposta = dadosRec400.Substring(2, 2);
                                                        isoRegistro.bit127 = "000" + dadosRec400.Substring(21, 6);
                                                        isoRegistro.nsuOrigem = regIso.nsuOrigem;
                                                        isoRegistro.codProcessamento = regIso.codProcessamento;
                                                        isoRegistro.codigo = str3;
                                                        isoRegistro.codLoja = regIso.codLoja;
                                                        isoRegistro.terminal = regIso.terminal;

                                                        Util.LOGCHECK("Montagem Bit 62");

                                                        isoRegistro.bit62 = !(dadosRec400.Substring(0, 4) == "0400") ? dadosRec400.Substring(7, 6) + regIso.valor : regIso.bit125.Substring(3, 6) + regIso.valor;

                                                        string registro2 = isoRegistro.registro;
                                                        Util.LOGDADOS("RESPOSTA ISO = " + registro2);
                                                        this.logISO(ref isoRegistro);
                                                        this.enviaDados(registro2, stream);
                                                    }
                                                }
                                            }
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
                    Util.LOGDADOS("CAINDO FORA!");
                    this.closeCliente(client);
                    Util.LOGSAIDA();
                }                    
            }
            catch (System.Exception ex)
            {
                throw (ex);
            }

            start();
        }

        public void esperaConectar(ref TcpClient client, ref NetworkStream stream)
        {
            Util.LOGENTRADA();
            try
            {
                client = this.server.AcceptTcpClient();
            }
            catch (Exception ex)
            {
            }
        }

        public string esperaDados(ref TcpClient client, ref NetworkStream stream)
        {
            Util.LOGENTRADA();
            string str = (string)null;
            bool flag1 = false;
            //Console.Write("Waiting for a connection... ");
            try
            {
                Util.LOGDADOS("Waiting for a connection...");
                this.data = (string)null;
                stream = client.GetStream();

                while (true)
                {
                    if (!client.Connected)
                    {
                        Util.LOGDADOS("SAINDO (disconectado)");
                        return "";
                    }                       

                    if (stream.DataAvailable)
                        return this.leDadosSocket(client, stream);

                    Thread.Sleep(1000);

                    if (GlobalVar.finalizar)
                    {
                        Util.LOGDADOS("SAINDO GlobalVar.finalizar");
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LOGDADOS("*ERR - SAINDO por exception");
                return null;
            }
        }

        public string leDadosSocket(TcpClient client, NetworkStream stream)
        {
            Util.LOGENTRADA();
            
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
                Util.LOGERRO("EXCEPTION = Message : " + ex.Message);
            }
            catch (Exception ex)
            {
                Util.LOGERRO("EXCEPTION = Message : " + ex.Message);
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

        public void closeCliente(TcpClient client)
        {
            client.Close();
        }

        public void abort()
        {
            this.server.Stop();
            for (int index = 0; index < GlobalVar.numThreads; ++index)
            {
                this.arrayThread[index].Abort();
                this.arrayThread[index].Join(100);
            }
        }

        public void enviaDados(string Dados, NetworkStream stream)
        {
            try
            {

                Util.LOGENTRADA();
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
            catch (Exception ex)
            {
                Util.LOGERRO(" ERRO EM ENVIADADOS");
                Util.LOGERRO("EXCEPTION =" + ex.ToString());
                Util.LOGERRO("EXCEPTION = Source : " + ex.Source);
                Util.LOGERRO("EXCEPTION = Message : " + ex.Message);
            }
            Util.LOGSAIDA();
        }

        private static Socket connectSocket(string server, int port)
        {
            Socket socket1 = null;

            try
            {
                Util.LOGENTRADA();

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
                Util.LOGERRO("EXCEPTION =" + ex.ToString());
                Util.LOGERRO("EXCEPTION = Source : " + ex.Source);
                Util.LOGERRO("EXCEPTION = Message : " + ex.Message);
            }
            catch (Exception ex)
            {
                Util.LOGERRO("EXCEPTION =" + ex.ToString());
                Util.LOGERRO("EXCEPTION = Source : " + ex.Source);
                Util.LOGERRO("EXCEPTION = Message : " + ex.Message);
            }

            return socket1;
        }

        public void socketEnvia(Socket s, string registro)
        {
            try
            {
                Util.LOGENTRADA();

                byte[] bytes = Encoding.ASCII.GetBytes(registro);

                s.Send(bytes, bytes.Length, SocketFlags.None);

                Util.LOGSAIDA();
            }
            catch (System.Exception ex)
            {
                Util.LOGERRO("EXCEPTION =" + ex.ToString());
            }
        }

        public string socketRecebe(Socket s)
        {
            try
            {
                Util.LOGENTRADA();
                Encoding ascii = Encoding.ASCII;
                byte[] numArray = new byte[99999];
                int bytes = s.Receive(numArray, numArray.Length, SocketFlags.None);
                string registro = this.ConvertBytetoString(numArray, bytes);
                Util.LOGDADOS(registro);
                Util.LOGSAIDA();
                return registro;
            }
            catch (System.Exception ex)
            {
                Util.LOGERRO("EXCEPTION =" + ex.ToString());
                return "";
            }
        }

        public string conectaSocketEnvia(string ipcliente, int porta, string registro)
        {
            try
            {
                Util.LOGENTRADA();
                byte[] bytes1 = Encoding.ASCII.GetBytes(registro);
                Socket socket = SocketConvey.connectSocket(ipcliente, porta);
                if (socket == null)
                    return "Connection failed";
                byte[] numArray = new byte[99999];
                socket.Send(bytes1, bytes1.Length, SocketFlags.None);
                int bytes2 = socket.Receive(numArray, numArray.Length, SocketFlags.None);
                string str = this.ConvertBytetoString(numArray, bytes2);
                socket.Close();
                Util.LOGSAIDA();
                return str;
            }
            catch (System.Exception ex)
            {
                Util.LOGERRO("EXCEPTION =" + ex.ToString());
                return "";
            }
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
