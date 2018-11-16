using protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace serwer
{
    class UDPSerwer
    {
        public static IPAddress DEFAULT_SERVER = IPAddress.Any; //IPAddress.Parse("127.0.0.1");
        public static int DEFAULT_PORT = 9999;

        public static IPEndPoint DEFAULT_IP_END_POINT =
            new IPEndPoint(DEFAULT_SERVER, DEFAULT_PORT);

        UdpClient m_server = null;

        public UDPSerwer()
        {
            Init(DEFAULT_IP_END_POINT);
        }

        ~UDPSerwer()
        {
            StopSerwer();
        }

        private void StopSerwer()
        {
            m_server.Close();
        }

        private void Init(IPEndPoint ipNport)
        {
            try
            {
                m_server = new UdpClient(ipNport);
                loop();
            }
            catch (Exception e)
            {
                m_server = null;
            }
        }

        void loop()
        {
            while (true)
            {
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

                var data = m_server.Receive(ref sender);

                Console.WriteLine(DateTime.Now + " Nowe zapytanie!");

                var threadLicz = new ThreadLicz(data,sender);
                threadLicz.Run();
                //var Thread = new Thread(new ThreadStart(threadLicz.Run));
                //Thread.Name = "connection";
                //Thread.Start();

                throw new NotImplementedException();

            }
        }
    }
}