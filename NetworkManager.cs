using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoodChat
{
    static class NetworkManager
    {
        public static bool Connect(string ip)
        {
            return true;
        }

        public static bool TrySendSource(string source, string entryPoint)
        {
            string toSend = $"{entryPoint}|{source}";
            Console.WriteLine($"Attempting to send {toSend.Length} byte source...");
            return true;
        }
    }
}
