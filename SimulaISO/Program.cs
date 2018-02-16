﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimulaISO
{
    class Program
    {
        private const int portNum = 2700;

        static void Main(string[] args)
        {
            Envia200();
        }

        static void Envia200()
        {
            var tcpClient = new TcpClient();

            try
            {
                tcpClient.Connect("localhost", portNum);
                NetworkStream networkStream = tcpClient.GetStream();

                if (networkStream.CanWrite && networkStream.CanRead)
                {
                    var DataToSend = "";

                    while (DataToSend != "quit")
                    {
                        Console.WriteLine("\nType a text to be sent:");
                        DataToSend = Console.ReadLine();
                        if (DataToSend.Length == 0) break;

                        Byte[] sendBytes = Encoding.ASCII.GetBytes(DataToSend);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);

                        // Reads the NetworkStream into a byte buffer.
                        byte[] bytes = new byte[tcpClient.ReceiveBufferSize];
                        int BytesRead = networkStream.Read(bytes, 0, (int)tcpClient.ReceiveBufferSize);

                        // Returns the data received from the host to the console.
                        string returndata = Encoding.ASCII.GetString(bytes, 0, BytesRead);
                        Console.WriteLine("This is what the host returned to you: \r\n{0}", returndata);
                    }
                    networkStream.Close();
                    tcpClient.Close();
                }
                else if (!networkStream.CanRead)
                {
                    Console.WriteLine("You can not write data to this stream");
                    tcpClient.Close();
                }
                else if (!networkStream.CanWrite)
                {
                    Console.WriteLine("You can not read data from this stream");
                    tcpClient.Close();
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
        }
    }
}
