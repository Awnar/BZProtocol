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

        private UdpClient serwer = null;

        static void Main(string[] args)
        {
            Console.WriteLine("Witaj");
            UdpClient serwer = new UdpClient();

            serwer.Connect(DEFAULT_SERVER, DEFAULT_PORT);

            Frame frame = new Frame();

        }

        private void send(Frame fr)
        {
            var data = fr.gen();
            serwer.Send(data, data.Length);
        }

        private void receive()
        {
            var timeToWait = TimeSpan.FromSeconds(10);

            var udpClient = new UdpClient(DEFAULT_PORT);
            var asyncResult = udpClient.BeginReceive(null, null);
            asyncResult.AsyncWaitHandle.WaitOne(timeToWait);
            if (asyncResult.IsCompleted)
            {
                try
                {
                    IPEndPoint remoteEP = null;
                    byte[] receivedData = udpClient.EndReceive(asyncResult, ref remoteEP);
                    // EndReceive worked and we have received data and remote endpoint
                }
                catch (Exception ex)
                {
                    // EndReceive failed and we ended up here
                }
            }
            else
            {
                // The operation wasn't completed before the timeout and we're off the hook
            }
        }
    }
}
