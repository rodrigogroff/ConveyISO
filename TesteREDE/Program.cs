using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TesteREDE
{
    class SimulaISO_Program
    {
        static void Main(string[] args)
        {
            int portNum = 2700;

            try
            {
                Console.WriteLine("IP? [1] localhost [2] 138.118.164.191 ");

                string myIp = Console.ReadLine();

                if (myIp == "1" || myIp == "")
                    myIp = "localhost";
                else if (myIp == "2")
                    myIp = "138.118.164.191";

                Console.WriteLine(">> " + myIp);

                Console.WriteLine("port? [2700]");

                string myPort = Console.ReadLine();

                if (myPort != "")
                    portNum = Convert.ToInt32(myPort);

                Console.WriteLine(">> porta " + portNum);

                Console.WriteLine("repetir? [S] [N]");

                string myStr = Console.ReadLine().ToUpper();

                Console.WriteLine(">> Abrindo conexão");

                while (true)
                {
                    using (var tcpClient = new TcpClient())
                    {
                        tcpClient.Connect(myIp, portNum);
                        NetworkStream networkStream = tcpClient.GetStream();

                        if (networkStream.CanWrite)
                        {
                            Console.WriteLine(">> Conexão aberta");

                            var DataToSend = "PING";

                            Console.WriteLine(">> Tentando escrever");

                            {
                                Byte[] sendBytes = Encoding.ASCII.GetBytes(DataToSend);
                                networkStream.Write(sendBytes, 0, sendBytes.Length);
                                networkStream.Flush();
                            }

                            Console.WriteLine("Enviado " + DataToSend);

                            {
                                Console.WriteLine(">> Tentando ler");

                                // Reads the NetworkStream into a byte buffer.
                                byte[] bytes = new byte[tcpClient.ReceiveBufferSize];
                                int BytesRead = networkStream.Read(bytes, 0, (int)tcpClient.ReceiveBufferSize);

                                // Returns the data received from the host to the console.
                                string returndata = Encoding.UTF7.GetString(bytes, 0, BytesRead);

                                Console.WriteLine("Recebido " + returndata);

                                if (returndata == "PONG")
                                    Console.WriteLine("\n ======== TESTE OK ============ ");
                                else
                                    Console.WriteLine("\n ############ !!! TESTE NAO OK !!! #################### ");
                            }

                            if (myStr == "N")
                                break;

                            Thread.Sleep(500);
                        }

                        networkStream.Close();
                        tcpClient.Close();
                    }
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Sever not available!");
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("Sever not available!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.ReadLine();
            Console.ReadLine();
        }
    }
}
