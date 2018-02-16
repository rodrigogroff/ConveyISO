﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Threading;

#region - ClientConnectionPool - 

public class ClientConnectionPool
{
    Queue SyncdQ = Queue.Synchronized(new Queue());

    public void Enqueue(ClientHandler client)
    {
        SyncdQ.Enqueue(client);
    }

    public ClientHandler Dequeue()
    {
        return (ClientHandler)(SyncdQ.Dequeue());
    }

    public int Count
    {
        get { return SyncdQ.Count; }
    }

    public object SyncRoot
    {
        get { return SyncdQ.SyncRoot; }
    }
}

#endregion

#region - ClientService - 

public class ClientService
{
    const int NUM_OF_THREAD = 10;

    ClientConnectionPool ConnectionPool;
    Thread[] ThreadTask = new Thread[NUM_OF_THREAD];

    bool ContinueProcess = false;

    public ClientService(ClientConnectionPool ConnectionPool)
    {
        this.ConnectionPool = ConnectionPool;
    }

    public void Start()
    {
        ContinueProcess = true;

        // Start threads to handle Client Task
        for (int i = 0; i < ThreadTask.Length; i++)
        {
            ThreadTask[i] = new Thread(new ThreadStart(this.Process));
            ThreadTask[i].Start();
        }
    }

    void Process()
    {
        while (ContinueProcess)
        {
            ClientHandler client = null;

            lock (ConnectionPool.SyncRoot)
            {
                if (ConnectionPool.Count > 0)
                    client = ConnectionPool.Dequeue();
            }

            if (client != null)
            {
                client.Process();

                if (client.Alive)
                    ConnectionPool.Enqueue(client);
            }

            Thread.Sleep(100);
        }
    }

    public void Stop()
    {
        ContinueProcess = false;

        for (int i = 0; i < ThreadTask.Length; i++)
        {
            if (ThreadTask[i] != null && ThreadTask[i].IsAlive)
                ThreadTask[i].Join();
        }

        while (ConnectionPool.Count > 0)
        {
            ClientHandler client = ConnectionPool.Dequeue();
            client.Close();
            Console.WriteLine("Client connection is closed!");
        }
    }
}

#endregion

public class SynchronousSocketListener
{
    // software express
    public static int portNum = 2700;

    public static int Main(String[] args)
    {
        Console.WriteLine("\nServer port...");
        var customPort = Console.ReadLine();

        if (customPort.Length > 0)
            portNum = Convert.ToInt32(customPort);

        Console.WriteLine("\nPort is: " + portNum);

        StartListening();

        Console.WriteLine("\nHit enter to continue...");
        Console.Read();

        return 0;
    }
    
    public static void StartListening()
    {
        var ConnectionPool = new ClientConnectionPool();
        var ClientTask = new ClientService(ConnectionPool);

        ClientTask.Start();

        var listener = new TcpListener(portNum);

        try
        {
            listener.Start();

            int ClientNbr = 0;

            // Start listening for connections.
            Console.WriteLine("Waiting for a connection...");

            while (true)
            {
                TcpClient handler = listener.AcceptTcpClient();

                if (handler != null)
                {
                    Console.WriteLine("Client#{0} accepted!", ++ClientNbr);
                    ConnectionPool.Enqueue(new ClientHandler(handler));
                }

                Thread.Sleep(100);
            }

            //listener.Stop();
            //ClientTask.Stop();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}

public partial class ClientHandler
{
    #region - code - 

    TcpClient ClientSocket;
    NetworkStream networkStream;

    bool ContinueProcess = false;
    byte[] bytes;

    StringBuilder msgReceived = new StringBuilder();
    string data = null;

    public ClientHandler(TcpClient ClientSocket)
    {
        ClientSocket.ReceiveTimeout = 1000; // 100 miliseconds
        this.ClientSocket = ClientSocket;
        networkStream = ClientSocket.GetStream();
        bytes = new byte[ClientSocket.ReceiveBufferSize];
        ContinueProcess = true;
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

    #endregion

    #region - log_functions - 

    public void Log(string dados)
    {
        Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:SS") + " {" + dados + "}");
    }

    public void Log(ISO8583 isoRegistro)
    {
        Log(" ISO8583-DETALHES DO REGISTRO \r\n         ======================================================== \r\n         Registro Iso : codigo       =" + isoRegistro.codigo + "\r\n         Bits preenchidos :          =" + isoRegistro.relacaoBits + "\r\n         bit( 3  ) - Codigo Proc.    =" + isoRegistro.codProcessamento + "\r\n         bit( 4  ) - valor           =" + isoRegistro.valor + "\r\n         bit( 7  ) - datahora        =" + isoRegistro.datetime + "\r\n         bit( 11 ) - NSU Origem      =" + isoRegistro.nsuOrigem + "\r\n         bit( 13 ) - data            =" + isoRegistro.Date + "\r\n         bit( 22 ) - modo captura    =" + isoRegistro.bit22 + "\r\n         bit( 35 ) - trilha          =" + isoRegistro.trilha2 + "\r\n         bit( 37 ) - nsu alternativo =" + isoRegistro.nsu + "\r\n         bit( 39 ) - codResposta     =" + isoRegistro.codResposta + "\r\n         bit( 41 ) - terminal        =" + isoRegistro.terminal + "\r\n         bit( 42 ) - codigoLoja      =" + isoRegistro.codLoja + "\r\n         bit( 49 ) - codigo moeda    =" + isoRegistro.bit49 + "\r\n         bit( 52 ) - Senha           =" + isoRegistro.senha + "\r\n         bit( 62 ) - Dados transacao =" + isoRegistro.bit62 + "\r\n         bit( 63 ) - Dados transacao =" + isoRegistro.bit63 + "\r\n         bit( 64 ) - Dados transacao =" + isoRegistro.bit64 + "\r\n         bit( 90 ) - dados original  =" + isoRegistro.bit90 + "\r\n         bit( 125 )- NSU original    =" + isoRegistro.bit125 + "\r\n         bit( 127 )- NSU             =" + isoRegistro.bit127 + "\r\n         ======================================================== \r\n");
    }

    #endregion

    private void ProcessDataReceived()
    {
        var dadosRecebidos = msgReceived.ToString();
        
        Log("ProcessDataReceived - dadosRecebidos >" + dadosRecebidos + "< (" + dadosRecebidos.Length + ")");

        if (dadosRecebidos.Length <= 20)
        {
            Log("Registro recebido invalido! (muito pequeno)");
        }
        else
        {
            if ( dadosRecebidos.Substring(0, 4) != "0200" &&
                 dadosRecebidos.Substring(0, 4) != "0202" &&
                 dadosRecebidos.Substring(0, 4) != "0400" &&
                 dadosRecebidos.Substring(0, 4) != "0420" )
            {
                Log("Código de processamento inválido!");
            }
            else
            {
                ISO8583 regIso = new ISO8583(dadosRecebidos);

                Log(regIso);

                if (regIso.erro)
                {
                    Log("Registro ISO com erro! " + regIso.strErro);
                }
                else
                {
                    if (dadosRecebidos.Substring(0, 4) == "0200" && (regIso.codProcessamento == "002000" || regIso.codProcessamento == "002800"))
                    {
                        #region - processa no CNET_SERVER - 

                        string registroCNET = !(regIso.codProcessamento == "002000") ?
                                                montaCNET_VendaCEparcelada(ref regIso) :
                                                montaCNET_VendaCE(regIso);

                        var dadosRecCNET_200 = enviaRecebeDadosCNET(registroCNET);

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

                        enviaDadosEXPRESS(Iso210.registro);                        
                    }                    
                }
            }
            
            networkStream.Close();
            ClientSocket.Close();
            ContinueProcess = false;            
        }
    }

    public string enviaRecebeDadosCNET(string registroCNET)
    {
        using (var tcpClient = new TcpClient())
        {
            tcpClient.Connect("localhost", 2000);
            NetworkStream networkStream = tcpClient.GetStream();

            byte[] sendBytes = Encoding.ASCII.GetBytes(registroCNET);
            networkStream.Write(sendBytes, 0, sendBytes.Length);

            byte[] bytes = new byte[tcpClient.ReceiveBufferSize];
            int BytesRead = networkStream.Read(bytes, 0, (int)tcpClient.ReceiveBufferSize);

            string dadosSocket = Encoding.ASCII.GetString(bytes, 0, BytesRead);
            Console.WriteLine("\nRecebido CNET Server >" + dadosSocket + "<");

            return dadosSocket;
        }
    }

    public void enviaDadosEXPRESS(string Dados)
    {
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
            Log("leDadosSocketEXPRESS EXCEPTION = Message : " + ex.Message);
        }
        catch (Exception ex)
        {
            Log("leDadosSocketEXPRESS = Message : " + ex.ToString());
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
}