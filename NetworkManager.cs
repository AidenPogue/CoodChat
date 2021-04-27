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

        private static List<TcpClient> clients = new List<TcpClient>();

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
                clients.Add(acceptedClient);
                new Thread(() => AwaitSocketData(acceptedClient.Client)).Start();
            }
        }

        private static void AwaitSocketData(Socket socket)
        {
            byte[] buffer = new byte[2048];
            while (true)
            {
                socket.Receive(buffer, SocketFlags.None);
                HandleTcpRecievedData(Encoding.ASCII.GetString(buffer).Trim('\0'), socket);
            }
        }

        public static void SendDataToClients(string data, Socket excludeClient)
        {
            foreach(TcpClient client in clients)
            {
                //Dont want to send code back to the person who sent it.
                if (client.Client == excludeClient) continue;
                client.Client.Send(Encoding.ASCII.GetBytes(data));
            }
        }
        #endregion

        #region Client
        public static bool TryConnect(string ip, int port)
        {
            Console.WriteLine($"Connecting to {ip}:{port}...");
            myClient = new TcpClient(ip, port);
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
                myClient.GetStream().Write(Encoding.ASCII.GetBytes(toSend), 0, toSend.Length);
            }
            Console.WriteLine("Bytes sent.");
        }
        #endregion

    }
}
