using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatConsole
{
    class Program
    {

        class TelnetSocket
        {
            private TelnetSocket()
            {
            }

            public static TelnetSocket CreateSocket(Socket socket)
            {
                TelnetSocket telnetSocket = new TelnetSocket();
                telnetSocket.NetWorkStream = new NetworkStream(socket);
                telnetSocket.StreamReader = new StreamReader(telnetSocket.NetWorkStream);
                telnetSocket.StreamWriter = new StreamWriter(telnetSocket.NetWorkStream);
                telnetSocket.Socket = socket;
                return telnetSocket;
            }

            public Socket Socket { get; set; }

            public StreamWriter StreamWriter { get; set; }

            public StreamReader StreamReader { get; set; }

            public NetworkStream NetWorkStream { get; set; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Socket skt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            skt.Bind(new IPEndPoint(IPAddress.Any, 1208));
            skt.Listen(10);
            List<TelnetSocket> listsoSockets = new List<TelnetSocket>();

            Socket socket;
            Console.WriteLine("服务分发总线程");
            Task.Run(() =>
            {

                while (true)
                {
                    socket = skt.Accept();
                    TelnetSocket telnetSocket = TelnetSocket.CreateSocket(socket);
                    listsoSockets.Add(telnetSocket);
                    //(new Thread(() =>
                    //{

                    //})).Start();

                    Task.Run(() =>
                    {
                        Console.WriteLine("Accept接收到连接");
                        telnetSocket.StreamWriter.Write("/**************************/\n\r");
                        telnetSocket.StreamWriter.Write("/*                        */\n\r");
                        //socket.Send(Encoding.ASCII.GetBytes("/**************/\n"));
                        telnetSocket.StreamWriter.Write("/*Welcome to Light Of Pie**/\n\r");
                        telnetSocket.StreamWriter.Write("/*                        */\n\r");
                        telnetSocket.StreamWriter.WriteLine("/**************************/\n\r");
                        telnetSocket.StreamWriter.Flush();
                        bool StopFlag = true;
                        bool IsConnected = false;
                        while (StopFlag)
                        {

                            string message = telnetSocket.StreamReader.ReadLine();

                            if (IsConnected == true)
                            {
                                if (message == null)
                                {
                                    StopFlag = false;
                                    listsoSockets.Remove(telnetSocket);
                                }
                            }

                            //发送给其他客户端
                            if (message == null)
                            {
                                //StopFlag = false;
                                IsConnected = true;
                            }
                            if (!telnetSocket.Socket.Connected)
                            {
                                //StopFlag = false;
                            }
                            else
                            {
                                foreach (TelnetSocket oneSocket in listsoSockets)
                                {
                                   // if (!oneSocket.Socket.Connected) continue;
                                    if (!object.ReferenceEquals(oneSocket, telnetSocket))
                                    {
                                        oneSocket.StreamWriter.Write(message?.Trim()+ "\r\n");
                                        oneSocket.StreamWriter.Flush();

                                        Console.WriteLine("消息已发");
                                    }
                                }
                            }
                        }
                    });
                }

            });


            Console.WriteLine("服务通信总线程");
            Console.Read();





        }
    }
}
