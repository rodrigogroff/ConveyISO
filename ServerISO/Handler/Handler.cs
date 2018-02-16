﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public partial class ClientHandler
{
    public ClientHandler(TcpClient ClientSocket, int number)
    {
        ClientSocket.ReceiveTimeout = 1000;
        this.ClientSocket = ClientSocket;
        networkStream = ClientSocket.GetStream();
        bytes = new byte[ClientSocket.ReceiveBufferSize];
        ContinueProcess = true;

        strLogFile = "logFile_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + GetRandomString(9) + ".txt";
        
        sw = new StreamWriter(strLogFile, false)
        {
            AutoFlush = true
        };

        Log("");
        Log("==========================================");
        Log("CNET ISO vr 1.00 [" + number + "]");
        Log("==========================================");
        Log("");
    }

    #region - code - 

    TcpClient ClientSocket;
    NetworkStream networkStream;
    StringBuilder msgReceived = new StringBuilder();
    Random random = new Random();

    string strLogFile = "";
    bool ContinueProcess = false;
    byte[] bytes;

    int RandomNumber(int min, int max)
    {
        Thread.Sleep(1);
        return random.Next(min, max);
    }

    public string GetRandomString(int size)
    {
        Thread.Sleep(100);

        var ret = "";

        for (int t = 0; t < size; ++t)
            ret += RandomNumber(0, 9).ToString();

        return ret;
    }

    public void Process()
    {
        try
        {
            int BytesRead = networkStream.Read(bytes, 0, (int)bytes.Length);

            if (BytesRead > 0)
                // There might be more data, so store the data received so far.
                msgReceived.Append(Encoding.ASCII.GetString(bytes, 0, BytesRead));
            else
                // All the data has arrived; put it in response.
                ProcessDataReceived();
        }
        catch (IOException)
        {
            // All the data has arrived; put it in response.
            ProcessDataReceived();
        }
        catch (SocketException)
        {
            networkStream.Close();
            ClientSocket.Close();
            ContinueProcess = false;
            Console.WriteLine("Conection is broken!");
        }
    }

    public void Close()
    {
        networkStream.Close();
        ClientSocket.Close();
    }

    public bool Alive
    {
        get
        {
            return ContinueProcess;
        }
    }

    #endregion

    #region - log_functions - 

    StreamWriter sw;

    public void Log(string dados)
    {
        var st = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " {" + dados + "}";

        sw.WriteLine(st);
        Console.WriteLine(st);
    }

    int numFalha = 1;

    public void LogFalha(string dados)
    {
        var st = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " {" + dados + "}";

        var swFalha = new StreamWriter("FALHA" + numFalha + strLogFile, false)
        {
            AutoFlush = true
        };

        numFalha++;

        swFalha.WriteLine(st);
        Console.WriteLine(st);
    }

    public void Log(ISO8583 isoRegistro)
    {
        // somente no disco!

        var dados = " ISO8583-DETALHES DO REGISTRO \r\n         ======================================================== \r\n         Registro Iso : codigo       =" + isoRegistro.codigo + "\r\n         Bits preenchidos :          =" + isoRegistro.relacaoBits + "\r\n         bit( 3  ) - Codigo Proc.    =" + isoRegistro.codProcessamento + "\r\n         bit( 4  ) - valor           =" + isoRegistro.valor + "\r\n         bit( 7  ) - datahora        =" + isoRegistro.datetime + "\r\n         bit( 11 ) - NSU Origem      =" + isoRegistro.nsuOrigem + "\r\n         bit( 13 ) - data            =" + isoRegistro.Date + "\r\n         bit( 22 ) - modo captura    =" + isoRegistro.bit22 + "\r\n         bit( 35 ) - trilha          =" + isoRegistro.trilha2 + "\r\n         bit( 37 ) - nsu alternativo =" + isoRegistro.nsu + "\r\n         bit( 39 ) - codResposta     =" + isoRegistro.codResposta + "\r\n         bit( 41 ) - terminal        =" + isoRegistro.terminal + "\r\n         bit( 42 ) - codigoLoja      =" + isoRegistro.codLoja + "\r\n         bit( 49 ) - codigo moeda    =" + isoRegistro.bit49 + "\r\n         bit( 52 ) - Senha           =" + isoRegistro.senha + "\r\n         bit( 62 ) - Dados transacao =" + isoRegistro.bit62 + "\r\n         bit( 63 ) - Dados transacao =" + isoRegistro.bit63 + "\r\n         bit( 64 ) - Dados transacao =" + isoRegistro.bit64 + "\r\n         bit( 90 ) - dados original  =" + isoRegistro.bit90 + "\r\n         bit( 125 )- NSU original    =" + isoRegistro.bit125 + "\r\n         bit( 127 )- NSU             =" + isoRegistro.bit127 + "\r\n         ======================================================== \r\n";
        var st = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " {" + dados + "}";

        sw.WriteLine(st);
    }

    #endregion

    #region - socket functions - 
    
    public void enviaDadosCNET(TcpClient tcpClient, string registroCNET)
    {
        Log("enviaDadosCNET " + registroCNET);

        try
        {
            var networkStream = tcpClient.GetStream();

            byte[] sendBytes = Encoding.ASCII.GetBytes(registroCNET);
            networkStream.Write(sendBytes, 0, sendBytes.Length);
        }
        catch (SocketException ex)
        {
            LogFalha("enviaDadosCNET SocketException : " + ex.Message);
            throw ex;
        }
        catch (Exception ex)
        {
            LogFalha("enviaDadosCNET Exception : " + ex.ToString());
            throw ex;
        }
    }

    public string enviaRecebeDadosCNET(TcpClient tcpClient, string registroCNET)
    {
        Log("enviaRecebeDadosCNET (enviado) " + registroCNET);

        try
        {
            NetworkStream networkStream = tcpClient.GetStream();

            byte[] sendBytes = Encoding.ASCII.GetBytes(registroCNET);
            networkStream.Write(sendBytes, 0, sendBytes.Length);

            byte[] bytes = new byte[tcpClient.ReceiveBufferSize];
            int BytesRead = networkStream.Read(bytes, 0, (int)tcpClient.ReceiveBufferSize);

            string dadosSocket = Encoding.ASCII.GetString(bytes, 0, BytesRead);

            Log("enviaRecebeDadosCNET (recebido) " + dadosSocket);

            return dadosSocket;
        }
        catch (SocketException ex)
        {
            LogFalha("enviaRecebeDadosCNET SocketException : " + ex.Message);
            throw ex;
        }
        catch (Exception ex)
        {
            LogFalha("enviaRecebeDadosCNET Exception : " + ex.ToString());
            throw ex;
        }
    }

    public void enviaDadosEXPRESS(string Dados)
    {
        Log("enviaDadosEXPRESS " + Dados);

        try
        {
            string str = string.Format("{0:X2}", (object)Dados.Length).PadLeft(4, '0');
            byte[] numArray = new byte[2];
            for (int index = 0; index < str.Length / 2; ++index)
                numArray[index] = (byte)Convert.ToInt32(str.Substring(index * 2, 2), 16);
            byte[] bytes = Encoding.ASCII.GetBytes("00" + Dados);
            bytes[0] = numArray[1];
            bytes[1] = numArray[0];

            Log("tamanho a ser enviado em hexa:" + str.Substring(2) + str.Substring(0, 2));

            networkStream.Write(bytes, 0, bytes.Length);
        }
        catch (SocketException ex)
        {
            LogFalha("enviaDadosEXPRESS SocketException : " + ex.Message);
            throw ex;
        }
        catch (Exception ex)
        {
            LogFalha("enviaDadosEXPRESS Exception : " + ex.ToString());
            throw ex;
        }
    }

    #endregion

    private void ProcessDataReceived()
    {
        bool bFinaliza = false;

        try
        {
            var dadosRecebidos = msgReceived.ToString();

            msgReceived.Clear();
            
            Log("ProcessDataReceived - dadosRecebidos >" + dadosRecebidos + "<");

            if (dadosRecebidos.Length <= 20)
            {
                Log("Registro recebido invalido! (muito pequeno: " + dadosRecebidos.Length + ")");
            }
            else
            {
                var isoCode = dadosRecebidos.Substring(0, 4);

                Log("isoCode " + isoCode);
                
                if (isoCode != "0200" && isoCode != "0202" && isoCode != "0400" && isoCode != "0420")
                {
                    Log("Código de processamento inválido!");
                }
                else
                {
                    var regIso = new ISO8583(dadosRecebidos);
                    
                    Log(regIso);

                    if (regIso.erro)
                    {
                        Log("Registro ISO com erro! " + regIso.strErro);
                    }
                    else
                    {
                        var isoCodProc = regIso.codProcessamento;

                        if (isoCode == "0200" && (isoCodProc == "002000" || isoCodProc == "002800"))
                        {
                            #region - code - 

                            using (var tcpClient = new TcpClient("localhost", 2000))
                            {
                                // montagem

                                string registroCNET = !(regIso.codProcessamento == "002000") ?
                                                    montaCNET_VendaCEparcelada(ref regIso) :
                                                    montaCNET_VendaCE(regIso);

                                if (registroCNET == null)
                                {
                                    LogFalha(isoCode + " montaCNET_Venda nulo");
                                }
                                else if (registroCNET.Length < 20)
                                {
                                    LogFalha(isoCode + " montaCNET_Venda tamanho inválido");
                                }
                                else
                                {
                                    // --------------------------------
                                    // processamento no cnet server VENDA
                                    // --------------------------------

                                    var dadosRecCNET_200 = enviaRecebeDadosCNET(tcpClient, registroCNET);
                                                                        
                                    // --------------------------------
                                    // cria 210 EXPRESS
                                    // --------------------------------

                                    #region - code - 

                                    var Iso210 = new ISO8583
                                    {
                                        codResposta = dadosRecCNET_200.Substring(2, 2),
                                        bit127 = "000" + dadosRecCNET_200.Substring(7, 6),
                                        nsuOrigem = regIso.nsuOrigem,
                                        codProcessamento = regIso.codProcessamento,
                                        codigo = "0210",
                                        valor = regIso.valor,
                                        terminal = regIso.terminal,
                                        codLoja = regIso.codLoja
                                    };

                                    if (regIso.codProcessamento != "002000")
                                        Iso210.bit63 = regIso.bit62;

                                    string str4, str5, str6;

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

                                    Iso210.bit62 = !(dadosRecCNET_200.Substring(2, 2) == "00") ?
                                        dadosRecCNET_200.Substring(73, 20) :
                                        str5 + str6 + str4.Substring(18, 3) + dadosRecCNET_200.Substring(27, 40);

                                    Log(Iso210);

                                    #endregion

                                    // --------------------------------
                                    // envia 210 EXPRESS
                                    // --------------------------------

                                    enviaDadosEXPRESS(Iso210.registro);
                                }
                            }

                            #endregion
                                                        
                            bFinaliza = false; // continua depois via 202
                        }
                        else if (isoCode == "0202")
                        {
                            #region - code - 

                            using (var tcpClient = new TcpClient("localhost", 2000))
                            {
                                var strRegIso = montaConfirmacaoCE(regIso);

                                if (strRegIso == null)
                                {
                                    LogFalha(isoCode + " montaDesfazimento / montaCancelamento nulo");
                                }
                                else if (strRegIso.Length < 20)
                                {
                                    LogFalha(isoCode + " montaDesfazimento / montaCancelamento tamanho inválido");
                                }
                                else
                                {
                                    // --------------------------------
                                    // processamento no cnet server CONF
                                    // --------------------------------

                                    enviaDadosCNET(tcpClient, montaConfirmacaoCE(regIso));
                                }
                            }

                            #endregion

                            bFinaliza = true;
                        }
                        else if (isoCode == "0400" || isoCode == "0420")
                        {
                            #region - 400 || 420 - 

                            string codigoIso, strRegIso;

                            // montagem

                            if (isoCode == "0400")
                            {
                                codigoIso = "0410";
                                strRegIso = montaCancelamento(regIso, "012345678901234567890123456");
                            }
                            else
                            {
                                codigoIso = "0430";
                                strRegIso = montaDesfazimento(regIso);
                            }

                            Log("codigoIso " + codigoIso);

                            if (strRegIso == null)
                            {
                                LogFalha(isoCode + " montaDesfazimento / montaCancelamento nulo" );
                            }
                            else if (strRegIso.Length < 20)
                            {
                                LogFalha(isoCode + " montaDesfazimento / montaCancelamento tamanho inválido");
                            }
                            else
                            {
                                using (var tcpClient = new TcpClient("localhost", 2000))
                                {
                                    // --------------------------------
                                    // processamento no cnet server DESFAZ
                                    // --------------------------------

                                    string dadosRec400 = enviaRecebeDadosCNET(tcpClient, strRegIso);

                                    if (codigoIso == "0410")
                                    {
                                        // -------------------------
                                        // cancelamento
                                        // -------------------------

                                        if (dadosRec400 == "")
                                        {
                                            Log(isoCode + "Recebeu ISO vazio");
                                        }
                                        else if (dadosRec400.Length < 27)
                                        {
                                            Log(isoCode + "Recebeu ISO tamanho incorreto");
                                        }
                                        else
                                        {
                                            dadosRec400 = dadosRec400.PadRight(200, ' ');

                                            var isoRegistro = new ISO8583
                                            {
                                                codigo = codigoIso,
                                                codProcessamento = regIso.codProcessamento,
                                                codLoja = regIso.codLoja,
                                                terminal = regIso.terminal,
                                                codResposta = dadosRec400.Substring(2, 2),
                                                bit127 = "000" + dadosRec400.Substring(21, 6),
                                                nsuOrigem = regIso.nsuOrigem,
                                            };

                                            Log("Montagem Bit 62");

                                            isoRegistro.bit62 = !(dadosRec400.Substring(0, 4) == "0400") ?
                                                dadosRec400.Substring(7, 6) + regIso.valor :
                                                regIso.bit125.Substring(3, 6) + regIso.valor;

                                            Log(isoRegistro);

                                            enviaDadosEXPRESS(isoRegistro.registro);
                                        }
                                    }
                                    else
                                    {
                                        // -------------------------
                                        // desfazimento não devolve EXPRESS
                                        // -------------------------
                                    }
                                }
                            }

                            #endregion

                            bFinaliza = true;
                        }
                        else
                            bFinaliza = true;
                    }
                }
            }
        }
        catch (SystemException ex)
        {
            LogFalha("ProcessDataReceived SystemException " + ex.ToString());
            bFinaliza = true;
        }
        catch (Exception ex)
        {
            LogFalha("ProcessDataReceived Exception " + ex.ToString());
            bFinaliza = true;
        }
        
        if (bFinaliza)
        {
            Log("ProcessDataReceived FINALIZADO ");

            networkStream.Close();
            ClientSocket.Close();
            ContinueProcess = false;
            sw.Close();
        }
    }    
}
