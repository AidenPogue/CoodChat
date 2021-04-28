using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace CoodChat
{
    static class NetworkManager
    {
        public static bool Hosting { get; private set; }

        private static List<Socket> clients = new List<Socket>();

        private static TcpClient myClient;

        private static void HandleTcpRecievedData(string data, Socket fromSocket)
        {
            Console.WriteLine($"Got {data.Length} bytes over TCP.");
            if (Hosting)
            {
                SendDataToClients(data, fromSocket);
            }

            var sourceAndEntryPoint = data.Split('|');
            AssemblyManager.TryBuildAndExecuteAssembly(sourceAndEntryPoint[1], sourceAndEntryPoint[0]);
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
                Console.WriteLine("Got TCP client.");
                clients.Add(acceptedClient.Client);
                new Thread(() => AwaitSocketData(acceptedClient.Client)).Start();
            }
        }

        private static void AwaitSocketData(Socket socket)
        {
            while (true)
            {
                var buffer = new byte[8192];
                try
                {
                    socket.Receive(buffer, SocketFlags.None);
                }
                catch 
                {
                    Console.WriteLine($"Connection to {(Hosting ? "client" : "server")} closed.");
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

                HandleTcpRecievedData(Encoding.UTF8.GetString(buffer).Trim('\0'), socket);
            }
        }

        public static void SendDataToClients(string data, Socket excludeClient)
        {
            foreach(Socket client in clients)
            {
                //Dont want to send code back to the person who sent it.
                if (client == excludeClient) continue;
                client.Send(Encoding.UTF8.GetBytes(data));
            }
        }
        #endregion

        #region Client
        public static bool TryConnect(string ip, int port)
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
            new Thread(() => AwaitSocketData(myClient.Client)).Start();
            return true;
        }

        public static void SendSource(string source, string entryPoint)
        {
            string toSend = $"{entryPoint}|{source}";
            Console.WriteLine($"Attempting to send {toSend.Length} byte source...");
            //Terrible code incoming
            if (Hosting)
            {
                SendDataToClients(toSend, null);
            }
            else
            {
                var stream = myClient.GetStream();
                //Console.WriteLine(stream.Length);
                stream.Write(Encoding.UTF8.GetBytes(toSend), 0, toSend.Length);
            }
            Console.WriteLine("Bytes sent.");
        }
        #endregion

    }
}
