using System;
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
    public static int portNum = 2000;

    public static int Main(String[] args)
    {
        Console.WriteLine("\nCNET Emulator server port: " + portNum);

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

public class ClientHandler
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
        Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " {tam:" + dados + "}");
    }
    
    #endregion

    private void ProcessDataReceived()
    {
        var dadosRecebidos = msgReceived.ToString();

        msgReceived = new StringBuilder();

        var bQuit = false;

        if (dadosRecebidos.Length == 0)
        {
            bQuit = true;
        }
        else
        {
            Log("ProcessDataReceived - dadosRecebidos >" + dadosRecebidos + "< (" + dadosRecebidos.Length + ")");

            if (dadosRecebidos.StartsWith("05CECE"))
            {
                var resp = "000000S00006720180215000067EMERSON CONRADO RIBEIRO                 004200                    000000000006156000096229913480015-02-201810:57:56068Saldo disponível no mês: 501,25 * Saldo disponível parcelado: 501,25";

                byte[] sendBytes = Encoding.ASCII.GetBytes(resp);
                networkStream.Write(sendBytes, 0, sendBytes.Length);

                bQuit = true;
            }
            else if (dadosRecebidos.StartsWith("05CEDF"))
            {
                var resp = "0";

                byte[] sendBytes = Encoding.ASCII.GetBytes(resp);
                networkStream.Write(sendBytes, 0, sendBytes.Length);

                bQuit = true;
            }
            else if (dadosRecebidos.StartsWith("05CECA"))
            {
                var resp = "000000S00007820171115000078HELENA APARECIDA MANOEL                 290000Cancelado           00000000000609200000000000077000077000000000140100659200000000759215-11-201718:08:5915-11-201718:14:45";

                byte[] sendBytes = Encoding.ASCII.GetBytes(resp);
                networkStream.Write(sendBytes, 0, sendBytes.Length);

                bQuit = true;
            }
            else
                bQuit = true;
        }

        if (bQuit)
        {
            networkStream.Close();
            ClientSocket.Close();
            ContinueProcess = false;
        }

        #region - sample code - 

        /*
         * 
         * 05CECE1000680882676600962299134801265092175CFDE84D7B2654300000000338201000000003382*********************************************************************************************************************00006811150094
15-02-18 10:57:56 AM# Pid:016352: L(0) hnd:005092
-> [ENTRADA] SocketConvey:connectSocket 
15-02-18 10:57:56 AM# Pid:016352: L(0) hnd:005160 SocketConvey:connectSocket
-> [ SAIDA ] SocketConvey:connectSocket 
15-02-18 10:57:56 AM# Pid:016352: L(0) hnd:005188
-> [ENTRADA] SocketConvey:socketEnvia 
15-02-18 10:57:56 AM# Pid:016352: L(0) hnd:005180 SocketConvey:socketEnvia
-> [ SAIDA ] SocketConvey:socketEnvia 
15-02-18 10:57:56 AM# Pid:016352: L(0) hnd:005192
-> [ENTRADA] SocketConvey:socketRecebe 
15-02-18 10:57:56 AM# Pid:016352: L(0) hnd:005196 SocketConvey:socketRecebe
-> [ DADOS ] 000000S00006720180215000067EMERSON CONRADO RIBEIRO                 004200                    000000000006156000096229913480015-02-201810:57:56068Saldo disponível no mês: 501,25 * Saldo disponível parcelado: 501,25


         * 
        bool bQuit = (String.Compare(msgReceived.ToString(), "quit", true) == 0);

        data = msgReceived.ToString();

        msgReceived.Length = 0; // Clear buffer

        Console.WriteLine("Text received from client:");
        Console.WriteLine(data);

        StringBuilder response = new StringBuilder();
        response.Append("Received at ");
        response.Append(DateTime.Now.ToString());
        response.Append("\r\n");
        response.Append(data);

        // Echo the data back to the client.
        byte[] sendBytes = Encoding.ASCII.GetBytes(response.ToString());
        networkStream.Write(sendBytes, 0, sendBytes.Length);

        // Client stop processing  
        if (bQuit)
        {
            networkStream.Close();
            ClientSocket.Close();
            ContinueProcess = false;
        }
        */

        #endregion
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
