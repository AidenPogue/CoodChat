using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsTest
{
    static class NetworkManager
    {
        public static bool Connect(string ip)
        {
            return true;
        }

        public static bool TrySendAssembly(byte[] bytes)
        {
            Console.WriteLine($"Attempting to send {bytes.Length} byte assembly...");
            return true;
        }
    }
}
