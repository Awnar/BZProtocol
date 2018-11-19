using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using protocol;

namespace klient
{
    class Program
    {
        private static IPAddress DEFAULT_SERVER = IPAddress.Parse("127.0.0.1");
        private static int DEFAULT_PORT = 9999;
        private static IPEndPoint IPserwer;

        static void Main(string[] args)
        {
            Console.WriteLine("Witaj");
            UdpClient serwer = new UdpClient();

            serwer.Connect(DEFAULT_SERVER, DEFAULT_PORT);

            Frame frame = new Frame();
            var data = frame.gen();

            serwer.Send(data, data.Length);

            data = serwer.Receive(ref IPserwer);
        }
    }
}
