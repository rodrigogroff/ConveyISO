using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimulaISO
{
    class SimulaISO_Program
    {
        private const int portNum = 2700;

        static void Main(string[] args)
        {
            //Envia200_202();
            //Envia420_desfazimento();
            Envia400_410();
        }

        static void Envia200_202()
        {
            var tcpClient = new TcpClient();

            try
            {
                tcpClient.Connect("localhost", portNum);
                NetworkStream networkStream = tcpClient.GetStream();

                if (networkStream.CanWrite)
                {
                    var DataToSend = "0200B238040020C0100000000000000000000020000000000033820215105755150094105755021502137826766009622991348012650921          CX00000300000000000680875CFDE84D7B26543";

                    {
                        Byte[] sendBytes = Encoding.ASCII.GetBytes(DataToSend);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                    }

                    Console.WriteLine("\nEnviado " + DataToSend);

                    {
                        // Reads the NetworkStream into a byte buffer.
                        byte[] bytes = new byte[tcpClient.ReceiveBufferSize];
                        int BytesRead = networkStream.Read(bytes, 0, (int)tcpClient.ReceiveBufferSize);
                                        
                        // Returns the data received from the host to the console.
                        string returndata = Encoding.UTF7.GetString(bytes, 0, BytesRead);
                        Console.WriteLine("\nRecebido " + returndata);
                    }
                        
                    DataToSend = "0202B238000002C0000400000000000000020020000000000033820215105811150094105755021500CX000003000000000006808027826766009622991348012650921009000000067";

                    {
                        Byte[] sendBytes = Encoding.ASCII.GetBytes(DataToSend);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                    }

                    Console.WriteLine("\nEnviado " + DataToSend);
                }
                
                networkStream.Close();
                tcpClient.Close();
                
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
        }

        static void Envia420_desfazimento()
        {
            var tcpClient = new TcpClient();

            try
            {
                tcpClient.Connect("localhost", portNum);
                NetworkStream networkStream = tcpClient.GetStream();

                if (networkStream.CanWrite)
                {
                    var DataToSend = "0420B238000000C00000000000000000000000200000000000305802151456129810271456120215CX000004000000000006822";

                    {
                        Byte[] sendBytes = Encoding.ASCII.GetBytes(DataToSend);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                    }

                    Console.WriteLine("\nEnviado " + DataToSend);

                    {
                        // Reads the NetworkStream into a byte buffer.
                        byte[] bytes = new byte[tcpClient.ReceiveBufferSize];
                        int BytesRead = networkStream.Read(bytes, 0, (int)tcpClient.ReceiveBufferSize);

                        // Returns the data received from the host to the console.
                        string returndata = Encoding.ASCII.GetString(bytes, 0, BytesRead);
                        Console.WriteLine("\nRecebido " + returndata);
                    }
                }

                networkStream.Close();
                tcpClient.Close();

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
        }
        
        static void Envia400_410()
        {
            var tcpClient = new TcpClient();

            try
            {
                tcpClient.Connect("localhost", portNum);
                NetworkStream networkStream = tcpClient.GetStream();

                if (networkStream.CanWrite)
                {
                    var DataToSend = "0400B238000000C0000000000000000000080028000000000075921115181457153510181457111500000090000000000006319009000000077";

                    {
                        Byte[] sendBytes = Encoding.ASCII.GetBytes(DataToSend);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                    }

                    Console.WriteLine("\nEnviado " + DataToSend);

                    {
                        // Reads the NetworkStream into a byte buffer.
                        byte[] bytes = new byte[tcpClient.ReceiveBufferSize];
                        int BytesRead = networkStream.Read(bytes, 0, (int)tcpClient.ReceiveBufferSize);

                        // Returns the data received from the host to the console.
                        string returndata = Encoding.UTF7.GetString(bytes, 0, BytesRead);
                        Console.WriteLine("\nRecebido " + returndata);
                    }
                }

                networkStream.Close();
                tcpClient.Close();

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
        }
    }
}
