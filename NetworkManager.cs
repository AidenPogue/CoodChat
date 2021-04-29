using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace CoodChat
{
    static class NetworkManager
    {
        public static bool Hosting { get; private set; }
        private static int bufferSize = 8192;

        private static List<Socket> clients = new List<Socket>();

        private static TcpClient myClient;

        private static void HandleTcpRecievedData(byte[] data, Socket fromSocket)
        {
            Console.WriteLine($"Got {data.Length} bytes over TCP.");
            string dataString = Encoding.UTF8.GetString(data).Trim('\0');
            if (Hosting)
            {
                SendDataToClients(data, fromSocket);
            }

            var barSplit = dataString.Split('|');
            
            switch(barSplit[0])
            {
                //Source code
                case "c":
                    {
                        AssemblyManager.TryBuildAndExecuteAssembly(barSplit[2], barSplit[1]);
                        break;
                    }
                //Message
                case "m":
                    {
                        Console.WriteLine(barSplit[1]);
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"Something has gone horribly wrong with the recived data.");
                        break;
                    }
            }
        }

        #region Server
        public static void Host(int port)
        {
            Console.WriteLine($"Starting server on port {port}...");
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Hosting = true;
            ConnectionListenLoop(server);
            
        }

        private static async void ConnectionListenLoop(TcpListener server)
        {
            Console.WriteLine("Server has started and is listening.");

            while (true)
            {
                var acceptedClient = await server.AcceptTcpClientAsync();
                Console.WriteLine($"TCP client {acceptedClient.Client.RemoteEndPoint} connected.");
                clients.Add(acceptedClient.Client);
                new Thread(() => AwaitSocketData(acceptedClient.Client)).Start();
            }
        }

        private static void AwaitSocketData(Socket socket)
        {
            var buffer = new byte[bufferSize];
            while (true)
            {
                Array.Clear(buffer, 0, bufferSize);
                try
                {
                    socket.Receive(buffer, SocketFlags.None);
                }
                catch 
                {
                    Console.WriteLine($"Connection to {(Hosting ? "client" : "server")} at {socket.RemoteEndPoint} closed.");
                    if (Hosting)
                    {
                        clients.Remove(socket);
                    }
                    else
                    {
                        NetworkPopupForm.ShowAsPopup();
                    }
                    Thread.CurrentThread.Abort();
                }

                HandleTcpRecievedData(buffer, socket);
            }
        }

        public static void SendDataToClients(byte[] data, Socket excludeClient)
        {
            foreach(Socket client in clients)
            {
                //Dont want to send code back to the person who sent it.
                if (client == excludeClient) continue;
                client.Send(data);
            }
        }
        #endregion

        #region Client
        public static bool TryConnect(string ip, int port, string username)
        {
            Console.WriteLine($"Connecting to {ip}:{port}...");
            try
            {
                myClient = new TcpClient(ip, port);
            }
            catch
            {
                Console.WriteLine("Connection failed.");
                return false;
            }
            Console.WriteLine("Connected.");
            SendToServer(username);
            new Thread(() => AwaitSocketData(myClient.Client)).Start();
            return true;
        }

        public static void SendToServer(string content, string prefix)
        {
            string toSend = $"{prefix}|{content}";
            Console.WriteLine($"Attempting to send {toSend.Length} byte source...");
            if (toSend.Length > bufferSize)
            {
                Console.WriteLine($"Code is {toSend.Length - bufferSize} bytes larger than the buffer size.");
                return;
            }
            //Terrible code incoming
            var bytes = Encoding.UTF8.GetBytes(toSend);
            if (Hosting)
            {
                SendDataToClients(bytes, null);
            }
            else
            {
                myClient.GetStream().Write(bytes, 0, toSend.Length);
            }
            Console.WriteLine("Bytes sent.");
        }
        #endregion

    }
}