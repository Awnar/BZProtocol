﻿using protocol;
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
        private static IPAddress DEFAULT_SERVER = IPAddress.Any; //IPAddress.Parse("127.0.0.1");
        private static int DEFAULT_PORT = 9999;

        private static IPEndPoint DEFAULT_IP_END_POINT = new IPEndPoint(DEFAULT_SERVER, DEFAULT_PORT);

        private UdpClient m_server = null;

        private void Init(IPEndPoint ipNport)
        {
            try
            {
                m_server = new UdpClient(ipNport);
                loop();
            }
            catch (Exception)
            {
                m_server = null;
            }
        }

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
            if (m_server != null)
                m_server.Close();
        }

        void loop()
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                while (true)
                {
                    var data = m_server.Receive(ref sender);
                    var processing = new Processing(data);
                    data = processing.Run();
                    m_server.Send(data, data.Length, sender);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("BŁĄD... ponawianie nasłuchiwania");
                Datagram d = new Datagram();
                d.Status = 10;
                d.L1 = 101;
                var da = d.gen();
                m_server.Send(da, da.Length, sender);
                loop();
            }
        }
    }
}